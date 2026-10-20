using System;
using System.Collections.Generic;
using System.Linq;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Common.Services;

public class InMemoryDataStore : IInMemoryDataStore
{
    private static InMemoryDataStore? _instance;
    public static InMemoryDataStore Instance => _instance ??= new InMemoryDataStore();

    private readonly List<Category> _categories = new();
    private readonly List<Expense> _expenses = new();
    private readonly List<Income> _incomes = new();

    public event Action? DataChanged;

    public InMemoryDataStore()
    {
        SeedDefaultCategories();
        SeedSampleData();
    }

    private void SeedDefaultCategories()
    {
        _categories.AddRange(new[]
        {
            new Category("Food", "#F59E0B"),
            new Category("Transport", "#3B82F6"),
            new Category("Shopping", "#EC4899"),
            new Category("Bills", "#EF4444"),
            new Category("Entertainment", "#8B5CF6"),
            new Category("Health", "#10B981"),
            new Category("Education", "#06B6D4"),
            new Category("Travel", "#14B8A6"),
            new Category("Other", "#64748B")
        });
    }

    private void SeedSampleData()
    {
        var today = DateTime.Today;

        // Sample Income
        _incomes.AddRange(new[]
        {
            new Income
            {
                Title = "Monthly Salary",
                Amount = 1800000m,
                Date = new DateTime(today.Year, today.Month, Math.Min(1, today.Day)),
                Note = "Regular monthly direct deposit"
            },
            new Income
            {
                Title = "Freelance Project",
                Amount = 450000m,
                Date = today.AddDays(-5),
                Note = "Website UI redesign client milestone"
            },
            new Income
            {
                Title = "Consulting Fee",
                Amount = 150000m,
                Date = today.AddDays(-12),
                Note = "Technical consulting call"
            }
        });

        // Sample Expenses
        _expenses.AddRange(new[]
        {
            new Expense
            {
                Title = "Supermarket Groceries",
                Amount = 85000m,
                Category = "Food",
                Date = today.AddDays(-1),
                Note = "Weekly vegetable, meat and pantry refill"
            },
            new Expense
            {
                Title = "Home High-Speed Internet Bill",
                Amount = 45000m,
                Category = "Bills",
                Date = today.AddDays(-3),
                Note = "Fiber internet monthly fee"
            },
            new Expense
            {
                Title = "Taxi Ride to Downtown",
                Amount = 12000m,
                Category = "Transport",
                Date = today.AddDays(-4),
                Note = "Grab taxi meeting trip"
            },
            new Expense
            {
                Title = "Casual Dining & Dinner with Friends",
                Amount = 38000m,
                Category = "Food",
                Date = today.AddDays(-6),
                Note = "Hotpot dinner"
            },
            new Expense
            {
                Title = "New Office Headphones",
                Amount = 125000m,
                Category = "Shopping",
                Date = today.AddDays(-8),
                Note = "Noise cancelling headset"
            },
            new Expense
            {
                Title = "Cinema Movie Night & Popcorn",
                Amount = 24000m,
                Category = "Entertainment",
                Date = today.AddDays(-10),
                Note = "Weekend movie with popcorn"
            },
            new Expense
            {
                Title = "Pharmacy & Vitamins",
                Amount = 32000m,
                Category = "Health",
                Date = today.AddDays(-14),
                Note = "Multivitamins and first aid"
            },
            new Expense
            {
                Title = "Programming Book & Online Course",
                Amount = 55000m,
                Category = "Education",
                Date = today.AddDays(-16),
                Note = ".NET architecture course"
            }
        });
    }

    // Categories
    public IReadOnlyList<Category> GetCategories()
    {
        return _categories.OrderBy(c => c.Name).ToList();
    }

