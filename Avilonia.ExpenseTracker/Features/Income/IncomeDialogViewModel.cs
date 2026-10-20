using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Income;

public partial class IncomeDialogViewModel : ViewModelBase
{
    private readonly Guid? _editingId;
    private readonly Action<Common.Models.Income> _onSaved;

    [ObservableProperty]
    private string _dialogTitle = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _selectedDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public IncomeDialogViewModel(Common.Models.Income? incomeToEdit, Action<Common.Models.Income> onSaved)
    {
        _onSaved = onSaved;

        if (incomeToEdit != null)
        {
            _editingId = incomeToEdit.Id;
            Title = incomeToEdit.Title;
            AmountText = incomeToEdit.Amount.ToString("0");
            SelectedDate = new DateTimeOffset(incomeToEdit.Date);
            Note = incomeToEdit.Note;
            DialogTitle = L["EditIncome"];
        }
        else
        {
            _editingId = null;
            DialogTitle = L["AddIncome"];
            SelectedDate = DateTimeOffset.Now;
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

        if (!SelectedDate.HasValue)
        {
            ErrorMessage = L["ValDateRequired"];
            HasError = true;
            return;
        }

        var income = new Common.Models.Income
        {
            Id = _editingId ?? Guid.NewGuid(),
            Title = trimmedTitle,
            Amount = parsedAmount,
            Date = SelectedDate.Value.DateTime,
            Note = Note?.Trim() ?? string.Empty
        };

        MainViewModel.Instance?.CloseModal();
        _onSaved.Invoke(income);
    }

    [RelayCommand]
    private void Cancel()
    {
        MainViewModel.Instance?.CloseModal();
    }
}
