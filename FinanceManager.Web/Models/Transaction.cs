using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Web.Models;

public class Transaction
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Range(typeof(decimal), "0.00", "9999999999.99")]
    public decimal Amount { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [MaxLength(250)]
    public string? Notes { get; set; }
}
