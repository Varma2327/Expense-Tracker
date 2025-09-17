
# Finance Manager — Deluxe UI (Mobile-first)

**Stack:** .NET 8 • ASP.NET Core MVC • EF Core (SQLite) • Razor • Bootstrap 5 • Chart.js

## Highlights
- Mobile-first **Bootstrap 5** UI with dark mode toggle
- **Dashboard**: line chart (Income vs Expense) + pie chart (last month's expense breakdown)
- **Transactions**: filters, totals, responsive table, CSV export
- **Categories**: CRUD with clean forms and validation
- Client-side LINQ aggregations for **SQLite** compatibility
- Zero external DB: `finance.db` auto-created & seeded

## Run
```ps1
cd FinanceManager.Web
dotnet restore
dotnet run
```
Open: `http://localhost:5000`
