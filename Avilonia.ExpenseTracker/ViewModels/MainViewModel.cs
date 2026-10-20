using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Common.Services;
using Avilonia.ExpenseTracker.Features.Categories;
using Avilonia.ExpenseTracker.Features.Dashboard;
using Avilonia.ExpenseTracker.Features.Expenses;
using Avilonia.ExpenseTracker.Features.Income;
using Avilonia.ExpenseTracker.Features.Reports;
using Avilonia.ExpenseTracker.Features.Settings;

namespace Avilonia.ExpenseTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public static MainViewModel? Instance { get; private set; }

    private readonly DashboardViewModel _dashboardVm;
    private readonly ExpensesViewModel _expensesVm;
    private readonly IncomeViewModel _incomeVm;
    private readonly CategoriesViewModel _categoriesVm;
    private readonly ReportsViewModel _reportsVm;
    private readonly SettingsViewModel _settingsVm;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private string _currentTab = "Dashboard";

    [ObservableProperty]
    private ViewModelBase? _activeModal;

    [ObservableProperty]
    private bool _isModalOpen;

    public MainViewModel()
    {
        Instance = this;

        _dashboardVm = new DashboardViewModel();
        _expensesVm = new ExpensesViewModel();
        _incomeVm = new IncomeViewModel();
        _categoriesVm = new CategoriesViewModel();
        _reportsVm = new ReportsViewModel();
        _settingsVm = new SettingsViewModel();

        _currentPage = _dashboardVm;
    }

    [RelayCommand]
    public void NavigateToDashboard()
    {
        _dashboardVm.RefreshData();
        CurrentPage = _dashboardVm;
        CurrentTab = "Dashboard";
    }

    [RelayCommand]
    public void NavigateToExpenses()
    {
        _expensesVm.ApplyFilterAndSort();
        CurrentPage = _expensesVm;
        CurrentTab = "Expenses";
    }

    [RelayCommand]
    public void NavigateToIncome()
    {
        _incomeVm.ApplyFilterAndSort();
        CurrentPage = _incomeVm;
        CurrentTab = "Income";
    }

    [RelayCommand]
    public void NavigateToCategories()
    {
        _categoriesVm.LoadCategories();
        CurrentPage = _categoriesVm;
        CurrentTab = "Categories";
    }

    [RelayCommand]
    public void NavigateToReports()
    {
        _reportsVm.GenerateReports();
        CurrentPage = _reportsVm;
        CurrentTab = "Reports";
    }

    [RelayCommand]
    public void NavigateToSettings()
    {
        CurrentPage = _settingsVm;
        CurrentTab = "Settings";
    }

    [RelayCommand]
    public void ToggleTheme()
    {
        var current = ThemeService.Instance.CurrentTheme;
        ThemeService.Instance.SetTheme(current == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }

    public void OpenModal(ViewModelBase modal)
    {
        ActiveModal = modal;
        IsModalOpen = true;
    }

    public void CloseModal()
    {
        ActiveModal = null;
        IsModalOpen = false;
    }
}
