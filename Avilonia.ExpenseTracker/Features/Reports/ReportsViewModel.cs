using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Features.Dashboard;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Reports;

public partial class ReportsViewModel : ViewModelBase
{
    [ObservableProperty]
    private decimal _totalIncome;

    [ObservableProperty]
    private decimal _totalExpense;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private decimal _currentMonthIncome;

    [ObservableProperty]
    private decimal _currentMonthExpense;

    [ObservableProperty]
    private decimal _currentMonthBalance;

    [ObservableProperty]
    private decimal _averageExpense;

    [ObservableProperty]
    private int _totalTransactions;

    [ObservableProperty]
    private Expense? _highestExpense;

    [ObservableProperty]
    private bool _hasHighestExpense;

    public ObservableCollection<CategorySummaryItem> CategoryBreakdown { get; } = new();
    public ObservableCollection<Expense> RecentSpending { get; } = new();

    public ReportsViewModel()
    {
        DataStore.DataChanged += GenerateReports;
        GenerateReports();
    }

    protected override void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnLocalizationChanged(sender, e);
        GenerateReports();
    }

    public void GenerateReports()
    {
        TotalIncome = DataStore.GetTotalIncome();
        TotalExpense = DataStore.GetTotalExpense();
        Balance = TotalIncome - TotalExpense;

        CurrentMonthIncome = DataStore.GetCurrentMonthIncome();
        CurrentMonthExpense = DataStore.GetCurrentMonthExpense();
        CurrentMonthBalance = CurrentMonthIncome - CurrentMonthExpense;

        var allExpenses = DataStore.GetExpenses();
        var allIncome = DataStore.GetIncome();
        TotalTransactions = allExpenses.Count + allIncome.Count;

        AverageExpense = allExpenses.Count > 0
            ? Math.Round(TotalExpense / allExpenses.Count, 0)
            : 0;

        HighestExpense = DataStore.GetHighestExpense();
        HasHighestExpense = HighestExpense != null;

        // Recent spending
        RecentSpending.Clear();
        foreach (var exp in allExpenses.OrderByDescending(e => e.Date).Take(8))
        {
            RecentSpending.Add(exp);
        }

        // Category breakdown
        CategoryBreakdown.Clear();
        var categoryExpenses = DataStore.GetExpenseByCategory();
        var totalExp = TotalExpense > 0 ? TotalExpense : 1;

        foreach (var kv in categoryExpenses)
        {
            var catObj = DataStore.GetCategoryByName(kv.Key);
            var pct = (double)(kv.Value / totalExp * 100);
            CategoryBreakdown.Add(new CategorySummaryItem
            {
                Name = kv.Key,
                DisplayName = L.GetCategoryDisplayName(kv.Key),
                Amount = kv.Value,
                Percentage = Math.Round(pct, 1),
                PercentageRatio = (double)(kv.Value / totalExp),
                ColorHex = catObj?.ColorHex ?? "#3B82F6",
                Icon = catObj?.Icon ?? "📁"
            });
        }
    }
}
