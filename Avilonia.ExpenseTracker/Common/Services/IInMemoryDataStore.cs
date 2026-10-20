using System;
using System.Collections.Generic;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Common.Services;

public interface IInMemoryDataStore
{
    event Action? DataChanged;

    // Categories
    IReadOnlyList<Category> GetCategories();
    Category? GetCategoryById(Guid id);
    Category? GetCategoryByName(string name);
    void AddCategory(Category category);
    void UpdateCategory(Category category);
    bool CanDeleteCategory(string categoryName, out int usageCount);
    bool DeleteCategory(Guid id, out string errorMessage);

    // Expenses
    IReadOnlyList<Expense> GetExpenses();
    Expense? GetExpenseById(Guid id);
    void AddExpense(Expense expense);
    void UpdateExpense(Expense expense);
    bool DeleteExpense(Guid id);

    // Income
    IReadOnlyList<Income> GetIncome();
    Income? GetIncomeById(Guid id);
    void AddIncome(Income income);
    void UpdateIncome(Income income);
    bool DeleteIncome(Guid id);

    // Summary Calculations
    decimal GetTotalIncome();
    decimal GetTotalExpense();
    decimal GetCurrentBalance();
    int GetExpenseCount();
    decimal GetCurrentMonthIncome();
    decimal GetCurrentMonthExpense();
    Expense? GetHighestExpense();
    IReadOnlyList<KeyValuePair<string, decimal>> GetExpenseByCategory();
}
