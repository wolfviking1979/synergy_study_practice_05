using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BudgetTracker.Models;

namespace BudgetTracker.Modules;

public static class UiModule
{
    private static List<string> _categories = new();

    public static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║             БЮДЖЕТНЫЙ ТРЕКЕР                   ║");
        Console.WriteLine("╠════════════════════════════════════════════════╣");
        Console.WriteLine("║  1.  Добавить запись                           ║");
        Console.WriteLine("║  2.  Показать все записи                       ║");
        Console.WriteLine("║  3.  Показать отчет                            ║");
        Console.WriteLine("║  4.  Показать статистику                       ║");
        Console.WriteLine("║  5.  Удалить запись                            ║");
        Console.WriteLine("║  6.  Редактировать запись                      ║");
        Console.WriteLine("║  7.  Поиск по категории                        ║");
        Console.WriteLine("║  8.  Фильтр по типу                            ║");
        Console.WriteLine("║  9.  Экспорт в CSV                             ║");
        Console.WriteLine("║  10. Показать путь к файлу                     ║");
        Console.WriteLine("║  0.  Выход                                     ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
        Console.Write("Выберите действие: ");
    }

    public static FinanceRecord ReadRecordFromUser()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine("║     ДОБАВЛЕНИЕ ЗАПИСИ             ║");
        Console.WriteLine("╚═══════════════════════════════════╝");
        Console.WriteLine();

        var record = new FinanceRecord();
        record.Id = Guid.NewGuid();

        // ===== ВЫБОР ДАТЫ =====
        Console.WriteLine("Выберите дату:");
        Console.WriteLine("  1. Сегодня");
        Console.WriteLine("  2. Вчера");
        Console.WriteLine("  3. Указать вручную");
        Console.Write("Ваш выбор: ");
        var dateChoice = Console.ReadLine()?.Trim();

