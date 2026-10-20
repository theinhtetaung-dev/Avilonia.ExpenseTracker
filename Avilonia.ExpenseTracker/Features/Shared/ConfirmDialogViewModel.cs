using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Shared;

public partial class ConfirmDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _confirmText = string.Empty;

    [ObservableProperty]
    private string _cancelText = string.Empty;

    [ObservableProperty]
    private bool _isDestructive = true;

    private readonly Action _onConfirm;
    private readonly Action? _onCancel;

    public ConfirmDialogViewModel(
        string title,
        string message,
        Action onConfirm,
        Action? onCancel = null,
        string? confirmText = null,
        string? cancelText = null,
        bool isDestructive = true)
    {
        Title = title;
        Message = message;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        ConfirmText = confirmText ?? L["Confirm"];
        CancelText = cancelText ?? L["Cancel"];
        IsDestructive = isDestructive;
    }

    [RelayCommand]
    private void Confirm()
    {
        MainViewModel.Instance?.CloseModal();
        _onConfirm.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        MainViewModel.Instance?.CloseModal();
        _onCancel?.Invoke();
    }
}
