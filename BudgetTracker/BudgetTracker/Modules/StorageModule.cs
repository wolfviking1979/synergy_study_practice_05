using System;
using System.Collections.Generic;
using System.IO;
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
        _filePath = Path.Combine(projectRoot, fileName);
        
        Console.WriteLine($"[INFO] Корень проекта: {projectRoot}");
        Console.WriteLine($"[INFO] Файл данных: {_filePath}");
    }

    private string GetProjectRoot()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string? directory = currentDir;
        
        while (directory != null)
        {
            try
            {
                var csprojFiles = Directory.GetFiles(directory, "*.csproj");
                if (csprojFiles.Length > 0)
                {
                    return directory;
                }
                directory = Directory.GetParent(directory)?.FullName;
            }
            catch
            {
                directory = Directory.GetParent(directory)?.FullName;
            }
        }
        
        return currentDir;
    }

    public List<FinanceRecord> Load()
    {
        if (!File.Exists(_filePath))
        {
            Console.WriteLine($"[INFO] Файл не найден. Будет создан новый");
            return new List<FinanceRecord>();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var records = JsonSerializer.Deserialize<List<FinanceRecord>>(json) ?? new List<FinanceRecord>();
            Console.WriteLine($"[INFO] Загружено {records.Count} записей");
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

    public string GetFilePath() => _filePath;
}