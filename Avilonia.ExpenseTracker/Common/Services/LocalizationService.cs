using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avilonia.ExpenseTracker.Common.Models;

namespace Avilonia.ExpenseTracker.Common.Services;

public class LocalizationService : ILocalizationService
{
    private static LocalizationService? _instance;
    public static LocalizationService Instance => _instance ??= new LocalizationService();

    // Default language is Myanmar as required!
    private AppLanguage _currentLanguage = AppLanguage.Myanmar;

    public AppLanguage CurrentLanguage => _currentLanguage;

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Dictionary<string, string> _myanmarStrings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Navigation
        ["AppTitle"] = "အသုံးစရိတ် မှတ်တမ်း",
        ["NavDashboard"] = "ပင်မစာမျက်နှာ",
        ["NavExpenses"] = "အသုံးစရိတ်များ",
        ["NavIncome"] = "ဝင်ငွေများ",
        ["NavCategories"] = "အမျိုးအစားများ",
        ["NavReports"] = "အစီရင်ခံစာများ",
        ["NavSettings"] = "ဆက်တင်များ",

        // Common Actions
        ["Add"] = "အသစ်ထည့်ရန်",
        ["Edit"] = "ပြင်ဆင်ရန်",
        ["Delete"] = "ဖျက်ရန်",
        ["Save"] = "သိမ်းရန်",
        ["Cancel"] = "မလုပ်တော့ပါ",
        ["Search"] = "ရှာရန်",
        ["Clear"] = "ရှင်းရန်",
        ["Close"] = "ပိတ်ရန်",
        ["Confirm"] = "အတည်ပြုရန်",
        ["Actions"] = "လုပ်ဆောင်ချက်များ",
        ["ViewDetails"] = "အသေးစိတ်ကြည့်ရန်",
        ["Back"] = "နောက်သို့",
        ["Ok"] = "ကောင်းပြီ",

        // Dashboard
        ["TotalIncome"] = "စုစုပေါင်း ဝင်ငွေ",
        ["TotalExpense"] = "စုစုပေါင်း အသုံးစရိတ်",
        ["CurrentBalance"] = "လက်ကျန်ငွေ",
        ["NumberOfExpenses"] = "အသုံးစရိတ် အရေအတွက်",
        ["RecentExpenses"] = "လတ်တလော အသုံးစရိတ်များ",
        ["RecentIncome"] = "လတ်တလော ဝင်ငွေများ",
        ["CurrentMonthSummary"] = "ယခုလ အကျဉ်းချုပ်",
        ["ExpenseByCategory"] = "အမျိုးအစားအလိုက် အသုံးစရိတ်",
        ["MonthlyNetSavings"] = "ယခုလ အသားတင် စုဆောင်းငွေ",
        ["MonthlyIncome"] = "လစဉ် ဝင်ငွေ",
        ["MonthlyExpense"] = "လစဉ် အသုံးစရိတ်",
        ["QuickOverview"] = "ဘဏ္ဍာရေး အနှစ်ချုပ်",

        // Expenses
        ["ExpensesTitle"] = "အသုံးစရိတ် စာရင်း",
        ["AddExpense"] = "အသုံးစရိတ် အသစ်ထည့်ရန်",
        ["EditExpense"] = "အသုံးစရိတ် ပြင်ဆင်ရန်",
        ["DeleteExpense"] = "အသုံးစရိတ် ဖျက်ရန်",
        ["ExpenseDetails"] = "အသုံးစရိတ် အသေးစိတ်",
        ["SearchExpensesPlaceholder"] = "ခေါင်းစဉ်၊ အမျိုးအစား၊ မှတ်ချက်ဖြင့် ရှာရန်...",
        ["NoExpensesFound"] = "အသုံးစရိတ် မရှိသေးပါ။",
        ["ConfirmDeleteExpenseTitle"] = "အသုံးစရိတ် ဖျက်ရန် အတည်ပြုချက်",
        ["ConfirmDeleteExpenseMessage"] = "ဤအသုံးစရိတ်ကို ဖျက်ရန် သေချာပါသလား? ဖျက်ပြီးပါက ပြန်လည်ရယူ၍ မရနိုင်ပါ။",

