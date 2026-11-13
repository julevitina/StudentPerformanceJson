using System.Text.Json;
using StudentPerformanceJson.Models;

namespace StudentPerformanceJson.Services;

public class JsonFileService
{
    private string _jsonPath;

    private readonly JsonSerializerOptions _options =
        new JsonSerializerOptions
        {
            WriteIndented = true
        };

    public JsonFileService(string jsonPath)
    {
        _jsonPath = jsonPath;
    }

    public void SetFilePath(string jsonPath)
    {
        _jsonPath = jsonPath;
    }

    public async Task<List<Student>> LoadAsync()
    {
        if (!File.Exists(_jsonPath))
            return new List<Student>();

        string json = await File.ReadAllTextAsync(_jsonPath);

        return Deserialize(json);
    }

    public async Task SaveAsync(List<Student> students)
    {
        string json = Serialize(students);

        await File.WriteAllTextAsync(_jsonPath, json);
    }

    public string Serialize(List<Student> students)
    {
        return JsonSerializer.Serialize(students, _options);
    }

    public List<Student> Deserialize(string json)
    {
        return JsonSerializer.Deserialize<List<Student>>(
                   json,
                   _options)
               ?? new List<Student>();
    }
}