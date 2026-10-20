using CommunityToolkit.Mvvm.ComponentModel;

namespace Avilonia.ExpenseTracker.Features.Dashboard;

public class CategorySummaryItem
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public double PercentageRatio { get; set; }
    public string ColorHex { get; set; } = "#3B82F6";
    public string Icon { get; set; } = "📁";
}
