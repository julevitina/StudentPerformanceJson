namespace StudentPerformanceJson.Models;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string? Department { get; set; }
    public int Course { get; set; }
    public string? Group { get; set; }
    public List<Subject> Subjects { get; set; } = new();

    public double GetAverageGrade()
    {
        if (Subjects.Count == 0)
            return 0;

        return Subjects.Average(subject => subject.Grade);
    }

    public bool HasSubject(string name)
    {
        return Subjects.Any(subject =>
            subject.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));
    }
}