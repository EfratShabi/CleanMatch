# CleanMatch – Roadmap

A platform that matches customers with cleaners in their area based on **distance, availability and rating**.
A layered ASP.NET Core API with role-based JWT authentication, a React + TypeScript frontend, and an LLM agent with an MCP server that turns a free-text request into a booking.

Legend: `[ ]` to do · `[x]` done

---

## Stage 0 – Foundations and bug fixes
> Some of these fixes were made locally on 16/10 and haven't been committed yet – check and mark what already exists.

- [x] Review the local changes from 16/10, commit and push
- [x] `CleanersController`: add `await` in `Get(id)` and `Delete`
- [x] `CleanerService.GetCleanerByIDOfCleaner` – call `GetCleanerByIDOfCleanerAsync` (search by ID number)
- [x] `CleanersController.Post` – check for duplicates **before** creating the user
- [x] AutoMapper: map `CleanerPutModel → Cleaners` and `CustomerPutModel → Customers`
- [x] `CustomerRepository.GetCustomerByIdAsync` – search by `Id`; separate method to search by ID number (for the duplicate check on signup)
- [x] `CustomersController.Put` – accept `CustomerPutModel`
- [x] `CleanerRepository` – `Include` feedback and availability
- [x] Convert `Program.cs` and `BadWords.txt` to UTF-8
- [x] Move `UseCors` before `UseAuthentication`

## Stage 1 – Security
- [x] Hash passwords (BCrypt) – on signup and login
- [x] Add a `UserId` claim to the token, and extend its lifetime (6 minutes → e.g. 60)
- [x] `GET /api/auth/me` endpoint – returns the logged-in user, role, and cleaner/customer Id
- [x] Move the JWT key to User Secrets and rotate the key
- [ ] Move the connection string from `DataContext.OnConfiguring` to configuration
- [x] Authorization on every endpoint: customers list not public, a cleaner edits only herself, a customer sees only their own bookings, `manager` sees everything
  - [x] Customers controller (list = manager only; get/put/delete = self or manager)
  - [x] Cleaners controller (list + signup public; get = any logged-in user; put/delete = herself or manager)
  - [x] Bookings controller (customer id taken from the token; no PUT – cancel and rebook; either side may cancel)
  - [ ] Later: let a cleaner see a customer who has a booking with her
  - [ ] Later (Stage 2): replace booking deletion with a `Cancelled` status
  - [x] Feedbacks controller (get = any logged-in user; post = customer; delete = manager only, since a feedback has no author yet)
- [ ] `CleanerDTO` returns entities (`ArrFidback` is `List<Feedback>`, `ArrAvailability` is `List<Availability>`) – switch to DTOs
- [ ] **Bug:** creating a feedback fails (FK error). `Feedback.CleanerId` doesn't match the navigation name `Cleaners`, so EF created a second column `CleanersId` as the real FK. Fix needs a migration – ask first; fold into the Stage 2 review redesign
- [ ] Hide cleaners' phone number and street from the public cleaners list (`CleanerDTO` change) – show them only to logged-in users
- [x] Global error handling (middleware) – consistent error responses
- [x] `ShabatMiddleware`: use Israel time, return 503 with a clear message, don't block CORS `OPTIONS` requests

## Stage 2 – The matching engine (the core of the project)
### Availability
- [ ] Repository + Service + Controller for `Availability` (fixed hours per weekday)
- [ ] Blocking specific dates (vacation / sick days)
- [ ] Remove the duplicate `availability` (bool) field from `Cleaners`

### Location
- [ ] On signup / address update – compute `Latitude`/`Longitude` via `LocationService` and store them
- [ ] `LocationService`: use `IHttpClientFactory`, parse numbers with `CultureInfo.InvariantCulture`

### Bookings
- [ ] Check that the requested hours fall within the cleaner's availability
- [ ] Check for overlap with the cleaner's other bookings
- [ ] Check that the customer is within the cleaner's distance range (`UpToKm`)
- [ ] Compute `TotalPrice` = hours × `PricePerHour`
- [ ] Booking status: `Pending` / `Confirmed` / `Completed` / `Cancelled`
- [ ] "My bookings" for customers and cleaners

### Rating
- [ ] Review with a 1–5 rating, linked to a completed booking (one review per booking)
- [ ] Enable the `BadWords` filter
- [ ] Average rating and review count per cleaner

### Search and matching
- [ ] `GET /api/cleaners/search?date&start&hours&maxPrice` – cleaners **available** at the requested time, within distance range
- [ ] Sort by a match score combining distance and rating (and explain the formula in the README)
- [ ] Unit tests for the availability, overlap and scoring logic

### Cleanup
- [ ] Delete `Cleaners.Core/Data/Service` and `IDataContext`
- [ ] Move the Models to a proper namespace (`Clean.Core.Models`)

## Stage 3 – Frontend (React + TypeScript)
- [ ] Setup: Vite, TypeScript, Tailwind, shadcn/ui, React Router, React Query, RTL
- [ ] API layer + token storage + redirect to login when the token expires
- [ ] Signup and login (customer / cleaner)
- [ ] Home page + cleaner search (date, time, duration, price)
- [ ] Cleaner profile: details, rating, reviews, availability
- [ ] Booking flow and confirmation
- [ ] "My bookings" (customer) + writing a review
- [ ] Cleaner dashboard: managing availability and bookings
- [ ] Mobile layout, loading and error states

## Stage 4 – LLM agent + MCP server
- [ ] MCP server in C# (NuGet: `ModelContextProtocol`) built on the existing Services
- [ ] Tools: `search_cleaners`, `get_cleaner_profile`, `get_availability`, `create_booking_draft`
- [ ] The agent: free-text request → parameters → tool calls → suggestions
- [ ] **Human in the loop:** the agent only creates a draft; the customer confirms with a button
- [ ] Chat endpoint + chat window in React
- [ ] Test with scenarios: "tomorrow morning", "up to 80 ILS", "someone highly rated near Rehovot"

## Stage 5 – Polish and deployment
- [ ] README: description, screenshots, architecture diagram, setup instructions
- [ ] Seed data
- [ ] Docker
- [ ] Deploy to the cloud + link to a live demo
