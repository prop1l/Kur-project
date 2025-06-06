﻿using Grade_project.Database.Models;
using Grade_project.Windows;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace Grade_project.Pages
{
    public partial class UsersPage : Page
    {
        private readonly HttpClient _httpClient;

        public UsersPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5172/")
            };
            LoadUsersAsync();
        }
        private async void LoadUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5172/Users");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<User>>();
                    UsersDataGrid.ItemsSource = users;
                }
                else
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {response.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddEditUserWindow(); 
            if (addUserWindow.ShowDialog() == true)
            {
                var user = addUserWindow.User;

                var response = await _httpClient.PostAsJsonAsync("Users/Register", user);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsersAsync(); 
                }
                else
                {
                    MessageBox.Show($"Ошибка при добавлении пользователя: {response.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var editUserWindow = new AddEditUserWindow(selectedUser); 
                if (editUserWindow.ShowDialog() == true)
                {
                    var updatedUser = editUserWindow.User;

                    var response = await _httpClient.PutAsJsonAsync($"Users/{updatedUser.UserId}", updatedUser);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Пользователь успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsersAsync(); 
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при обновлении пользователя: {response.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void ChangeRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var roles = await _httpClient.GetFromJsonAsync<List<Role>>("Roles");
                if (roles == null || !roles.Any())
                {
                    MessageBox.Show("Не удалось загрузить список ролей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new SelectRoleWindow(selectedUser.UserId);
                if (dialog.ShowDialog() == true)
                {
                    var dto = new UserRoleDto
                    {
                        UserId = selectedUser.UserId,
                        RoleId = dialog.SelectedRoleId.Value
                    };

                    await _httpClient.PostAsJsonAsync("UserRoles", dto);
                    LoadUsersAsync();
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для изменения роли.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _httpClient.DeleteAsync($"Users/{selectedUser.UserId}");
                    if (response.IsSuccessStatusCode)
                    {
                        LoadUsersAsync(); 
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {response.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class UserRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}