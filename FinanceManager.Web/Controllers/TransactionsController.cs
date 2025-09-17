using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceManager.Web.Data;
using FinanceManager.Web.Models;

namespace FinanceManager.Web.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;
        public TransactionsController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? categoryId)
        {
            var q = _context.Transactions
                .Include(t => t.Category)
                .AsQueryable();

            if (from.HasValue) q = q.Where(t => t.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(t => t.Date <= to.Value.Date);
            if (categoryId.HasValue) q = q.Where(t => t.CategoryId == categoryId);

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.SelectedCategoryId = categoryId;

            var list = await q.OrderByDescending(t => t.Date).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var t = await _context.Transactions.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            return View(t);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View(new Transaction { Date = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadCategories();
            return View(transaction);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var t = await _context.Transactions.FindAsync(id);
            if (t == null) return NotFound();
            await LoadCategories(t.CategoryId);
            return View(t);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction transaction)
        {
            if (id != transaction.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadCategories(transaction.CategoryId);
            return View(transaction);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var t = await _context.Transactions.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _context.Transactions.FindAsync(id);
            if (t != null)
            {
                _context.Transactions.Remove(t);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(DateTime? from, DateTime? to, int? categoryId)
        {
            var q = _context.Transactions.Include(t => t.Category).AsQueryable();
            if (from.HasValue) q = q.Where(t => t.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(t => t.Date <= to.Value.Date);
            if (categoryId.HasValue) q = q.Where(t => t.CategoryId == categoryId);

            var rows = await q.OrderByDescending(t => t.Date).ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Date,Category,Amount,Notes");
            foreach (var t in rows)
            {
                var date = t.Date.ToString("yyyy-MM-dd");
                var cat = t.Category?.Name?.Replace(',', ' ') ?? "";
                var amt = t.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var notes = (t.Notes ?? "").Replace('\n',' ').Replace('\r',' ').Replace(',', ' ');
                csv.AppendLine($"{date},{cat},{amt},{notes}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "transactions.csv");
        }

        private async Task LoadCategories(int? selectedId = null)
        {
            ViewBag.CategoryId = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", selectedId);
        }
    }
}
