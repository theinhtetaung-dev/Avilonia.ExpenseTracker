using System;

namespace Avilonia.ExpenseTracker.Common.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#3B82F6";
    public string Icon { get; set; } = string.Empty;

    public Category() { }

    public Category(string name, string colorHex = "#3B82F6", string icon = "")
    {
        Id = Guid.NewGuid();
        Name = name;
        ColorHex = colorHex;
        Icon = icon;
    }

    public Category Clone()
    {
        return new Category
        {
            Id = this.Id,
            Name = this.Name,
            ColorHex = this.ColorHex,
            Icon = this.Icon
        };
    }
}
