using FinanceManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        // For SQLite, decimal maps to TEXT/REAL internally via EF; fine for demo
    }
}
