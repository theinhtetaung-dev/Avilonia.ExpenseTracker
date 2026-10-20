using System;

namespace Avilonia.ExpenseTracker.Common.Models;

public class Income
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Note { get; set; } = string.Empty;

    public Income Clone()
    {
        return new Income
        {
            Id = this.Id,
            Title = this.Title,
            Amount = this.Amount,
            Date = this.Date,
            Note = this.Note
        };
    }
}
