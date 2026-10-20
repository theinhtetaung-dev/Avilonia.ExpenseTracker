using System;

namespace Avilonia.ExpenseTracker.Common.Models;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public string Note { get; set; } = string.Empty;

    public Expense Clone()
    {
        return new Expense
        {
            Id = this.Id,
            Title = this.Title,
            Amount = this.Amount,
            Category = this.Category,
            Date = this.Date,
            Note = this.Note
        };
    }
}
