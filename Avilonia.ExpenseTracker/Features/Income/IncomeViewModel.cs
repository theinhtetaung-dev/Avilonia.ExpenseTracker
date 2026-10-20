using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Features.Shared;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Income;

public partial class IncomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _startDateFilter;

    [ObservableProperty]
    private DateTimeOffset? _endDateFilter;

    [ObservableProperty]
    private SortOptionItem? _selectedSortOption;

    [ObservableProperty]
    private Common.Models.Income? _selectedIncome;

    [ObservableProperty]
    private bool _hasIncome;

    [ObservableProperty]
    private decimal _filteredTotalAmount;

    public ObservableCollection<Common.Models.Income> FilteredIncome { get; } = new();
    public ObservableCollection<SortOptionItem> AvailableSortOrders { get; } = new();

    public IncomeViewModel()
    {
        DataStore.DataChanged += OnDataChanged;
        LoadSortOptions();
        ApplyFilterAndSort();
    }

    private void OnDataChanged()
    {
        ApplyFilterAndSort();
    }

    protected override void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnLocalizationChanged(sender, e);
        LoadSortOptions();
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
    partial void OnStartDateFilterChanged(DateTimeOffset? value) => ApplyFilterAndSort();
    partial void OnEndDateFilterChanged(DateTimeOffset? value) => ApplyFilterAndSort();
    partial void OnSelectedSortOptionChanged(SortOptionItem? value) => ApplyFilterAndSort();

    public void ApplyFilterAndSort()
    {
        var query = DataStore.GetIncome().AsEnumerable();

        // 1. Search (Title, Note)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(i =>
                i.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(i.Note) && i.Note.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        // 2. Start Date Filter
        if (StartDateFilter.HasValue)
        {
            var start = StartDateFilter.Value.Date;
            query = query.Where(i => i.Date.Date >= start);
        }

        // 3. End Date Filter
        if (EndDateFilter.HasValue)
        {
            var end = EndDateFilter.Value.Date;
            query = query.Where(i => i.Date.Date <= end);
        }

        // 4. Sorting
        var sortOrder = SelectedSortOption?.Order ?? TransactionSortOrder.NewestFirst;
        query = sortOrder switch
        {
            TransactionSortOrder.NewestFirst => query.OrderByDescending(i => i.Date),
            TransactionSortOrder.OldestFirst => query.OrderBy(i => i.Date),
            TransactionSortOrder.HighestAmount => query.OrderByDescending(i => i.Amount),
            TransactionSortOrder.LowestAmount => query.OrderBy(i => i.Amount),
            _ => query.OrderByDescending(i => i.Date)
        };

        FilteredIncome.Clear();
        decimal total = 0;
        foreach (var item in query)
        {
            FilteredIncome.Add(item);
            total += item.Amount;
        }

        FilteredTotalAmount = total;
        HasIncome = FilteredIncome.Count > 0;
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        StartDateFilter = null;
        EndDateFilter = null;
        if (AvailableSortOrders.Count > 0)
        {
            SelectedSortOption = AvailableSortOrders[0];
        }
    }

    [RelayCommand]
    private void AddIncome()
    {
        var dialog = new IncomeDialogViewModel(null, inc =>
        {
            DataStore.AddIncome(inc);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void EditIncome(Common.Models.Income? income)
    {
        var target = income ?? SelectedIncome;
        if (target == null) return;

        var dialog = new IncomeDialogViewModel(target, updated =>
        {
            DataStore.UpdateIncome(updated);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void DeleteIncome(Common.Models.Income? income)
    {
        var target = income ?? SelectedIncome;
        if (target == null) return;

        var confirmDialog = new ConfirmDialogViewModel(
            L["ConfirmDeleteIncomeTitle"],
            L["ConfirmDeleteIncomeMessage"],
            onConfirm: () =>
            {
                DataStore.DeleteIncome(target.Id);
            },
            confirmText: L["Delete"],
            cancelText: L["Cancel"],
            isDestructive: true);

        MainViewModel.Instance?.OpenModal(confirmDialog);
    }

    [RelayCommand]
    private void ViewDetails(Common.Models.Income? income)
    {
        var target = income ?? SelectedIncome;
        if (target == null) return;

        var detailsMsg = $"{L["Title"]}: {target.Title}\n\n" +
                         $"{L["Amount"]}: {target.Amount:N0} MMK\n\n" +
                         $"{L["Date"]}: {target.Date:dd-MM-yyyy}\n\n" +
                         $"{L["Note"]}: {(string.IsNullOrWhiteSpace(target.Note) ? "-" : target.Note)}";

        var msgDialog = new MessageDialogViewModel(L["IncomeDetails"], detailsMsg, isError: false, buttonText: L["Close"]);
        MainViewModel.Instance?.OpenModal(msgDialog);
    }
}
