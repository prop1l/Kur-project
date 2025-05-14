using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Grade_project.Windows;

namespace Grade_project.Pages
{
    public partial class SpecialitiesPage : Page
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };

        public SpecialitiesPage()
        {
            InitializeComponent();
            LoadSpecialitiesAsync();
        }

        private async void LoadSpecialitiesAsync()
        {
            var specialities = await _client.GetFromJsonAsync<List<Speciality>>("Specialities");
            SpecialitiesDataGrid.ItemsSource = specialities;
        }

        private void AddSpeciality_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditSpecialityWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadSpecialitiesAsync();
            }
        }

        private void EditSpeciality_Click(object sender, RoutedEventArgs e)
        {
            if (SpecialitiesDataGrid.SelectedItem is Speciality selected)
            {
                var dialog = new AddEditSpecialityWindow(selected);
                if (dialog.ShowDialog() == true)
                {
                    LoadSpecialitiesAsync();
                }
            }
        }

        private async void DeleteSpeciality_Click(object sender, RoutedEventArgs e)
        {
            if (SpecialitiesDataGrid.SelectedItem is Speciality selected)
            {
                var result = MessageBox.Show("Удалить специальность?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _client.DeleteAsync($"Specialities/{selected.SpecialityId}");
                    LoadSpecialitiesAsync();
                }
            }
        }
    }
}