using System.Net.Http.Json;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using Grade_project.Database.Models;

namespace Grade_project.Windows
{
    public partial class SelectRoleWindow : Window
    {
        private readonly int _userId;
        private readonly HttpClient _client = new();
        public int? SelectedRoleId { get; private set; }

        public SelectRoleWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadRolesAsync();
        }

        private async void LoadRolesAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Role>>("http://localhost:5172/Roles");
                if (response != null && response.Any())
                {
                    RoleComboBox.ItemsSource = response;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleComboBox.SelectedItem is Role selectedRole)
            {
                var dto = new UserRoleDto
                {
                    UserId = _userId,
                    RoleId = selectedRole.RoleId
                };

                var assignResponse = await _client.PostAsJsonAsync("http://localhost:5172/api/UserRoles/add", dto);

                if (assignResponse.IsSuccessStatusCode)
                {
                    SelectedRoleId = selectedRole.RoleId;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    var error = await assignResponse.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка назначения роли: {error}");
                }
            }
            else
            {
                MessageBox.Show("Выберите роль из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    public class UserRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}