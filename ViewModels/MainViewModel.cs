using System.Collections.ObjectModel;
using StudentPerformanceJson.Models;
using StudentPerformanceJson.Services;

namespace StudentPerformanceJson.ViewModels;

public class MainViewModel
{
    private readonly JsonFileService _fileService;

    public ObservableCollection<Student> Students { get; } = new();

    public ObservableCollection<Student> FilteredStudents { get; } = new();

    public string SearchText { get; set; } = string.Empty;

    public string? SelectedDepartment { get; set; }

    public int? SelectedCourse { get; set; }

    public string? SelectedSubject { get; set; }

    public MainViewModel(JsonFileService fileService)
    {
        _fileService = fileService;
    }

    public void SetFilePath(string filePath)
    {
        _fileService.SetFilePath(filePath);
    }

    public async Task LoadFileAsync()
    {
        List<Student> students =
            await _fileService.LoadAsync();

        Students.Clear();

        foreach (Student student in students)
        {
            Students.Add(student);
        }

        ApplyFilter();
    }

    public async Task SaveFileAsync()
    {
        await _fileService.SaveAsync(
            Students.ToList());
    }

    public void AddStudent(Student student)
    {
        Students.Add(student);

        ApplyFilter();
    }

    public void EditStudent(Student student)
    {
        Student? existingStudent =
            Students.FirstOrDefault(
                currentStudent =>
                    currentStudent.Id == student.Id);

        if (existingStudent == null)
            return;

        existingStudent.FullName = student.FullName;
        existingStudent.Faculty = student.Faculty;
        existingStudent.Department = student.Department;
        existingStudent.Course = student.Course;
        existingStudent.Group = student.Group;
        existingStudent.Subjects = student.Subjects;

        ApplyFilter();
    }

    public void DeleteStudent(Student student)
    {
        Students.Remove(student);

        ApplyFilter();
    }

    public void ApplyFilter()
    {
        IEnumerable<Student> query = Students;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(student =>
                student.FullName.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(SelectedDepartment))
        {
            query = query.Where(student =>
                string.Equals(
                    student.Department,
                    SelectedDepartment,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedCourse.HasValue)
        {
            query = query.Where(student =>
                student.Course == SelectedCourse.Value);
        }

        if (!string.IsNullOrWhiteSpace(SelectedSubject))
        {
            query = query.Where(student =>
                student.HasSubject(SelectedSubject));
        }

        FilteredStudents.Clear();

        foreach (Student student in query)
        {
            FilteredStudents.Add(student);
        }
    }
}