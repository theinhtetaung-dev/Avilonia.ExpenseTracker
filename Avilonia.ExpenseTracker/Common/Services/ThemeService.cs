using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Common.Services;

public interface IThemeService : INotifyPropertyChanged
{
    AppTheme CurrentTheme { get; }
    void SetTheme(AppTheme theme);
}

public class ThemeService : IThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    private AppTheme _currentTheme = AppTheme.Light;
    public AppTheme CurrentTheme => _currentTheme;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetTheme(AppTheme theme)
    {
        _currentTheme = theme;
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = theme == AppTheme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
    }
}
