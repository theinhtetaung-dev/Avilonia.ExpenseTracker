namespace Avilonia.ExpenseTracker.Common.Models;

public class SortOptionItem
{
    public TransactionSortOrder Order { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public SortOptionItem() { }

    public SortOptionItem(TransactionSortOrder order, string displayName)
    {
        Order = order;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}
