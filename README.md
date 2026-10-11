# Kanban Backend (ASP.NET Core Web API)

## File structure kya karta hai

- `Models/` — Database tables ka C# representation (ApplicationUser, Board, BoardColumn, TaskCard)
- `Data/ApplicationDbContext.cs` — EF Core ka setup, tables aur relationships define karta hai
- `DTOs/` — API request/response ke exact shapes (Model ko directly expose nahi karte, security ke liye)
- `Services/TokenService.cs` — JWT token banata hai login/register ke baad
- `Controllers/` — Actual API endpoints (Auth, Boards, Columns, Cards)
- `Program.cs` — Sab kuch yahan wire hota hai: database, JWT, CORS, Swagger

## Setup (Visual Studio 2026)

1. Is folder ko naye "ASP.NET Core Web API" project me copy karo (ya seedha isi folder ko VS me kholo)
2. `appsettings.json` me apni Neon connection string aur ek random JWT secret daalo
3. Package Manager Console me: `Update-Database` chalao (isse Postgres me tables ban jayenge)
4. `F5` dabao ya `dotnet run` — Swagger UI khulega `https://localhost:xxxx/swagger` pe

## Important: appsettings.json GitHub pe push mat karna

`.gitignore` me already `appsettings.json` add hai. GitHub pe sirf `appsettings.json` ka **template**
(placeholder values ke saath) push karna, jaise humne yahan banaya — real secrets kabhi commit mat karna.

## API Endpoints

| Method | Endpoint | Kya karta hai |
|--------|----------|----------------|
| POST | /api/auth/register | Naya account banao |
| POST | /api/auth/login | Login karo, JWT token milega |
| GET | /api/boards | Apne saare boards dekho |
| POST | /api/boards | Naya board banao |
| POST | /api/columns | Board me column add karo |
| POST | /api/cards | Column me card add karo |
| PUT | /api/cards/move | Card ko drag-drop karke doosre column/position me bhejo |
| DELETE | /api/cards/{id} | Card delete karo |