    public Category? GetCategoryById(Guid id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public Category? GetCategoryByName(string name)
    {
        return _categories.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public void AddCategory(Category category)
    {
        _categories.Add(category);
        DataChanged?.Invoke();
    }

    public void UpdateCategory(Category category)
    {
        var existing = GetCategoryById(category.Id);
        if (existing != null)
        {
            var oldName = existing.Name;
            existing.Name = category.Name;
            existing.ColorHex = category.ColorHex;
            existing.Icon = category.Icon;

            // If name changed, update all expenses using the old category name
            if (!string.Equals(oldName, category.Name, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var exp in _expenses.Where(e => string.Equals(e.Category, oldName, StringComparison.OrdinalIgnoreCase)))
                {
                    exp.Category = category.Name;
                }
            }

            DataChanged?.Invoke();
        }
    }

    public bool CanDeleteCategory(string categoryName, out int usageCount)
    {
        usageCount = _expenses.Count(e => string.Equals(e.Category, categoryName, StringComparison.OrdinalIgnoreCase));
        return usageCount == 0;
    }

    public bool DeleteCategory(Guid id, out string errorMessage)
    {
        errorMessage = string.Empty;
        var existing = GetCategoryById(id);
        if (existing == null)
            return false;

        if (!CanDeleteCategory(existing.Name, out var usageCount))
        {
            errorMessage = LocalizationService.Instance.Format("CategoryInUseError", existing.Name, usageCount);
            return false;
        }

        _categories.Remove(existing);
        DataChanged?.Invoke();
        return true;
    }

    // Expenses
    public IReadOnlyList<Expense> GetExpenses()
    {
        return _expenses.ToList();
    }

    public Expense? GetExpenseById(Guid id)
    {
        return _expenses.FirstOrDefault(e => e.Id == id);
    }

    public void AddExpense(Expense expense)
    {
        _expenses.Add(expense);
        DataChanged?.Invoke();
    }

    public void UpdateExpense(Expense expense)
    {
        var existing = GetExpenseById(expense.Id);
        if (existing != null)
        {
            existing.Title = expense.Title;
            existing.Amount = expense.Amount;
            existing.Category = expense.Category;
            existing.Date = expense.Date;
            existing.Note = expense.Note;
            DataChanged?.Invoke();
        }
    }

    public bool DeleteExpense(Guid id)
    {
        var existing = GetExpenseById(id);
        if (existing != null)
        {
            _expenses.Remove(existing);
            DataChanged?.Invoke();
            return true;
        }
        return false;
    }

    // Income
    public IReadOnlyList<Income> GetIncome()
    {
        return _incomes.ToList();
    }

    public Income? GetIncomeById(Guid id)
    {
        return _incomes.FirstOrDefault(i => i.Id == id);
    }

    public void AddIncome(Income income)
    {
        _incomes.Add(income);
        DataChanged?.Invoke();
    }

    public void UpdateIncome(Income income)
    {
        var existing = GetIncomeById(income.Id);
        if (existing != null)
        {
            existing.Title = income.Title;
            existing.Amount = income.Amount;
            existing.Date = income.Date;
            existing.Note = income.Note;
            DataChanged?.Invoke();
        }
    }

    public bool DeleteIncome(Guid id)
    {
        var existing = GetIncomeById(id);
        if (existing != null)
        {
            _incomes.Remove(existing);
            DataChanged?.Invoke();
            return true;
        }
        return false;
    }

    // Summaries
    public decimal GetTotalIncome()
    {
        return _incomes.Sum(i => i.Amount);
    }

    public decimal GetTotalExpense()
    {
        return _expenses.Sum(e => e.Amount);
    }

    public decimal GetCurrentBalance()
    {
        return GetTotalIncome() - GetTotalExpense();
    }

    public int GetExpenseCount()
    {
        return _expenses.Count;
    }

    public decimal GetCurrentMonthIncome()
    {
        var now = DateTime.Today;
        return _incomes
            .Where(i => i.Date.Year == now.Year && i.Date.Month == now.Month)
            .Sum(i => i.Amount);
    }

    public decimal GetCurrentMonthExpense()
    {
        var now = DateTime.Today;
        return _expenses
            .Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
            .Sum(e => e.Amount);
    }

    public Expense? GetHighestExpense()
    {
        return _expenses.OrderByDescending(e => e.Amount).FirstOrDefault();
    }

    public IReadOnlyList<KeyValuePair<string, decimal>> GetExpenseByCategory()
    {
        return _expenses
            .GroupBy(e => e.Category)
            .Select(g => new KeyValuePair<string, decimal>(g.Key, g.Sum(x => x.Amount)))
            .OrderByDescending(kv => kv.Value)
            .ToList();
    }
}
