using BudgetTracker.Models;

namespace BudgetTracker.Modules;

public static class BusinessLogicModule
{
    public static decimal GetBalance(List<FinanceRecord> records)
    {
        var totalIncome = records.Where(r => r.Type == "Income").Sum(r => r.Amount);
        var totalExpense = records.Where(r => r.Type == "Expense").Sum(r => r.Amount);
        return totalIncome - totalExpense;
    }

    public static decimal GetTotalByType(List<FinanceRecord> records, string type, int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        return records
            .Where(r => r.Type == type && r.Date >= cutoffDate)
            .Sum(r => r.Amount);
    }

    public static Dictionary<string, decimal> GetExpensesByCategory(List<FinanceRecord> records, int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        return records
            .Where(r => r.Type == "Expense" && r.Date >= cutoffDate)
            .Where(r => !string.IsNullOrWhiteSpace(r.Category)) // Защита от пустых категорий
            .GroupBy(r => r.Category)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Amount));
    }
}