using System;
using System.Collections.Generic;
using System.Linq;
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
            .Where(r => !string.IsNullOrWhiteSpace(r.Category))
            .GroupBy(r => r.Category)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Amount));
    }

    // Дополнительная статистика
    public static (decimal avgIncome, decimal avgExpense, decimal maxIncome, decimal maxExpense) GetStatistics(List<FinanceRecord> records)
    {
        var incomes = records.Where(r => r.Type == "Income").Select(r => r.Amount).ToList();
        var expenses = records.Where(r => r.Type == "Expense").Select(r => r.Amount).ToList();

        var avgIncome = incomes.Any() ? incomes.Average() : 0;
        var avgExpense = expenses.Any() ? expenses.Average() : 0;
        var maxIncome = incomes.Any() ? incomes.Max() : 0;
        var maxExpense = expenses.Any() ? expenses.Max() : 0;

        return (avgIncome, avgExpense, maxIncome, maxExpense);
    }

    public static List<FinanceRecord> FilterByCategory(List<FinanceRecord> records, string category)
    {
        return records
            .Where(r => r.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<FinanceRecord> FilterByType(List<FinanceRecord> records, string type)
    {
        return records.Where(r => r.Type == type).ToList();
    }
}