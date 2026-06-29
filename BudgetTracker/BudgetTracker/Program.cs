using BudgetTracker.Models;
using BudgetTracker.Modules;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== БЮДЖЕТНЫЙ ТРЕКЕР ===");
Console.WriteLine("Запуск программы...");

var storage = new StorageModule();
var records = storage.Load();

bool isRunning = true;
while (isRunning)
{
    try
    {
        UiModule.ShowMenu();
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                var newRecord = UiModule.ReadRecordFromUser();
                records.Add(newRecord);
                storage.Save(records);
                Console.WriteLine("✓ Запись успешно добавлена!");
                Console.ReadKey();
                break;
                
            case "2":
                UiModule.DisplayRecords(records);
                break;
                
            case "3":
                records = storage.Load();
                var balance = BusinessLogicModule.GetBalance(records);
                var income = BusinessLogicModule.GetTotalByType(records, "Income");
                var expense = BusinessLogicModule.GetTotalByType(records, "Expense");
                var categories = BusinessLogicModule.GetExpensesByCategory(records);
                UiModule.DisplayReport(balance, income, expense, categories);
                break;
                
            case "4":
                storage.Save(records);
                Console.WriteLine("Данные сохранены. До свидания!");
                isRunning = false;
                break;
                
            default:
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                Console.ReadKey();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}