using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Features.Categories;

public class CategoryListItem
{
    public Category Model { get; }
    public Guid Id => Model.Id;
    public string Name => Model.Name;
    public string DisplayName { get; set; } = string.Empty;
    public string ColorHex => Model.ColorHex;
    public string Icon => Model.Icon;
    public int UsageCount { get; set; }

    public CategoryListItem(Category model, string displayName, int usageCount)
    {
        Model = model;
        DisplayName = displayName;
        UsageCount = usageCount;
    }
}
