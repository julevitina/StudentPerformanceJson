using StudentPerformanceJson.Models;
using StudentPerformanceJson.Services;
using StudentPerformanceJson.ViewModels;
using StudentPerformanceJson.Views;

namespace StudentPerformanceJson;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    private string? _filePath;

    public MainPage()
    {
        InitializeComponent();

        JsonFileService fileService =
            new JsonFileService(string.Empty);

        _viewModel =
            new MainViewModel(fileService);

        studentsCollection.ItemsSource =
            _viewModel.FilteredStudents;
    }

    private async void OnOpenJsonClicked(
        object sender,
        EventArgs e)
    {
        FileResult? result =
            await FilePicker.Default.PickAsync();

        if (result == null)
            return;

        if (string.IsNullOrWhiteSpace(result.FullPath))
            return;

        try
        {
            _filePath = result.FullPath;

            _viewModel.SetFilePath(_filePath);

            await _viewModel.LoadFileAsync();

            fileLabel.Text =
                $"Selected file: {result.FileName}";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                $"Could not open JSON file.\n{ex.Message}",
                "OK");
        }
    }

    private async void OnSaveJsonClicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            await DisplayAlertAsync(
                "Error",
                "Open a JSON file first.",
                "OK");

            return;
        }

        try
        {
            await _viewModel.SaveFileAsync();

            await DisplayAlertAsync(
                "Success",
                "Data saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                $"Could not save JSON file.\n{ex.Message}",
                "OK");
        }
    }

    private void OnSearchClicked(
        object sender,
        EventArgs e)
    {
        _viewModel.SearchText =
            searchEntry.Text?.Trim() ?? string.Empty;

        _viewModel.SelectedDepartment =
            string.IsNullOrWhiteSpace(departmentEntry.Text)
                ? null
                : departmentEntry.Text.Trim();

        if (int.TryParse(
                courseEntry.Text,
                out int course))
        {
            _viewModel.SelectedCourse = course;
        }
        else
        {
            _viewModel.SelectedCourse = null;
        }

        _viewModel.SelectedSubject =
            string.IsNullOrWhiteSpace(subjectEntry.Text)
                ? null
                : subjectEntry.Text.Trim();

        _viewModel.ApplyFilter();
    }

    private void OnClearClicked(
        object sender,
        EventArgs e)
    {
        searchEntry.Text = string.Empty;
        departmentEntry.Text = string.Empty;
        courseEntry.Text = string.Empty;
        subjectEntry.Text = string.Empty;

        _viewModel.SearchText = string.Empty;
        _viewModel.SelectedDepartment = null;
        _viewModel.SelectedCourse = null;
        _viewModel.SelectedSubject = null;

        _viewModel.ApplyFilter();
    }

    private async void OnAddClicked(
        object sender,
        EventArgs e)
    {
        EditStudentPage page =
            new EditStudentPage(
                null,
                false,
                student =>
                {
                    student.Id = GetNextStudentId();

                    _viewModel.AddStudent(student);
                });

        await Navigation.PushAsync(page);
    }

    private async void OnEditClicked(
        object sender,
        EventArgs e)
    {
        if (studentsCollection.SelectedItem
            is not Student selectedStudent)
        {
            await DisplayAlertAsync(
                "Error",
                "Select a student first.",
                "OK");

            return;
        }

        EditStudentPage page =
            new EditStudentPage(
                selectedStudent,
                true,
                student =>
                {
                    _viewModel.EditStudent(student);
                });

        await Navigation.PushAsync(page);
    }

    private async void OnDeleteClicked(
        object sender,
        EventArgs e)
    {
        if (studentsCollection.SelectedItem
            is not Student selectedStudent)
        {
            await DisplayAlertAsync(
                "Error",
                "Select a student first.",
                "OK");

            return;
        }

        bool answer =
            await DisplayAlertAsync(
                "Delete student",
                $"Delete {selectedStudent.FullName}?",
                "Yes",
                "No");

        if (!answer)
            return;

        _viewModel.DeleteStudent(selectedStudent);

        studentsCollection.SelectedItem = null;
    }

    private async void OnAboutClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new AboutPage());
    }

    private int GetNextStudentId()
    {
        if (_viewModel.Students.Count == 0)
            return 1;

        return _viewModel.Students.Max(
                   student => student.Id)
               + 1;
    }
}