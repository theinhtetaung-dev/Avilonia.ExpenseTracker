using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Features.Shared;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Expenses;

public partial class ExpensesViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategoryFilter = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _startDateFilter;

    [ObservableProperty]
    private DateTimeOffset? _endDateFilter;

    [ObservableProperty]
    private SortOptionItem? _selectedSortOption;

    [ObservableProperty]
    private Expense? _selectedExpense;

    [ObservableProperty]
    private bool _hasExpenses;

    [ObservableProperty]
    private decimal _filteredTotalAmount;

    public ObservableCollection<Expense> FilteredExpenses { get; } = new();
    public ObservableCollection<string> AvailableCategoryFilters { get; } = new();
    public ObservableCollection<SortOptionItem> AvailableSortOrders { get; } = new();

    public ExpensesViewModel()
    {
        DataStore.DataChanged += OnDataChanged;
        LoadSortOptions();
        LoadCategories();
        ApplyFilterAndSort();
    }

    private void OnDataChanged()
    {
        LoadCategories();
        ApplyFilterAndSort();
    }

    protected override void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnLocalizationChanged(sender, e);
        LoadSortOptions();
        LoadCategories();
        ApplyFilterAndSort();
    }

    private void LoadSortOptions()
    {
        var currentOrder = SelectedSortOption?.Order ?? TransactionSortOrder.NewestFirst;
        AvailableSortOrders.Clear();
        AvailableSortOrders.Add(new SortOptionItem(TransactionSortOrder.NewestFirst, L["SortNewest"]));
        AvailableSortOrders.Add(new SortOptionItem(TransactionSortOrder.OldestFirst, L["SortOldest"]));
        AvailableSortOrders.Add(new SortOptionItem(TransactionSortOrder.HighestAmount, L["SortHighestAmount"]));
        AvailableSortOrders.Add(new SortOptionItem(TransactionSortOrder.LowestAmount, L["SortLowestAmount"]));

        SelectedSortOption = AvailableSortOrders.FirstOrDefault(s => s.Order == currentOrder) ?? AvailableSortOrders[0];
    }

    partial void OnSearchTextChanged(string value) => ApplyFilterAndSort();
    partial void OnSelectedCategoryFilterChanged(string value) => ApplyFilterAndSort();
    partial void OnStartDateFilterChanged(DateTimeOffset? value) => ApplyFilterAndSort();
    partial void OnEndDateFilterChanged(DateTimeOffset? value) => ApplyFilterAndSort();
    partial void OnSelectedSortOptionChanged(SortOptionItem? value) => ApplyFilterAndSort();

    private void LoadCategories()
    {
        var prev = SelectedCategoryFilter;
        AvailableCategoryFilters.Clear();
        AvailableCategoryFilters.Add(L["FilterByCategory"]);

        foreach (var c in DataStore.GetCategories())
        {
            AvailableCategoryFilters.Add(c.Name);
        }

        if (string.IsNullOrEmpty(prev) || !AvailableCategoryFilters.Contains(prev))
        {
            SelectedCategoryFilter = AvailableCategoryFilters[0];
        }
        else
        {
            SelectedCategoryFilter = prev;
        }
    }

    public void ApplyFilterAndSort()
    {
        var query = DataStore.GetExpenses().AsEnumerable();

        // 1. Search (Title, Category, Note)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(e =>
                e.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Category.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(e.Note) && e.Note.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                L.GetCategoryDisplayName(e.Category).Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Category Filter
        if (!string.IsNullOrWhiteSpace(SelectedCategoryFilter) &&
            SelectedCategoryFilter != L["FilterByCategory"] &&
            SelectedCategoryFilter != "All Categories")
        {
            query = query.Where(e => string.Equals(e.Category, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        // 3. Start Date Filter
        if (StartDateFilter.HasValue)
        {
            var start = StartDateFilter.Value.Date;
            query = query.Where(e => e.Date.Date >= start);
        }

        // 4. End Date Filter
        if (EndDateFilter.HasValue)
        {
            var end = EndDateFilter.Value.Date;
            query = query.Where(e => e.Date.Date <= end);
        }

        // 5. Sorting
        var sortOrder = SelectedSortOption?.Order ?? TransactionSortOrder.NewestFirst;
        query = sortOrder switch
        {
            TransactionSortOrder.NewestFirst => query.OrderByDescending(e => e.Date),
            TransactionSortOrder.OldestFirst => query.OrderBy(e => e.Date),
            TransactionSortOrder.HighestAmount => query.OrderByDescending(e => e.Amount),
            TransactionSortOrder.LowestAmount => query.OrderBy(e => e.Amount),
            _ => query.OrderByDescending(e => e.Date)
        };

        FilteredExpenses.Clear();
        decimal total = 0;
        foreach (var item in query)
        {
            FilteredExpenses.Add(item);
            total += item.Amount;
        }

        FilteredTotalAmount = total;
        HasExpenses = FilteredExpenses.Count > 0;
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        if (AvailableCategoryFilters.Count > 0)
        {
            SelectedCategoryFilter = AvailableCategoryFilters[0];
        }
        StartDateFilter = null;
        EndDateFilter = null;
        if (AvailableSortOrders.Count > 0)
        {
            SelectedSortOption = AvailableSortOrders[0];
        }
    }

    [RelayCommand]
    private void AddExpense()
    {
        var dialog = new ExpenseDialogViewModel(null, exp =>
        {
            DataStore.AddExpense(exp);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void EditExpense(Expense? expense)
    {
        var target = expense ?? SelectedExpense;
        if (target == null) return;

        var dialog = new ExpenseDialogViewModel(target, updated =>
        {
            DataStore.UpdateExpense(updated);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void DeleteExpense(Expense? expense)
    {
        var target = expense ?? SelectedExpense;
        if (target == null) return;

        var confirmDialog = new ConfirmDialogViewModel(
            L["ConfirmDeleteExpenseTitle"],
            L["ConfirmDeleteExpenseMessage"],
            onConfirm: () =>
            {
                DataStore.DeleteExpense(target.Id);
            },
            confirmText: L["Delete"],
            cancelText: L["Cancel"],
            isDestructive: true);

        MainViewModel.Instance?.OpenModal(confirmDialog);
    }

    [RelayCommand]
    private void ViewDetails(Expense? expense)
    {
        var target = expense ?? SelectedExpense;
        if (target == null) return;

        var detailsMsg = $"{L["Title"]}: {target.Title}\n\n" +
                         $"{L["Amount"]}: {target.Amount:N0} MMK\n\n" +
                         $"{L["Category"]}: {L.GetCategoryDisplayName(target.Category)}\n\n" +
                         $"{L["Date"]}: {target.Date:dd-MM-yyyy}\n\n" +
                         $"{L["Note"]}: {(string.IsNullOrWhiteSpace(target.Note) ? "-" : target.Note)}";

        var msgDialog = new MessageDialogViewModel(L["ExpenseDetails"], detailsMsg, isError: false, buttonText: L["Close"]);
        MainViewModel.Instance?.OpenModal(msgDialog);
    }
}
