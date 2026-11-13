using System.Collections.ObjectModel;
using StudentPerformanceJson.Models;

namespace StudentPerformanceJson.Views;

public partial class EditStudentPage : ContentPage
{
    private readonly Student? _student;
    private readonly bool _isEdit;
    private readonly Action<Student> _saveAction;

    private readonly ObservableCollection<Subject> _subjects = new();

    public EditStudentPage(
        Student? student,
        bool isEdit,
        Action<Student> saveAction)
    {
        InitializeComponent();

        _student = student;
        _isEdit = isEdit;
        _saveAction = saveAction;

        subjectsCollection.ItemsSource = _subjects;

        if (_isEdit && _student != null)
        {
            titleLabel.Text = "Edit Student";

            fullNameEntry.Text = _student.FullName;
            facultyEntry.Text = _student.Faculty;
            departmentEntry.Text = _student.Department;
            courseEntry.Text = _student.Course.ToString();
            groupEntry.Text = _student.Group;

            foreach (Subject subject in _student.Subjects)
            {
                _subjects.Add(new Subject
                {
                    Name = subject.Name,
                    Grade = subject.Grade
                });
            }
        }
        else
        {
            titleLabel.Text = "Add Student";
        }
    }

    private async void OnAddSubjectClicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(subjectNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Error",
                "Enter subject name.",
                "OK");

            return;
        }

        if (!int.TryParse(gradeEntry.Text, out int grade) ||
            grade < 0 ||
            grade > 100)
        {
            await DisplayAlertAsync(
                "Error",
                "Grade must be from 0 to 100.",
                "OK");

            return;
        }

        _subjects.Add(new Subject
        {
            Name = subjectNameEntry.Text.Trim(),
            Grade = grade
        });

        subjectNameEntry.Text = string.Empty;
        gradeEntry.Text = string.Empty;
    }

    private async void OnSaveClicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(fullNameEntry.Text) ||
            string.IsNullOrWhiteSpace(facultyEntry.Text))
        {
            await DisplayAlertAsync(
                "Error",
                "Full name and faculty are required.",
                "OK");

            return;
        }

        if (!int.TryParse(courseEntry.Text, out int course) ||
            course <= 0)
        {
            await DisplayAlertAsync(
                "Error",
                "Enter a correct course.",
                "OK");

            return;
        }

        Student resultStudent = new Student
        {
            Id = _isEdit && _student != null
                ? _student.Id
                : 0,

            FullName = fullNameEntry.Text.Trim(),
            Faculty = facultyEntry.Text.Trim(),
            Department = departmentEntry.Text?.Trim(),
            Course = course,
            Group = groupEntry.Text?.Trim(),
            Subjects = _subjects.ToList()
        };

        _saveAction(resultStudent);

        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}