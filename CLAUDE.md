# CleanMatch – Instructions for Claude

## About the project
CleanMatch is a platform that matches customers with cleaners in their area based on distance, availability and rating.
- **Backend:** ASP.NET Core 8 Web API, layered architecture, EF Core + SQL Server (LocalDB), role-based JWT authentication
- **Frontend (planned):** React + TypeScript (Vite, Tailwind, shadcn/ui), full RTL / Hebrew support
- **AI (planned):** an MCP server in C# that exposes the business logic as tools, and an LLM agent that turns a free-text request into a booking draft

The full work plan is in `ROADMAP.md`. **Always work according to it.**

## Solution structure
| Project | Responsibility |
|---|---|
| `Clean.Core` | Entities, DTOs, Models, Repository and Service interfaces, AutoMapper profile |
| `Clean.Data` | `DataContext`, Repository implementations, Migrations |
| `Clean.Service` | Business logic (Services), `LocationService` |
| `CleanAPI` | Controllers, Program.cs, Middlewares (including `ShabatMiddleware`), Post Model mappings |
| `CleanerTest` | xUnit tests |
| `Cleaners.*` | Old empty projects – scheduled for deletion (see ROADMAP) |

## Working rules
1. **Reply to me in Hebrew.** Code, identifiers and commit messages in English.
2. **I'm learning as we go.** Before every change, briefly explain the problem, the fix and why. When a new concept comes up (e.g. hashing, `Include`, middleware), add a sentence or two of explanation.
3. **Wait for my approval before changing files.** If I say "let me write this myself", give me guidance and hints, not the full code, then review what I wrote.
4. **Small steps:** one ROADMAP task at a time.
5. **After every task:**
   - Run `dotnet build` and make sure there are 0 errors (and `dotnet test` once tests exist)
   - Mark the task `[x]` in `ROADMAP.md`
   - Suggest a commit with a clear English message (e.g. `fix: await async calls in CleanersController`)
6. **Don't invent requirements.** If something is unclear or there are several good options, ask me.

## Code rules
- All database calls are **asynchronous** (`async`/`await`) through every layer.
- Controllers **never return Entities** – only DTOs. Input comes in through Post/Put Models.
- Business logic belongs in the **Service** layer, not in Controllers or Repositories.
- Every new Controller or endpoint needs authorization thought through (`[Authorize(Roles = ...)]`). Use `AllowAnonymous` only when truly needed.
- **Secrets** (JWT key, connection string) never go in code or in an `appsettings.json` that is committed to Git. Use User Secrets. The repository is public.
- **Migrations:** ask me before creating a new migration. Command: `dotnet ef migrations add <Name> --project Clean.Data --startup-project CleanAPI`
- `ShabatMiddleware` is a project requirement and stays. It may be improved (Israel time zone, real Shabbat times) – never removed.
- Save files as **UTF-8**.

## Running
```
dotnet build
dotnet run --project CleanAPI
```
Swagger: http://localhost:5184/swagger
