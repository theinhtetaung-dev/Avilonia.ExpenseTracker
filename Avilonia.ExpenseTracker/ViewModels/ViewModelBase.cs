using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avilonia.ExpenseTracker.Common.Services;

namespace Avilonia.ExpenseTracker.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public ILocalizationService L => LocalizationService.Instance;
    protected IInMemoryDataStore DataStore => InMemoryDataStore.Instance;

    protected ViewModelBase()
    {
        LocalizationService.Instance.PropertyChanged += OnLocalizationChanged;
    }

    protected virtual void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(L));
    }
}
