using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace MauiApp3
{
    public partial class MainPage : ContentPage
    {
        // Про автора
        private async void OnAboutClicked(object sender, EventArgs e)
        {
            string aboutInfo = "Автор: Ступак Катерина\n" +
                               "Курс: 2\n" +
                               "Група: К-26\n" +
                               "Рік: 2024\n" +
                               "Опис: Це програма для роботи з файлами Json. Дозволяє побачити інформацію про оцінки студентів, додати, редагувати та видаляти інформацію.";

            await DisplayAlert("Про програму", aboutInfo, "OK");
        }

        // Додавання студента
        private async void OnAddClicked(object sender, EventArgs e)
        {
            var student = await ShowStudentForm(new StudentInfo());
            if (student != null)
            {
                _students.Add(student);
                DisplayData(_students);
            }
        }

        // Редагування студента
        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null)
            {
                await DisplayAlert("Error", "No student selected!", "OK");
                return;
            }

            var updatedStudent = await ShowStudentForm(_selectedStudent);
            if (updatedStudent != null)
            {
                var index = _students.IndexOf(_selectedStudent);
                _students[index] = updatedStudent;
                _selectedStudent = null;
                DisplayData(_students);
            }
        }

        // Видалення студента
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null)
            {
                await DisplayAlert("Error", "No student selected!", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete {_selectedStudent.Name}?", "Yes", "No");
            if (confirm)
            {
                _students.Remove(_selectedStudent);
                _selectedStudent = null;
                DisplayData(_students);
            }
        }
        private void OnStudentSelected(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is StudentInfo student)
            {
                _selectedStudent = student;
                DisplayAlert("Student Selected", $"Selected: {student.Name}", "OK");
            }
        }
        // Пошук студента
        private void OnSearchClicked(object sender, EventArgs e)
        {
            var searchText = SearchEntry.Text ?? string.Empty;
            if (!System.Text.RegularExpressions.Regex.IsMatch(searchText, @"^[a-zA-Z0-9\s]*$"))
            {
                DisplayAlert("Error", "Please. Use only letters and numbers.", "OK");
                return;
            }
            var filteredData = LINQ.SearchStudents(_students, searchText);
            DisplayData(filteredData);
        }

    }
}
