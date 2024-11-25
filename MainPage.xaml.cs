using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Text;

namespace MauiApp3
{
    public partial class MainPage : ContentPage
    {
        private List<StudentInfo> _students = new();
        private StudentInfo _selectedStudent;

        public MainPage()
        {
            InitializeComponent();
        }

        // Завантаження JSON
        private async void OnLoadJsonClicked(object sender, EventArgs e)
        {
            try
            {
                // Вибір JSON-файлу
                var jsonFile = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a JSON file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } }
            })
                });

                if (jsonFile == null)
                {
                    FilePathLabel.Text = "No file selected";
                    return;
                }

                FilePathLabel.Text = $"Loaded File: {jsonFile.FullPath}";
                var jsonData = await File.ReadAllTextAsync(jsonFile.FullPath);

                var root = JsonSerializer.Deserialize<Root>(jsonData);

                // Діагностика JSON
                if (root?.University == null)
                {
                    Console.WriteLine("University is null.");
                    FilePathLabel.Text = "Invalid JSON structure.";
                    return;
                }

                foreach (var department in root.University.Departments)
                {
                    Console.WriteLine($"Department: {department.Name}");
                    foreach (var discipline in department.Disciplines)
                    {
                        Console.WriteLine($" Discipline: {discipline.Name}");
                        foreach (var student in discipline.Students)
                        {
                            Console.WriteLine($"  Student: {student.Name}, Grade: {student.Grade}");
                        }
                    }
                }

                _students = LINQ.FlattenData(root.University);
                DisplayData(_students);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Збереження JSON
        private async void OnSaveJsonClicked(object sender, EventArgs e)
        {
            try
            {
                var filePath = @"C:\Users\User\source\repos\MauiApp3\SaveStudents.json";
                var root = new Root
                {
                    University = new University
                    {
                        Departments = LINQ.GroupData(_students)
                    }
                };

                var jsonData = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
                
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(jsonData);
                }

                await DisplayAlert("Success", $"Data saved to {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        // Відображення даних у таблиці
        private void DisplayData(IEnumerable<StudentInfo> students)
        {
            DataGrid.Children.Clear();
            DataGrid.RowDefinitions.Clear();

            // Додавання заголовків
            DataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AddToGrid(new Label { Text = "Name", FontAttributes = FontAttributes.Bold }, 0, 0);
            AddToGrid(new Label { Text = "Discipline", FontAttributes = FontAttributes.Bold }, 1, 0);
            AddToGrid(new Label { Text = "Department", FontAttributes = FontAttributes.Bold }, 2, 0);
            AddToGrid(new Label { Text = "Grade", FontAttributes = FontAttributes.Bold }, 3, 0);

            int row = 1;
            foreach (var student in students)
            {
                DataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var backgroundColor = row % 2 == 0 ? Colors.Plum : Colors.Pink;

                AddToGrid(CreateSelectableLabel(student.Name, student, backgroundColor), 0, row);
                AddToGrid(CreateSelectableLabel(student.Discipline, student, backgroundColor), 1, row);
                AddToGrid(CreateSelectableLabel(student.Department, student, backgroundColor), 2, row);
                AddToGrid(CreateSelectableLabel(student.Grade.ToString(), student, backgroundColor), 3, row);

                row++;
            }
        }

        private Label CreateSelectableLabel(string text, StudentInfo student, Color backgroundColor)
        {
            var label = new Label
            {
                Text = text,
                BackgroundColor = backgroundColor,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            label.BindingContext = student;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnStudentSelected;
            label.GestureRecognizers.Add(tapGesture);

            return label;
        }

        private void AddToGrid(View view, int column, int row)
        {
            Grid.SetColumn(view, column);
            Grid.SetRow(view, row);
            DataGrid.Children.Add(view);
        }
        private async Task<StudentInfo> ShowStudentForm(StudentInfo student)
        {
            string name = await DisplayPromptAsync("Student Info", "Enter Name:", initialValue: student.Name);
            if (string.IsNullOrWhiteSpace(name)) return null;

            string discipline = await DisplayPromptAsync("Student Info", "Enter Discipline:", initialValue: student.Discipline);
            if (string.IsNullOrWhiteSpace(discipline)) return null;

            string department = await DisplayPromptAsync("Student Info", "Enter Department:", initialValue: student.Department);
            if (string.IsNullOrWhiteSpace(department)) return null;

            int grade;
            while (true)
            {
                string gradeInput = await DisplayPromptAsync("Student Info", "Enter Grade:", initialValue: student.Grade.ToString());

                if (int.TryParse(gradeInput, out grade) && grade >= 1 && grade <= 100)
                {
                    break; 
                }
                else
                {
                    await DisplayAlert("Error", "Grade must be between 1 and 100.", "OK");
                }
            }

            return new StudentInfo
            {
                Name = name,
                Discipline = discipline,
                Department = department,
                Grade = grade
            };
        }
    }
}
