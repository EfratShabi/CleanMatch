# CleanMatch

A platform that matches customers with cleaners in their area, based on **distance, availability and rating**.

The backend is an ASP.NET Core 8 Web API (layered architecture, EF Core on SQL Server, role-based JWT).
See `ROADMAP.md` for the full plan and what is done.

## Setup

Secrets are never stored in Git. After cloning, set them once with User Secrets:

```
dotnet user-secrets set "JWT:Key" "<a long random string>" --project CleanAPI
dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\MSSQLLocalDB;Database=Clean_DB;" --project CleanAPI
dotnet ef database update --project Clean.Data --startup-project CleanAPI
dotnet run --project CleanAPI
```

Swagger: http://localhost:5184/swagger

In production, set the same keys as environment variables (`JWT__Key`, `ConnectionStrings__Default`).

## How the cleaner search works

`GET /api/cleaners/search?date=2026-11-23&start=09:00&hours=2&maxPrice=80` (logged-in customers only)

The search returns only cleaners who **can really take the booking**, then ranks them.

### 1. Who is included

A cleaner appears only if all of these hold. They are the exact same rules used when a booking is created, so a cleaner shown in the results is one the booking will accept:

- the requested hours fall inside her weekly availability for that weekday
- the date is not one of her blocked dates (vacation, sick days)
- she has no other booking that overlaps (a 30-minute break is required between two bookings)
- the customer's address is within the cleaner's own `UpToKm` range
- her price per hour is not above `maxPrice` (optional)

The requested booking must be at least 45 minutes, must end on the same day, and must not be in the past.

### 2. How they are ranked

```
score = 100 x ( 0.6 x ratingScore + 0.4 x distanceScore )
```

- **distanceScore** = `1 - distance / UpToKm`, between 0 and 1.
  Distance is measured against each cleaner's *own* range, so a cleaner at the edge of her range scores 0 on distance, and one next door scores 1.
- **ratingScore** = the *adjusted* rating, scaled from 1..5 to 0..1.

The rating is adjusted so that a single review cannot decide the ranking. A plain average would put a cleaner with one 5-star review above a cleaner with fifty 4.8-star reviews. Instead:

```
adjusted = (5 x 3.5 + sum of ratings) / (5 + number of reviews)
```

A cleaner with no reviews starts at 3.5 (the middle of the scale), and every review pulls her toward her real average. The constants (`3.5`, `5`, and the weights 0.6 / 0.4) live in `Clean.Service/CleanerMatching.cs`.

Ties are broken by the lower price per hour.

### 3. What comes back

Name, city, price per hour, total price for the requested hours, average rating and review count, distance in km, and the match score. Phone number and street are not included.
