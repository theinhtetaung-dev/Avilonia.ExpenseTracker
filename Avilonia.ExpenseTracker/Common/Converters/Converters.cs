using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avilonia.ExpenseTracker.Common.Services;

namespace Avilonia.ExpenseTracker.Common.Converters;

public class MmkCurrencyConverter : IValueConverter
{
    public static readonly MmkCurrencyConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal dec)
        {
            var prefix = parameter as string ?? string.Empty;
            return $"{prefix}{dec:N0} MMK";
        }

        if (value is int i)
        {
            var prefix = parameter as string ?? string.Empty;
            return $"{prefix}{i:N0} MMK";
        }

        return "0 MMK";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class AppDateFormatConverter : IValueConverter
{
    public static readonly AppDateFormatConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
        }
        if (value is DateTimeOffset dto)
        {
            return dto.ToString("dd-MM-yyyy");
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && DateTime.TryParseExact(s, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt;
        }
        return null;
    }
}

public class CategoryDisplayConverter : IValueConverter
{
    public static readonly CategoryDisplayConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string catName)
        {
            return LocalizationService.Instance.GetCategoryDisplayName(catName);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class CategoryColorConverter : IValueConverter
{
    public static readonly CategoryColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var categoryName = value as string;
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var cat = InMemoryDataStore.Instance.GetCategoryByName(categoryName);
            if (cat != null && !string.IsNullOrWhiteSpace(cat.ColorHex))
            {
                if (Color.TryParse(cat.ColorHex, out var color))
                {
                    return new SolidColorBrush(color);
                }
            }
        }
        return new SolidColorBrush(Color.Parse("#3B82F6"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class CategoryIconConverter : IValueConverter
{
    public static readonly CategoryIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var categoryName = value as string;
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var cat = InMemoryDataStore.Instance.GetCategoryByName(categoryName);
            if (cat != null && !string.IsNullOrWhiteSpace(cat.Icon))
            {
                return cat.Icon;
            }
        }
        return "📁";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class IsPositiveColorConverter : IValueConverter
{
    public static readonly IsPositiveColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal d)
        {
            return d >= 0
                ? new SolidColorBrush(Color.Parse("#16A34A")) // Green
                : new SolidColorBrush(Color.Parse("#DC2626")); // Red
        }
        return new SolidColorBrush(Color.Parse("#0F172A"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
