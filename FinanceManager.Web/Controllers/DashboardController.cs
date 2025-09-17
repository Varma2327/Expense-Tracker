using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinanceManager.Web.Models;          // <-- Needed for CategoryType
using FinanceManager.Web.Services;

namespace FinanceManager.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ReportService reports;
        public DashboardController(ReportService reports) => this.reports = reports;

        public async Task<IActionResult> Index(int months = 6)
        {
            var data = await reports.GetMonthlyAsync(months);
            var last = data.LastOrDefault();

            if (last != null)
            {
                var breakdown = await reports.GetCategoryBreakdownAsync(last.Year, last.Month, CategoryType.Expense);
                ViewBag.BreakdownLabels = breakdown.Select(x => x.Category).ToList();
                ViewBag.BreakdownValues = breakdown.Select(x => x.Total).ToList();
            }
            else
            {
                ViewBag.BreakdownLabels = new List<string>();
                ViewBag.BreakdownValues = new List<decimal>();
            }

            return View(data);
        }
    }
}