        // Income
        ["IncomeTitle"] = "ဝင်ငွေ စာရင်း",
        ["AddIncome"] = "ဝင်ငွေ အသစ်ထည့်ရန်",
        ["EditIncome"] = "ဝင်ငွေ ပြင်ဆင်ရန်",
        ["DeleteIncome"] = "ဝင်ငွေ ဖျက်ရန်",
        ["IncomeDetails"] = "ဝင်ငွေ အသေးစိတ်",
        ["SearchIncomePlaceholder"] = "ခေါင်းစဉ်၊ မှတ်ချက်ဖြင့် ရှာရန်...",
        ["NoIncomeFound"] = "ဝင်ငွေ မရှိသေးပါ။",
        ["ConfirmDeleteIncomeTitle"] = "ဝင်ငွေ ဖျက်ရန် အတည်ပြုချက်",
        ["ConfirmDeleteIncomeMessage"] = "ဤဝင်ငွေကို ဖျက်ရန် သေချာပါသလား? ဖျက်ပြီးပါက ပြန်လည်ရယူ၍ မရနိုင်ပါ။",

        // Fields
        ["Title"] = "ခေါင်းစဉ်",
        ["Amount"] = "ပမာဏ (MMK)",
        ["Category"] = "အမျိုးအစား",
        ["Date"] = "ရက်စွဲ (ရက်-လ-နှစ်)",
        ["Note"] = "မှတ်ချက်",
        ["EnterTitlePlaceholder"] = "ခေါင်းစဉ် ထည့်သွင်းပါ...",
        ["EnterAmountPlaceholder"] = "ပမာဏ (ကျပ်) ထည့်သွင်းပါ...",
        ["SelectCategoryPlaceholder"] = "အမျိုးအစား ရွေးချယ်ပါ",
        ["EnterNotePlaceholder"] = "မှတ်ချက် အသေးစိတ် ထည့်သွင်းပါ (မထည့်လည်းရပါသည်)...",

        // Filters & Sorting
        ["FilterByCategory"] = "အမျိုးအစား အားလုံး",
        ["FromDate"] = "စတင်ရက်",
        ["ToDate"] = "ပြီးဆုံးရက်",
        ["ClearFilters"] = "စစ်ထုတ်မှုများ ရှင်းရန်",
        ["SortBy"] = "စီစဉ်ရန်",
        ["SortNewest"] = "အသစ်ဆုံး အရင်",
        ["SortOldest"] = "အဟောင်းဆုံး အရင်",
        ["SortHighestAmount"] = "ပမာဏ အများဆုံး",
        ["SortLowestAmount"] = "ပမာဏ အနည်းဆုံး",

        // Categories
        ["CategoriesTitle"] = "အမျိုးအစား စီမံခန့်ခွဲမှု",
        ["AddCategory"] = "အမျိုးအစား အသစ်ထည့်ရန်",
        ["EditCategory"] = "အမျိုးအစား ပြင်ဆင်ရန်",
        ["DeleteCategory"] = "အမျိုးအစား ဖျက်ရန်",
        ["CategoryName"] = "အမျိုးအစား အမည်",
        ["CategoryColor"] = "အရောင် ရွေးချယ်ရန်",
        ["CategoryIcon"] = "အိုင်ကွန် / သင်္ကေတ",
        ["NoCategoriesFound"] = "အမျိုးအစား မရှိသေးပါ။",
        ["ConfirmDeleteCategoryTitle"] = "အမျိုးအစား ဖျက်ရန် အတည်ပြုချက်",
        ["ConfirmDeleteCategoryMessage"] = "'{0}' အမျိုးအစားကို ဖျက်ရန် သေချာပါသလား?",
        ["CategoryInUseError"] = "'{0}' အမျိုးအစားကို ဖျက်၍မရပါ၊ လက်ရှိတွင် အသုံးစရိတ် ({1}) ခု၌ အသုံးပြုနေပါသည်။",
        ["CategoryNamePlaceholder"] = "အမျိုးအစား အမည် ထည့်သွင်းပါ...",

