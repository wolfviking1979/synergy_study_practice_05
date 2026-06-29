namespace BudgetTracker.Models;

public class FinanceRecord
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "Expense"; // "Income" или "Expense"
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Date:dd.MM.yyyy} | {Type,-7} | {Amount,10:C} | {Category,-15} | {Description}";
    }
}