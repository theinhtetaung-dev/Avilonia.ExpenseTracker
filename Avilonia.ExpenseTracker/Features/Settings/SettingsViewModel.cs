using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Common.Services;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ILocalizationService _locService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private bool _isMyanmarLanguage;

    [ObservableProperty]
    private bool _isEnglishLanguage;

    [ObservableProperty]
    private bool _isLightMode;

    [ObservableProperty]
    private bool _isDarkMode;

    public SettingsViewModel()
    {
        _locService = LocalizationService.Instance;
        _themeService = ThemeService.Instance;

        UpdateLanguageToggles();
        UpdateThemeToggles();
    }

    private void UpdateLanguageToggles()
    {
        IsMyanmarLanguage = _locService.CurrentLanguage == AppLanguage.Myanmar;
        IsEnglishLanguage = _locService.CurrentLanguage == AppLanguage.English;
    }

    private void UpdateThemeToggles()
    {
        IsLightMode = _themeService.CurrentTheme == AppTheme.Light;
        IsDarkMode = _themeService.CurrentTheme == AppTheme.Dark;
    }

    [RelayCommand]
    private void SelectMyanmar()
    {
        _locService.SetLanguage(AppLanguage.Myanmar);
        UpdateLanguageToggles();
    }

    [RelayCommand]
    private void SelectEnglish()
    {
        _locService.SetLanguage(AppLanguage.English);
        UpdateLanguageToggles();
    }

    [RelayCommand]
    private void SelectLightMode()
    {
        _themeService.SetTheme(AppTheme.Light);
        UpdateThemeToggles();
    }

    [RelayCommand]
    private void SelectDarkMode()
    {
        _themeService.SetTheme(AppTheme.Dark);
        UpdateThemeToggles();
    }
}
