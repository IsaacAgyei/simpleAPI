# UserManagementAPI

Simple ASP.NET Core Web API for managing users (in-memory store).

## What’s included

- CRUD endpoints for `User` objects
- In-memory repository implementation (singleton)
- Swagger UI in development

## Run locally

From the project root:

```bash
cd /Users/isaacagyei/Documents/simpleAPI/UserManagementAPI
dotnet build
dotnet run
```

By default the API will be available at `https://localhost:5001` (or the assigned ports). Swagger UI will be available at `/swagger` in development.

## Sample requests

- GET all users: GET /api/users
- GET one user: GET /api/users/{id}
- POST create: POST /api/users (JSON body: firstName, lastName, email, phone)
- PUT update: PUT /api/users/{id} (JSON body with fields)
- DELETE: DELETE /api/users/{id}

## Notes

This project uses an in-memory repository for demo/poc purposes only. Replace with a persistent store for production.
