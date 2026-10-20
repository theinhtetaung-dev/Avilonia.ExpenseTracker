using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Expenses;

public partial class ExpenseDialogViewModel : ViewModelBase
{
    private readonly Guid? _editingId;
    private readonly Action<Expense> _onSaved;

    [ObservableProperty]
    private string _dialogTitle = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _selectedDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<string> AvailableCategories { get; } = new();

    public ExpenseDialogViewModel(Expense? expenseToEdit, Action<Expense> onSaved)
    {
        _onSaved = onSaved;

        // Load available categories from DataStore
        var categories = DataStore.GetCategories();
        foreach (var cat in categories)
        {
            AvailableCategories.Add(cat.Name);
        }

        if (expenseToEdit != null)
        {
            _editingId = expenseToEdit.Id;
            Title = expenseToEdit.Title;
            AmountText = expenseToEdit.Amount.ToString("0");
            SelectedCategory = expenseToEdit.Category;
            SelectedDate = new DateTimeOffset(expenseToEdit.Date);
            Note = expenseToEdit.Note;
            DialogTitle = L["EditExpense"];
        }
        else
        {
            _editingId = null;
            DialogTitle = L["AddExpense"];
            SelectedDate = DateTimeOffset.Now;
            if (AvailableCategories.Count > 0)
            {
                SelectedCategory = AvailableCategories[0];
            }
        }
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = string.Empty;
        HasError = false;

        var trimmedTitle = Title.Trim();
        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            ErrorMessage = L["ValTitleRequired"];
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(AmountText) || !decimal.TryParse(AmountText, out var parsedAmount))
        {
            ErrorMessage = L["ValAmountRequired"];
            HasError = true;
            return;
        }

        if (parsedAmount <= 0)
        {
            ErrorMessage = L["ValAmountPositive"];
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            ErrorMessage = L["ValCategoryRequired"];
            HasError = true;
            return;
        }

        if (!SelectedDate.HasValue)
        {
            ErrorMessage = L["ValDateRequired"];
            HasError = true;
            return;
        }

        var expense = new Expense
        {
            Id = _editingId ?? Guid.NewGuid(),
            Title = trimmedTitle,
            Amount = parsedAmount,
            Category = SelectedCategory,
            Date = SelectedDate.Value.DateTime,
            Note = Note?.Trim() ?? string.Empty
        };

        MainViewModel.Instance?.CloseModal();
        _onSaved.Invoke(expense);
    }

    [RelayCommand]
    private void Cancel()
    {
        MainViewModel.Instance?.CloseModal();
    }
}
