# Simple Trading

## 📘 Project Overview

→ [Simple Trading Client App](https://github.com/stefan8893/simple-trading-client-app?tab=readme-ov-file#project-overview)

## 🗄️ Database Setup

The application supports three database systems:

1. **SQL Server** (used in production)
2. **PostgreSQL** (used for local development)
3. **SQLite** (used for testing)

### 🔹 Production (SQL Server)

In production, the app connects to an **Azure SQL Database** via **SQL Server drivers**. From the application's perspective, it's simply using SQL Server.

### 🔹 Local Development (PostgreSQL)

On the development machine, **PostgreSQL** is used because it is already installed. No additional database system is required locally.

### 🔹 Testing (SQLite)

For testing purposes, the application uses an **in-memory SQLite database**.  
Although EF Core offers a separate in-memory provider (`Microsoft.EntityFrameworkCore.InMemory`), it lacks support for essential SQL features like **referential integrity**. SQLite provides a better balance between performance and realistic behavior in tests.

---

## 🔄 Switching Between Databases

To support multiple database systems, the project uses **Entity Framework Core**.  
EF Core implements the **repository pattern**, making it easy to switch between providers by abstracting away database-specific logic.

---

## 🧩 Adding Migrations

To add a new migration, run the following command from the `SimpleTrading.WebApi` project directory for every db provider:

```bash
dotnet ef migrations add '<__INSERT_NAME__>' --startup-project ..\SimpleTrading.WebApi\ --project ..\SimpleTrading.DataAccess.<DB_PROVIDER>\ -- --dbprovider <DB_PROVIDER>
```

Replace `<DB_PROVIDER>` with the desired provider:
**SqlServer**, **Postgres**, or **Sqlite**
