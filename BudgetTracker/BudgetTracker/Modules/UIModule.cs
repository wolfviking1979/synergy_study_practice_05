using BudgetTracker.Models;

namespace BudgetTracker.Modules;

public static class UiModule
{
    public static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine("║     БЮДЖЕТНЫЙ ТРЕКЕР             ║");
        Console.WriteLine("╠═══════════════════════════════════╣");
        Console.WriteLine("║  1. Добавить запись              ║");
        Console.WriteLine("║  2. Показать все записи          ║");
        Console.WriteLine("║  3. Показать отчет               ║");
        Console.WriteLine("║  4. Выход                        ║");
        Console.WriteLine("╚═══════════════════════════════════╝");
        Console.Write("Выберите действие: ");
    }

    public static FinanceRecord ReadRecordFromUser()
    {
        Console.Clear();
        Console.WriteLine("╔═══════════════════════════════════╗");
        Console.WriteLine("║     ДОБАВЛЕНИЕ ЗАПИСИ            ║");
        Console.WriteLine("╚═══════════════════════════════════╝");
        Console.WriteLine();

        var record = new FinanceRecord();
        record.Id = Guid.NewGuid();
        record.Date = DateTime.Now;

        // ===== ВЫБОР ТИПА (ЦИФРЫ) =====
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

        // ===== ВВОД КАТЕГОРИИ =====
        Console.Write("Категория (например: Еда, Транспорт, Зарплата): ");
        var category = Console.ReadLine()?.Trim();
        record.Category = string.IsNullOrWhiteSpace(category) ? "Прочее" : category;
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
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     ВСЕ ЗАПИСИ                                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        if (!records.Any())
        {
            Console.WriteLine("📭 Записей пока нет. Добавьте первую запись!");
        }
        else
        {
            Console.WriteLine("  Дата    │  Тип    │   Сумма   │   Категория   │  Описание");
            Console.WriteLine("──────────┼─────────┼───────────┼───────────────┼──────────────────");
            
            foreach (var record in records.OrderByDescending(r => r.Date))
            {
                string typeIcon = record.Type == "Income" ? "💰 Доход" : "💸 Расход";
                Console.WriteLine($"{record.Date:dd.MM.yyyy} │ {typeIcon,-8} │ {record.Amount,9:C} │ {record.Category,-13} │ {record.Description}");
            }
            
            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────────");
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
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           ФИНАНСОВЫЙ ОТЧЕТ (за 30 дней)                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        // Цветной вывод баланса
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
        Console.WriteLine("  ────────────────────────────────────────");
        Console.WriteLine();

        if (categories.Any())
        {
            Console.WriteLine("  📂 Расходы по категориям:");
            Console.WriteLine("  ────────────────────────────────────────");
            foreach (var cat in categories.OrderByDescending(c => c.Value))
            {
                decimal percent = expense > 0 ? (cat.Value / expense) * 100 : 0;
                
                // Простая визуализация прогресс-бара
                int barLength = (int)(percent / 5); // Максимум 20 символов
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
}