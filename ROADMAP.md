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
- [x] Move the connection string from `DataContext.OnConfiguring` to User Secrets (`ConnectionStrings:Default`)
- [x] Authorization on every endpoint: customers list not public, a cleaner edits only herself, a customer sees only their own bookings, `manager` sees everything
  - [x] Customers controller (list = manager only; get/put/delete = self or manager)
  - [x] Cleaners controller (list + signup public; get = any logged-in user; put/delete = herself or manager)
  - [x] Bookings controller (customer id taken from the token; no PUT – cancel and rebook; either side may cancel)
  - [x] A cleaner can see a customer while she has an active (confirmed, not yet finished) booking with them
  - [x] Booking deletion replaced with a `Cancelled` status (`POST api/bookings/{id}/cancel`)
  - [x] Feedbacks controller (get = any logged-in user; post = customer; delete = manager only, since a feedback has no author yet)
- [x] `CleanerDTO` no longer returns entities (reviews became `AverageRating` + `ReviewCount`, availability became `AvailabilityDTO`)
- [x] **Bug fixed:** creating a feedback failed (FK error). The navigation was renamed to `Cleaner` so it matches `CleanerId`, and the stray `CleanersId` column was removed by a migration
- [x] Hide cleaners' phone number and street (`CleanerDTO` returns them as null) – shown only to the cleaner herself, a manager, and a customer with an active booking with her
- [x] Global error handling (middleware) – consistent error responses
- [x] `ShabatMiddleware`: use Israel time, return 503 with a clear message, don't block CORS `OPTIONS` requests

## Stage 2 – The matching engine (the core of the project)
### Availability
- [x] Repository + Service + Controller for `Availability` (fixed hours per weekday; several ranges per day; replace-the-week update rejected with 409 if it would push an existing future booking outside the new hours)
- [x] Blocking specific dates (vacation / sick days) – date ranges, rejected with 409 if a booking exists on one of the days
- [x] Remove the duplicate `availability` (bool) field from `Cleaners`

### Location
- [x] On signup / address update – compute `Latitude`/`Longitude` via `LocationService` and store them (city + street, then zip; unknown address = 400)
- [x] `LocationService`: use `IHttpClientFactory`, parse numbers with `CultureInfo.InvariantCulture`

### Bookings
- [x] Check that the requested hours fall within the cleaner's availability (and the date is not blocked)
- [x] Check for overlap with the cleaner's other bookings (30-minute break required between bookings; minimum booking 45 minutes)
- [x] Check that the customer is within the cleaner's distance range (`UpToKm`)
- [x] Compute `TotalPrice` = hours × `PricePerHour`
- [ ] Known limitation: two customers booking the same slot at the exact same moment can both pass the overlap check (check and save are separate steps). Real fix: a transaction or DB-level lock
- [x] Booking status: `Confirmed` / `Cancelled` stored; `Completed` is derived (a confirmed booking whose time has passed). Cancelling records who and when
- [ ] Cancellation policy (decision still open): either side may always cancel; record who cancelled and when; late cancellations (fewer than N days before the date, N not decided) are flagged and later affect the cleaner's ranking
- [x] "My bookings" for customers and cleaners

### Rating
- [x] Review with a 1–5 rating, linked to a completed booking (one review per booking)
- [x] Enable the `BadWords` filter (also fixed: the file is now loaded from the app folder, not the current directory)
- [x] Average rating and review count per cleaner

### Search and matching
- [x] `GET /api/cleaners/search?date&start&hours&maxPrice` – cleaners **available** at the requested time, within distance range (`maxPrice` is per hour; customers only)
- [x] Sort by a match score combining distance and rating (and explain the formula in the README)
- [x] Unit tests for the availability, overlap and scoring logic

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
