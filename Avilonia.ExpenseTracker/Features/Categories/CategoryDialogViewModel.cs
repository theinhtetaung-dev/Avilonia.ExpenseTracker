using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Categories;

public partial class CategoryDialogViewModel : ViewModelBase
{
    private readonly Guid? _editingId;
    private readonly Action<Category> _onSaved;

    [ObservableProperty]
    private string _dialogTitle = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _selectedColorHex = "#3B82F6";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<string> AvailableColors { get; } = new()
    {
        "#EF4444", "#F97316", "#F59E0B", "#10B981", "#14B8A6",
        "#3B82F6", "#6366F1", "#8B5CF6", "#EC4899", "#64748B"
    };

    public CategoryDialogViewModel(Category? categoryToEdit, Action<Category> onSaved)
    {
        _onSaved = onSaved;

        if (categoryToEdit != null)
        {
            _editingId = categoryToEdit.Id;
            Name = categoryToEdit.Name;
            SelectedColorHex = categoryToEdit.ColorHex;
            DialogTitle = L["EditCategory"];
        }
        else
        {
            _editingId = null;
            DialogTitle = L["AddCategory"];
            SelectedColorHex = AvailableColors[5];
        }
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        SelectedColorHex = color;
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = string.Empty;
        HasError = false;

        var trimmedName = Name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            ErrorMessage = L["ValCategoryNameRequired"];
            HasError = true;
            return;
        }

        // Check duplicate name
        var existing = DataStore.GetCategoryByName(trimmedName);
        if (existing != null && existing.Id != _editingId)
        {
            ErrorMessage = L["ValCategoryExists"];
            HasError = true;
            return;
        }

        var category = new Category
        {
            Id = _editingId ?? Guid.NewGuid(),
            Name = trimmedName,
            ColorHex = SelectedColorHex,
            Icon = string.Empty
        };

        MainViewModel.Instance?.CloseModal();
        _onSaved.Invoke(category);
    }

    [RelayCommand]
    private void Cancel()
    {
        MainViewModel.Instance?.CloseModal();
    }
}
