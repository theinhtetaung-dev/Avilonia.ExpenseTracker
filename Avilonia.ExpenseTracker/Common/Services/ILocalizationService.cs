using System;
using System.ComponentModel;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Common.Services;

public interface ILocalizationService : INotifyPropertyChanged
{
    AppLanguage CurrentLanguage { get; }
    void SetLanguage(AppLanguage language);
    string this[string key] { get; }
    string GetString(string key);
    string Format(string key, params object[] args);
    string GetCategoryDisplayName(string categoryName);
}
