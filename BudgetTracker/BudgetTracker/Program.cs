using System;
using System.IO;
using BudgetTracker.Models;
using BudgetTracker.Modules;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("╔═══════════════════════════════════════╗");
Console.WriteLine("║     БЮДЖЕТНЫЙ ТРЕКЕР                  ║");
Console.WriteLine("║     Запуск программы...               ║");
Console.WriteLine("╚═══════════════════════════════════════╝");
Console.WriteLine();

var storage = new StorageModule();
var records = storage.Load();

bool isRunning = true;
while (isRunning)
{
    try
    {
        UiModule.ShowMenu();
        var choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1": // Добавить запись
                var newRecord = UiModule.ReadRecordFromUser();
                records.Add(newRecord);
                storage.Save(records);
                Console.WriteLine("✓ Запись успешно добавлена!");
                Console.ReadKey();
                break;
                
            case "2": // Показать все записи
                UiModule.DisplayRecords(records);
                break;
                
            case "3": // Показать отчет
                var balance = BusinessLogicModule.GetBalance(records);
                var income = BusinessLogicModule.GetTotalByType(records, "Income");
                var expense = BusinessLogicModule.GetTotalByType(records, "Expense");
                var categories = BusinessLogicModule.GetExpensesByCategory(records);
                UiModule.DisplayReport(balance, income, expense, categories);
                break;
                
            case "4": // Показать статистику
                UiModule.DisplayStatistics(records);
                break;
                
            case "5": // Удалить запись
                if (!records.Any())
                {
                    Console.WriteLine("❌ Нет записей для удаления!");
                    Console.ReadKey();
                    break;
                }
                int deleteIndex = UiModule.SelectRecordForAction(records, "удаления");
                if (deleteIndex > 0)
                {
                    var removed = records[deleteIndex - 1];
                    records.RemoveAt(deleteIndex - 1);
                    storage.Save(records);
                    Console.WriteLine($"✅ Удалена запись: {removed.Description} ({removed.Amount:C})");
                    Console.ReadKey();
                }
                else if (deleteIndex == 0)
                {
                    Console.WriteLine("ℹ️ Операция отменена");
                    Console.ReadKey();
                }
                break;
                
            case "6": // Редактировать запись
                if (!records.Any())
                {
                    Console.WriteLine("❌ Нет записей для редактирования!");
                    Console.ReadKey();
                    break;
                }
                int editIndex = UiModule.SelectRecordForAction(records, "редактирования");
                if (editIndex > 0)
                {
                    var oldRecord = records[editIndex - 1];
                    Console.WriteLine($"\n📝 Редактируем запись: {oldRecord.Description} ({oldRecord.Amount:C})");
                    Console.WriteLine("Введите новые данные (или нажмите Enter чтобы пропустить поле)\n");
                    
                    var editedRecord = UiModule.ReadRecordFromUser();
                    records[editIndex - 1] = editedRecord;
                    storage.Save(records);
                    Console.WriteLine("✅ Запись обновлена!");
                    Console.ReadKey();
                }
                else if (editIndex == 0)
                {
                    Console.WriteLine("ℹ️ Операция отменена");
                    Console.ReadKey();
                }
                break;
                
            case "7": // Поиск по категории
                Console.Write("\n🔍 Введите категорию для поиска: ");
                var searchTerm = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var found = BusinessLogicModule.FilterByCategory(records, searchTerm);
                    UiModule.DisplayRecords(found);
                    Console.WriteLine($"\n🔍 Найдено записей: {found.Count}");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("❌ Категория не указана");
                    Console.ReadKey();
                }
                break;
                
            case "8": // Фильтр по типу
                if (!records.Any())
                {
                    Console.WriteLine("❌ Нет записей для фильтрации!");
                    Console.ReadKey();
                    break;
                }
                Console.WriteLine("\nПоказать:");
                Console.WriteLine("  1. Только доходы");
                Console.WriteLine("  2. Только расходы");
                Console.Write("Выбор: ");
                var filterChoice = Console.ReadLine()?.Trim();
                var filtered = filterChoice == "1" 
                    ? BusinessLogicModule.FilterByType(records, "Income")
                    : filterChoice == "2" 
                        ? BusinessLogicModule.FilterByType(records, "Expense")
                        : records;
                UiModule.DisplayRecords(filtered);
                break;
                
            case "9": // Экспорт в CSV
                UiModule.ExportToCsv(records);
                break;
                
            case "10": // Показать путь к файлу
                UiModule.ShowFilePath(storage.GetFilePath());
                break;
                
            case "0": // Выход
                storage.Save(records);
                Console.WriteLine("💾 Данные сохранены. До свидания!");
                isRunning = false;
                break;
                
            default:
                Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
                Console.ReadKey();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка: {ex.Message}");
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}