        switch (dateChoice)
        {
            case "1":
                record.Date = DateTime.Now;
                break;
            case "2":
                record.Date = DateTime.Now.AddDays(-1);
                break;
            case "3":
                while (true)
                {
                    Console.Write("Введите дату (дд.ММ.гггг): ");
                    if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime customDate))
                    {
                        record.Date = customDate;
                        break;
                    }
                    Console.WriteLine("❌ Неверный формат! Используйте дд.ММ.гггг");
                }
                break;
            default:
                record.Date = DateTime.Now;
                break;
        }
        Console.WriteLine($"✓ Дата: {record.Date:dd.MM.yyyy}");
        Console.WriteLine();

        // ===== ВЫБОР ТИПА =====
        Console.WriteLine("Выберите тип операции:");
        Console.WriteLine("  1. Доход  (поступление денег)");
        Console.WriteLine("  2. Расход (трата денег)");
        Console.Write("Ваш выбор (1 или 2): ");
        
        var typeChoice = Console.ReadLine()?.Trim();
        while (typeChoice != "1" && typeChoice != "2")
        {
            Console.Write("❌ Ошибка! Введите 1 (Доход) или 2 (Расход): ");
            typeChoice = Console.ReadLine()?.Trim();
        }
        
        record.Type = typeChoice == "1" ? "Income" : "Expense";
        string typeDisplay = record.Type == "Income" ? "ДОХОД" : "РАСХОД";
        Console.WriteLine($"✓ Тип: {typeDisplay}");
        Console.WriteLine();

        // ===== ВВОД СУММЫ =====
        while (true)
        {
            Console.Write("Сумма (в рублях): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                record.Amount = amount;
                break;
            }
            Console.WriteLine("❌ Ошибка: введите положительное число (например: 1500.50)");
        }
        Console.WriteLine($"✓ Сумма: {record.Amount:C}");
        Console.WriteLine();

        // ===== ВВОД КАТЕГОРИИ (с автодополнением) =====
        Console.WriteLine("Доступные категории:");
        var distinctCategories = _categories.Distinct().OrderBy(c => c).ToList();
        if (distinctCategories.Any())
        {
            foreach (var cat in distinctCategories)
            {
                Console.WriteLine($"  - {cat}");
            }
        }
        else
        {
            Console.WriteLine("  (нет сохраненных категорий)");
        }
        
        Console.Write("Категория (или новая): ");
        var category = Console.ReadLine()?.Trim();
        record.Category = string.IsNullOrWhiteSpace(category) ? "Прочее" : category;
        if (!_categories.Contains(record.Category))
            _categories.Add(record.Category);
        Console.WriteLine($"✓ Категория: {record.Category}");
        Console.WriteLine();

        // ===== ВВОД ОПИСАНИЯ =====
        Console.Write("Описание (необязательно, Enter для пропуска): ");
        record.Description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(record.Description))
            Console.WriteLine($"✓ Описание: {record.Description}");
        else
            Console.WriteLine("✓ Описание пропущено");
        
        Console.WriteLine();
        Console.WriteLine("✅ Запись создана успешно!");
        Console.WriteLine("Нажмите Enter для сохранения...");
        Console.ReadLine();
        
        return record;
    }

    public static void DisplayRecords(List<FinanceRecord> records)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                            ВСЕ ЗАПИСИ                                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        if (!records.Any())
        {
            Console.WriteLine("📭 Записей пока нет. Добавьте первую запись!");
        }
        else
        {
            Console.WriteLine("  № │  Дата    │  Тип    │   Сумма   │   Категория   │  Описание        ");
            Console.WriteLine("────┼──────────┼─────────┼───────────┼───────────────┼──────────────────");
            
            int index = 1;
            foreach (var record in records.OrderByDescending(r => r.Date))
            {
                string typeIcon = record.Type == "Income" ? "💰 Доход" : "💸 Расход";
                Console.WriteLine($"{index,3} │ {record.Date:dd.MM.yyyy} │ {typeIcon,-8} │ {record.Amount,9:C} │ {record.Category,-13} │ {record.Description}");
                index++;
            }
            
            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
            var totalIncome = records.Where(r => r.Type == "Income").Sum(r => r.Amount);
            var totalExpense = records.Where(r => r.Type == "Expense").Sum(r => r.Amount);
            Console.WriteLine($"💰 Доходы:  {totalIncome,10:C}");
            Console.WriteLine($"💸 Расходы: {totalExpense,10:C}");
            Console.WriteLine($"📈 Баланс:  {(totalIncome - totalExpense),10:C}");
            Console.WriteLine($"📊 Всего записей: {records.Count}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    public static void DisplayReport(decimal balance, decimal income, decimal expense, Dictionary<string, decimal> categories)
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                ФИНАНСОВЫЙ ОТЧЕТ (за 30 дней)                          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        if (balance >= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  📊 БАЛАНС:           {balance,15:C}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  📊 БАЛАНС:           {balance,15:C}");
            Console.ResetColor();
        }
        
        Console.WriteLine($"  💰 Доходы:           {income,15:C}");
        Console.WriteLine($"  💸 Расходы:          {expense,15:C}");
        Console.WriteLine("  ───────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        if (categories.Any())
        {
            Console.WriteLine("  📂 Расходы по категориям:");
            Console.WriteLine("  ───────────────────────────────────────────────────────────────────");
            foreach (var cat in categories.OrderByDescending(c => c.Value))
            {
                decimal percent = expense > 0 ? (cat.Value / expense) * 100 : 0;
                int barLength = (int)(percent / 5);
                string bar = new string('█', Math.Min(barLength, 20));
                
                Console.WriteLine($"    {cat.Key,-18} {cat.Value,12:C}  {bar} {percent,5:F1}%");
            }
        }
        else
        {
            if (expense == 0 && income == 0)
                Console.WriteLine("  📭 Нет данных за последние 30 дней.");
            else if (expense == 0)
                Console.WriteLine("  ✅ Расходов за последние 30 дней нет!");
            else
                Console.WriteLine("  📭 Нет данных о расходах по категориям.");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    public static void DisplayStatistics(List<FinanceRecord> records)
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     СТАТИСТИКА                            ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var totalRecords = records.Count;
        if (totalRecords == 0)
        {
            Console.WriteLine("📭 Нет данных для статистики.");
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
            return;
        }

        var stats = BusinessLogicModule.GetStatistics(records);
        var totalIncome = records.Where(r => r.Type == "Income").Sum(r => r.Amount);
        var totalExpense = records.Where(r => r.Type == "Expense").Sum(r => r.Amount);

        Console.WriteLine($"📊 Всего записей:    {totalRecords}");
        Console.WriteLine($"💰 Средний доход:    {stats.avgIncome,12:C}");
        Console.WriteLine($"💸 Средний расход:   {stats.avgExpense,12:C}");
        Console.WriteLine($"📈 Максимальный доход: {stats.maxIncome,10:C}");
        Console.WriteLine($"📉 Максимальный расход: {stats.maxExpense,10:C}");
        Console.WriteLine($"💰 Общий доход:      {totalIncome,12:C}");
        Console.WriteLine($"💸 Общий расход:     {totalExpense,12:C}");
        Console.WriteLine($"📊 Баланс:           {(totalIncome - totalExpense),12:C}");

        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    public static int SelectRecordForAction(List<FinanceRecord> records, string action)
    {
        DisplayRecords(records);
        Console.Write($"\nВведите номер записи для {action} (или 0 для отмены): ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index <= records.Count)
        {
            return index;
        }
        return -1;
    }

    public static void ShowFilePath(string filePath)
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   ПУТЬ К ФАЙЛУ                            ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"📁 Папка: {Path.GetDirectoryName(filePath)}");
        Console.WriteLine($"📄 Файл:  {filePath}");
        Console.WriteLine();
        Console.WriteLine("Вы можете открыть эту папку и проверить файл вручную.");
        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    public static void ExportToCsv(List<FinanceRecord> records)
    {
        if (!records.Any())
        {
            Console.WriteLine("❌ Нет данных для экспорта!");
            Console.ReadKey();
            return;
        }

        var csvPath = $"budget_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var lines = new List<string> { "Дата,Тип,Сумма,Категория,Описание" };
        lines.AddRange(records.Select(r => 
            $"{r.Date:dd.MM.yyyy},{r.Type},{r.Amount.ToString("F2")},{r.Category},{r.Description}"));
        File.WriteAllLines(csvPath, lines);
        
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   ЭКСПОРТ В CSV                           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"✅ Экспортировано записей: {records.Count}");
        Console.WriteLine($"📄 Файл: {Path.GetFullPath(csvPath)}");
        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }
}