        // Default Categories
        ["Category_Food"] = "အစားအသောက်",
        ["Category_Transport"] = "သယ်ယူပို့ဆောင်ရေး",
        ["Category_Shopping"] = "စျေးဝယ်ခြင်း",
        ["Category_Bills"] = "ဘေလ်နှင့် လစဉ်စရိတ်များ",
        ["Category_Entertainment"] = "အပန်းဖြေခြင်း",
        ["Category_Health"] = "ကျန်းမာရေး",
        ["Category_Education"] = "ပညာရေး",
        ["Category_Travel"] = "ခရီးသွားခြင်း",
        ["Category_Other"] = "အခြား",

        // Reports
        ["ReportsTitle"] = "ဘဏ္ဍာရေး အစီရင်ခံစာများ",
        ["HighestExpense"] = "အများဆုံး အသုံးစရိတ်",
        ["RecentSpending"] = "လတ်တလော အသုံးစရိတ်များ",
        ["TotalSpending"] = "စုစုပေါင်း အသုံးစရိတ်",
        ["AverageExpense"] = "ပျမ်းမျှ အသုံးစရိတ်",
        ["CategoryBreakdown"] = "အမျိုးအစားအလိုက် အသုံးစရိတ် ခွဲခြမ်းစိတ်ဖြာချက်",
        ["Percentage"] = "ရာခိုင်နှုန်း",
        ["AmountMMK"] = "ပမာဏ (MMK)",
        ["TransactionsCount"] = "စုစုပေါင်း မှတ်တမ်း",
        ["NoDataAvailable"] = "ပြသရန် အချက်အလက် မရှိသေးပါ။",

        // Settings
        ["SettingsTitle"] = "အက်ပ် ဆက်တင်များ",
        ["LanguageSection"] = "ဘာသာစကား (Language)",
        ["LanguageDesc"] = "အက်ပ်တွင် အသုံးပြုလိုသော ဘာသာစကားကို ရွေးချယ်ပါ",
        ["ThemeSection"] = "အပြင်အဆင် မုဒ် (Theme)",
        ["ThemeDesc"] = "အလင်း (Light) နှင့် အမှောင် (Dark) မုဒ်များ ပြောင်းလဲအသုံးပြုနိုင်ပါသည်",
        ["LightMode"] = "အလင်း မုဒ် (Light Mode)",
        ["DarkMode"] = "အမှောင် မုဒ် (Dark Mode)",
        ["CurrencySection"] = "ငွေကြေးစနစ်",
        ["CurrencyDesc"] = "မြန်မာကျပ်ငွေ (MMK) သာ အသုံးပြုထားပါသည်",
        ["DateFormatSection"] = "ရက်စွဲ ပုံစံ",
        ["DateFormatDesc"] = "ရက်-လ-နှစ် (dd-MM-yyyy) သာ အသုံးပြုထားပါသည်",
        ["StorageSection"] = "ဒေတာ သိမ်းဆည်းမှု ပုံစံ",
        ["StorageDesc"] = "မှတ်ဉာဏ်တွင်း သိမ်းဆည်းမှု (In-Memory MVP) စနစ်ဖြစ်ပါသည်",

