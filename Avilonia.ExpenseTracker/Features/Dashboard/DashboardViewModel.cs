using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Features.Expenses;
using Avilonia.ExpenseTracker.Features.Income;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Dashboard;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty]
    private decimal _totalIncome;

    [ObservableProperty]
    private decimal _totalExpense;

    [ObservableProperty]
    private decimal _currentBalance;

    [ObservableProperty]
    private int _expenseCount;

    [ObservableProperty]
    private decimal _currentMonthIncome;

    [ObservableProperty]
    private decimal _currentMonthExpense;

    [ObservableProperty]
    private decimal _currentMonthNet;

    public ObservableCollection<Expense> RecentExpenses { get; } = new();
    public ObservableCollection<Common.Models.Income> RecentIncome { get; } = new();
    public ObservableCollection<CategorySummaryItem> CategorySummaries { get; } = new();

    public DashboardViewModel()
    {
        DataStore.DataChanged += RefreshData;
        RefreshData();
    }

    protected override void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnLocalizationChanged(sender, e);
        // Refresh display names
        RefreshData();
    }

    public void RefreshData()
    {
        TotalIncome = DataStore.GetTotalIncome();
        TotalExpense = DataStore.GetTotalExpense();
        CurrentBalance = DataStore.GetCurrentBalance();
        ExpenseCount = DataStore.GetExpenseCount();

        CurrentMonthIncome = DataStore.GetCurrentMonthIncome();
        CurrentMonthExpense = DataStore.GetCurrentMonthExpense();
        CurrentMonthNet = CurrentMonthIncome - CurrentMonthExpense;

        // Recent expenses (latest 5)
        RecentExpenses.Clear();
        foreach (var exp in DataStore.GetExpenses().OrderByDescending(e => e.Date).Take(5))
        {
            RecentExpenses.Add(exp);
        }

        // Recent income (latest 5)
        RecentIncome.Clear();
        foreach (var inc in DataStore.GetIncome().OrderByDescending(i => i.Date).Take(5))
        {
            RecentIncome.Add(inc);
        }

        // Category breakdown
        CategorySummaries.Clear();
        var categoryExpenses = DataStore.GetExpenseByCategory();
        var totalExp = TotalExpense > 0 ? TotalExpense : 1;

        foreach (var kv in categoryExpenses)
        {
            var catObj = DataStore.GetCategoryByName(kv.Key);
            var percentage = (double)(kv.Value / totalExp * 100);
            CategorySummaries.Add(new CategorySummaryItem
            {
                Name = kv.Key,
                DisplayName = L.GetCategoryDisplayName(kv.Key),
                Amount = kv.Value,
                Percentage = Math.Round(percentage, 1),
                PercentageRatio = (double)(kv.Value / totalExp),
                ColorHex = catObj?.ColorHex ?? "#3B82F6",
                Icon = catObj?.Icon ?? "📁"
            });
        }
    }

    [RelayCommand]
    private void QuickAddExpense()
    {
        var dialog = new ExpenseDialogViewModel(null, exp =>
        {
            DataStore.AddExpense(exp);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void QuickAddIncome()
    {
        var dialog = new IncomeDialogViewModel(null, inc =>
        {
            DataStore.AddIncome(inc);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void NavigateToExpenses()
    {
        MainViewModel.Instance?.NavigateToExpenses();
    }

    [RelayCommand]
    private void NavigateToIncome()
    {
        MainViewModel.Instance?.NavigateToIncome();
    }
}
