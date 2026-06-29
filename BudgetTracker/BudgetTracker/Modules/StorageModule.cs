using System.Text.Json;
using BudgetTracker.Models;

namespace BudgetTracker.Modules;

public class StorageModule
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public StorageModule(string fileName = "budget_data.json")
    {
        // Определяем корень проекта
        string projectRoot = GetProjectRoot();
        
        // Полный путь к файлу в корне проекта
        _filePath = Path.Combine(projectRoot, fileName);
        
        Console.WriteLine($"[INFO] Корень проекта: {projectRoot}");
        Console.WriteLine($"[INFO] Файл данных: {_filePath}");
    }

    /// <summary>
    /// Определяет корневую папку проекта
    /// </summary>
    private string GetProjectRoot()
    {
        // Начинаем с текущей директории
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        
        // Ищем файл .csproj, поднимаясь вверх по папкам
        string? directory = currentDir;
        while (directory != null)
        {
            // Проверяем, есть ли в этой папке файл .csproj
            var csprojFiles = Directory.GetFiles(directory, "*.csproj");
            if (csprojFiles.Length > 0)
            {
                // Нашли корень проекта
                return directory;
            }
            
            // Поднимаемся на уровень выше
            directory = Directory.GetParent(directory)?.FullName;
        }
        
        // Если .csproj не найден, используем текущую папку
        return currentDir;
    }

    public List<FinanceRecord> Load()
    {
        if (!File.Exists(_filePath))
        {
            Console.WriteLine($"[INFO] Файл не найден. Будет создан новый: {_filePath}");
            return new List<FinanceRecord>();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var records = JsonSerializer.Deserialize<List<FinanceRecord>>(json) ?? new List<FinanceRecord>();
            Console.WriteLine($"[INFO] Загружено {records.Count} записей из файла");
            return records;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ERROR] Ошибка чтения JSON: {ex.Message}");
            return new List<FinanceRecord>();
        }
    }

    public void Save(List<FinanceRecord> records)
    {
        try
        {
            var json = JsonSerializer.Serialize(records, _options);
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"[INFO] Сохранено {records.Count} записей");
            Console.WriteLine($"[INFO] Путь: {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Ошибка сохранения: {ex.Message}");
        }
    }
    
    // Вспомогательный метод для получения пути к файлу
    public string GetFilePath() => _filePath;
}