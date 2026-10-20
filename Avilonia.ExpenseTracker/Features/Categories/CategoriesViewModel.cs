using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avilonia.ExpenseTracker.Common.Models;
using Avilonia.ExpenseTracker.Features.Shared;
using Avilonia.ExpenseTracker.ViewModels;

namespace Avilonia.ExpenseTracker.Features.Categories;

public partial class CategoriesViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _hasCategories;

    public ObservableCollection<CategoryListItem> Categories { get; } = new();

    public CategoriesViewModel()
    {
        DataStore.DataChanged += LoadCategories;
        LoadCategories();
    }

    protected override void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnLocalizationChanged(sender, e);
        LoadCategories();
    }

    public void LoadCategories()
    {
        Categories.Clear();
        var allCats = DataStore.GetCategories();

        foreach (var cat in allCats)
        {
            DataStore.CanDeleteCategory(cat.Name, out var count);
            var displayName = L.GetCategoryDisplayName(cat.Name);
            Categories.Add(new CategoryListItem(cat, displayName, count));
        }

        HasCategories = Categories.Count > 0;
    }

    [RelayCommand]
    private void AddCategory()
    {
        var dialog = new CategoryDialogViewModel(null, newCategory =>
        {
            DataStore.AddCategory(newCategory);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void EditCategory(CategoryListItem? item)
    {
        if (item == null) return;

        var dialog = new CategoryDialogViewModel(item.Model, updated =>
        {
            DataStore.UpdateCategory(updated);
        });
        MainViewModel.Instance?.OpenModal(dialog);
    }

    [RelayCommand]
    private void DeleteCategory(CategoryListItem? item)
    {
        if (item == null) return;

        // Check if currently used by any expense
        if (!DataStore.CanDeleteCategory(item.Name, out var usageCount))
        {
            var warningMessage = L.Format("CategoryInUseError", item.DisplayName, usageCount);
            var warningDialog = new MessageDialogViewModel(
                L["Warning"],
                warningMessage,
                isError: true,
                buttonText: L["Close"]);

            MainViewModel.Instance?.OpenModal(warningDialog);
            return;
        }

        // Category is not in use, confirm deletion
        var confirmMsg = L.Format("ConfirmDeleteCategoryMessage", item.DisplayName);
        var confirmDialog = new ConfirmDialogViewModel(
            L["ConfirmDeleteCategoryTitle"],
            confirmMsg,
            onConfirm: () =>
            {
                DataStore.DeleteCategory(item.Id, out _);
            },
            confirmText: L["Delete"],
            cancelText: L["Cancel"],
            isDestructive: true);

        MainViewModel.Instance?.OpenModal(confirmDialog);
    }
}