        // Validation
        ["ValTitleRequired"] = "ခေါင်းစဉ် ထည့်သွင်းရန် လိုအပ်ပါသည်။",
        ["ValAmountRequired"] = "ပမာဏ ထည့်သွင်းရန် လိုအပ်ပါသည်။",
        ["ValAmountPositive"] = "ပမာဏသည် ၀ ထက် ကြီးရပါမည်။",
        ["ValCategoryRequired"] = "အမျိုးအစား ရွေးချယ်ရန် လိုအပ်ပါသည်။",
        ["ValDateRequired"] = "ရက်စွဲ ရွေးချယ်ရန် လိုအပ်ပါသည်။",
        ["ValCategoryNameRequired"] = "အမျိုးအစား အမည် ထည့်သွင်းရန် လိုအပ်ပါသည်။",
        ["ValCategoryExists"] = "ဤအမည်ဖြင့် အမျိုးအစား ရှိနှင့်ပြီးဖြစ်ပါသည်။",
        ["Warning"] = "သတိပေးချက်",
        ["Error"] = "အမှား"
    };

    private readonly Dictionary<string, string> _englishStrings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Navigation
        ["AppTitle"] = "Expense Tracker",
        ["NavDashboard"] = "Dashboard",
        ["NavExpenses"] = "Expenses",
        ["NavIncome"] = "Income",
        ["NavCategories"] = "Categories",
        ["NavReports"] = "Reports",
        ["NavSettings"] = "Settings",

        // Common Actions
        ["Add"] = "Add",
        ["Edit"] = "Edit",
        ["Delete"] = "Delete",
        ["Save"] = "Save",
        ["Cancel"] = "Cancel",
        ["Search"] = "Search",
        ["Clear"] = "Clear",
        ["Close"] = "Close",
        ["Confirm"] = "Confirm",
        ["Actions"] = "Actions",
        ["ViewDetails"] = "View Details",
        ["Back"] = "Back",
        ["Ok"] = "OK",

        // Dashboard
        ["TotalIncome"] = "Total Income",
        ["TotalExpense"] = "Total Expense",
        ["CurrentBalance"] = "Current Balance",
        ["NumberOfExpenses"] = "Expense Count",
        ["RecentExpenses"] = "Recent Expenses",
        ["RecentIncome"] = "Recent Income",
        ["CurrentMonthSummary"] = "Current Month Summary",
        ["ExpenseByCategory"] = "Expense by Category",
        ["MonthlyNetSavings"] = "Net Savings This Month",
        ["MonthlyIncome"] = "Monthly Income",
        ["MonthlyExpense"] = "Monthly Expense",
        ["QuickOverview"] = "Financial Overview",

        // Expenses
        ["ExpensesTitle"] = "Expense List",
        ["AddExpense"] = "Add Expense",
        ["EditExpense"] = "Edit Expense",
        ["DeleteExpense"] = "Delete Expense",
        ["ExpenseDetails"] = "Expense Details",
        ["SearchExpensesPlaceholder"] = "Search by title, category, note...",
        ["NoExpensesFound"] = "No expenses found.",
        ["ConfirmDeleteExpenseTitle"] = "Delete Expense",
        ["ConfirmDeleteExpenseMessage"] = "Are you sure you want to delete this expense? This action cannot be undone.",

        // Income
        ["IncomeTitle"] = "Income List",
        ["AddIncome"] = "Add Income",
        ["EditIncome"] = "Edit Income",
        ["DeleteIncome"] = "Delete Income",
        ["IncomeDetails"] = "Income Details",
        ["SearchIncomePlaceholder"] = "Search by title, note...",
        ["NoIncomeFound"] = "No income found.",
        ["ConfirmDeleteIncomeTitle"] = "Delete Income",
        ["ConfirmDeleteIncomeMessage"] = "Are you sure you want to delete this income? This action cannot be undone.",

        // Fields
        ["Title"] = "Title",
        ["Amount"] = "Amount (MMK)",
        ["Category"] = "Category",
        ["Date"] = "Date (dd-MM-yyyy)",
        ["Note"] = "Note",
        ["EnterTitlePlaceholder"] = "Enter title...",
        ["EnterAmountPlaceholder"] = "Enter amount in MMK...",
        ["SelectCategoryPlaceholder"] = "Select category",
        ["EnterNotePlaceholder"] = "Enter optional note...",

        // Filters & Sorting
        ["FilterByCategory"] = "All Categories",
        ["FromDate"] = "From Date",
        ["ToDate"] = "To Date",
        ["ClearFilters"] = "Clear Filters",
        ["SortBy"] = "Sort by",
        ["SortNewest"] = "Newest first",
        ["SortOldest"] = "Oldest first",
        ["SortHighestAmount"] = "Highest amount",
        ["SortLowestAmount"] = "Lowest amount",

        // Categories
        ["CategoriesTitle"] = "Category Management",
        ["AddCategory"] = "Add Category",
        ["EditCategory"] = "Edit Category",
        ["DeleteCategory"] = "Delete Category",
        ["CategoryName"] = "Category Name",
        ["CategoryColor"] = "Category Color",
        ["CategoryIcon"] = "Icon / Symbol",
        ["NoCategoriesFound"] = "No categories found.",
        ["ConfirmDeleteCategoryTitle"] = "Delete Category",
        ["ConfirmDeleteCategoryMessage"] = "Are you sure you want to delete category '{0}'?",
        ["CategoryInUseError"] = "Cannot delete category '{0}' because it is currently used by {1} expense(s).",
        ["CategoryNamePlaceholder"] = "Enter category name...",

        // Default Categories
        ["Category_Food"] = "Food",
        ["Category_Transport"] = "Transport",
        ["Category_Shopping"] = "Shopping",
        ["Category_Bills"] = "Bills",
        ["Category_Entertainment"] = "Entertainment",
        ["Category_Health"] = "Health",
        ["Category_Education"] = "Education",
        ["Category_Travel"] = "Travel",
        ["Category_Other"] = "Other",

        // Reports
        ["ReportsTitle"] = "Financial Reports",
        ["HighestExpense"] = "Highest Expense",
        ["RecentSpending"] = "Recent Spending",
        ["TotalSpending"] = "Total Expense",
        ["AverageExpense"] = "Average Expense",
        ["CategoryBreakdown"] = "Expense by Category Breakdown",
        ["Percentage"] = "Percentage",
        ["AmountMMK"] = "Amount (MMK)",
        ["TransactionsCount"] = "Total Transactions",
        ["NoDataAvailable"] = "No data available.",

        // Settings
        ["SettingsTitle"] = "Settings",
        ["LanguageSection"] = "Language",
        ["LanguageDesc"] = "Choose application display language",
        ["ThemeSection"] = "Theme Mode",
        ["ThemeDesc"] = "Switch between Light and Dark interface appearance",
        ["LightMode"] = "Light Mode",
        ["DarkMode"] = "Dark Mode",
        ["CurrencySection"] = "Currency",
        ["CurrencyDesc"] = "Strictly MMK (Myanmar Kyat)",
        ["DateFormatSection"] = "Date Format",
        ["DateFormatDesc"] = "Strictly dd-MM-yyyy (e.g., 20-10-2026)",
        ["StorageSection"] = "Data Storage",
        ["StorageDesc"] = "In-Memory Storage (MVP)",

        // Validation
        ["ValTitleRequired"] = "Title is required.",
        ["ValAmountRequired"] = "Amount is required.",
        ["ValAmountPositive"] = "Amount must be greater than 0.",
        ["ValCategoryRequired"] = "Category is required.",
        ["ValDateRequired"] = "Date is required.",
        ["ValCategoryNameRequired"] = "Category name is required.",
        ["ValCategoryExists"] = "A category with this name already exists.",
        ["Warning"] = "Warning",
        ["Error"] = "Error"
    };

    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        var dict = _currentLanguage == AppLanguage.Myanmar ? _myanmarStrings : _englishStrings;
        if (dict.TryGetValue(key, out var val))
            return val;

        // Fallback to English if key missing in Myanmar
        if (_englishStrings.TryGetValue(key, out var fallbackVal))
            return fallbackVal;

        return key;
    }

    public string Format(string key, params object[] args)
    {
        var template = GetString(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    public string GetCategoryDisplayName(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        var key = "Category_" + categoryName.Trim();
        var translated = GetString(key);
        if (translated != key)
            return translated;

        return categoryName;
    }

    public void SetLanguage(AppLanguage language)
    {
        if (_currentLanguage == language)
            return;

        _currentLanguage = language;
        // Raise for everything so all indexers and properties re-evaluate in UI
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
    }
}
