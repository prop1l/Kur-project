using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace Grade_project.Pages
{
    public partial class TeachersPage : Page
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };

        public TeachersPage()
        {
            InitializeComponent();
            LoadTeachersAsync();
        }

        private async void LoadTeachersAsync()
        {
            var teachers = await _client.GetFromJsonAsync<List<Teacher>>("Teachers");
            TeachersDataGrid.ItemsSource = teachers;
        }

        //private void AddTeacher_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddEditTeacherWindow();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        LoadTeachersAsync();
        //    }
        //}

        //private void EditTeacher_Click(object sender, RoutedEventArgs e)
        //{
        //    if (TeachersDataGrid.SelectedItem is Teacher selectedTeacher)
        //    {
        //        var dialog = new AddEditTeacherWindow(selectedTeacher);
        //        if (dialog.ShowDialog() == true)
        //        {
        //            LoadTeachersAsync();
        //        }
        //    }
        //}

        private async void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (TeachersDataGrid.SelectedItem is Teacher selectedTeacher)
            {
                var result = MessageBox.Show("Вы уверены?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _client.DeleteAsync($"Teachers/{selectedTeacher.TeacherId}");
                    LoadTeachersAsync();
                }
            }
        }
    }
}