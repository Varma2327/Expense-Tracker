
# Personal Finance Manager (SQLite build) — One-hour polish

**Tech:** .NET 8, ASP.NET Core MVC, EF Core (SQLite), Razor, Chart.js

## What I implemented
- Dashboard with **Income vs Expense line chart** and **expense breakdown pie** (last month).
- Full CRUD for **Transactions** and **Categories**.
- Filters + totals on Transactions; **Export CSV** for reports.
- Client-side LINQ aggregations to keep **SQLite** happy (no tricky translations).
- Auto-create + seed data; zero external DB setup.

## How to run
```ps1
cd FinanceManager.Web
dotnet restore
dotnet run
```
Browse: `http://localhost:5000`

## Resume bullets (pasteable)
- Built a full-stack **ASP.NET Core MVC** app with **EF Core** (SQLite) and **Razor** views.
- Implemented **LINQ-based reporting**, **Chart.js visualizations**, filtering, and CSV export.
- Designed **code-first** models and data seeding; handled **SQLite provider quirks** with client-side aggregation.
- Deployed-ready for Azure App Service; can switch to **SQL Server/Azure SQL** by changing the provider.
