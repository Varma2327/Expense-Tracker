using FinanceManager.Web.Data;
using FinanceManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Web.Services
{
    public record MonthlySummary(int Year, int Month, decimal Income, decimal Expense);

    public class ReportService
    {
        private readonly AppDbContext db;
        public ReportService(AppDbContext db) => this.db = db;

        public async Task<List<MonthlySummary>> GetMonthlyAsync(int monthsBack = 6)
        {
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year, now.Month, 1).AddMonths(-monthsBack + 1);

            // Materialize, then aggregate client-side (SQLite friendly)
            var rows = await db.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= from)
                .AsNoTracking()
                .ToListAsync();

            var result = rows
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new MonthlySummary(
                    g.Key.Year,
                    g.Key.Month,
                    g.Where(x => x.Category!.Type == CategoryType.Income).Sum(x => x.Amount),
                    g.Where(x => x.Category!.Type == CategoryType.Expense).Sum(x => x.Amount)
                ))
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            return result;
        }

        public async Task<List<(string Category, decimal Total)>> GetCategoryBreakdownAsync(int year, int month, CategoryType type)
        {
            var rows = await db.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date.Year == year && t.Date.Month == month && t.Category!.Type == type)
                .AsNoTracking()
                .ToListAsync();

            var result = rows
                .GroupBy(t => t.Category!.Name)
                .Select(g => (Category: g.Key, Total: g.Sum(x => x.Amount)))
                .OrderByDescending(x => x.Total)
                .ToList();

            return result;
        }
    }
}
