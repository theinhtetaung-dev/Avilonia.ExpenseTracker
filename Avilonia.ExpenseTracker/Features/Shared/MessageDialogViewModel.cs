using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Shared;

public partial class MessageDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private string _buttonText;

    [ObservableProperty]
    private bool _isError;

    public MessageDialogViewModel(string title, string message, bool isError = false, string? buttonText = null)
    {
        _title = title;
        _message = message;
        _isError = isError;
        _buttonText = buttonText ?? L["Close"];
    }

    [RelayCommand]
    private void Close()
    {
        MainViewModel.Instance?.CloseModal();
    }
}
