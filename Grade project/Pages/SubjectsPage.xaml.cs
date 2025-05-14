using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace Grade_project.Pages
{
    public partial class SubjectsPage : Page
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };

        public SubjectsPage()
        {
            InitializeComponent();
            LoadSubjectsAsync();
        }

        private async void LoadSubjectsAsync()
        {
            var subjects = await _client.GetFromJsonAsync<List<Subject>>("Subjects");
            SubjectsDataGrid.ItemsSource = subjects;
        }

        //private void AddSubject_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddEditSubjectWindow();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        LoadSubjectsAsync();
        //    }
        //}

        //private void EditSubject_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SubjectsDataGrid.SelectedItem is Subject selected)
        //    {
        //        var dialog = new AddEditSubjectWindow(selected);
        //        if (dialog.ShowDialog() == true)
        //        {
        //            LoadSubjectsAsync();
        //        }
        //    }
        //}

        private async void DeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectsDataGrid.SelectedItem is Subject selected)
            {
                var result = MessageBox.Show("Удалить предмет?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _client.DeleteAsync($"Subjects/{selected.SubjectId}");
                    LoadSubjectsAsync();
                }
            }
        }
    }
}