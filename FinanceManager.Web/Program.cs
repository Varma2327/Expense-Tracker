using FinanceManager.Web.Data;
using FinanceManager.Web.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=finance.db";
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(cs));
builder.Services.AddScoped<FinanceManager.Web.Services.ReportService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DemoSeed.Run(db);
}

app.Run();

static class DemoSeed
{
    public static void Run(AppDbContext db)
    {
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Id = 1, Name = "Salary", Type = CategoryType.Income },
                new Category { Id = 2, Name = "Freelance", Type = CategoryType.Income },
                new Category { Id = 3, Name = "Food", Type = CategoryType.Expense },
                new Category { Id = 4, Name = "Rent", Type = CategoryType.Expense },
                new Category { Id = 5, Name = "Travel", Type = CategoryType.Expense }
            );
            db.SaveChanges();
        }

        if (!db.Transactions.Any())
        {
            var today = DateTime.UtcNow.Date;
            var rnd = new Random(42);
            for (int m = 0; m < 3; m++)
            {
                var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-m);
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(0), Amount = 3500 + (m * 100), CategoryId = 1, Notes = "Monthly salary" });
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(10), Amount = 600 + (m * 50), CategoryId = 2, Notes = "Freelance gig" });
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(1), Amount = 900, CategoryId = 4, Notes = "Apartment rent" });
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(3), Amount = 120 + rnd.Next(0,50), CategoryId = 3, Notes = "Groceries" });
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(7), Amount = 45 + rnd.Next(0,20), CategoryId = 3, Notes = "Eating out" });
                db.Transactions.Add(new Transaction { Date = monthStart.AddDays(15), Amount = 80 + rnd.Next(0,40), CategoryId = 5, Notes = "Taxi/Metro" });
            }
            db.SaveChanges();
        }
    }
}
