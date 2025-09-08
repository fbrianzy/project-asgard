# Quick Start

```bash
dotnet new webapi -n TaskTracker.Api -o tasktracker-api/src/TaskTracker.Api
cd tasktracker-api/src/TaskTracker.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
dotnet tool install --global dotnet-ef

# migrasi & seed
dotnet ef migrations add Init
dotnet ef database update

# run
dotnet run
# buka Swagger: http://localhost:5189/swagger
```