using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http.Json;
using Grade_project.Database.Models;

namespace Grade_project.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserInfosPage.xaml
    /// </summary>
    public partial class UserInfosPage : Page
    {

        private readonly HttpClient _httpClient;

        public UserInfosPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5172/")
            };
            LoadTokensAsync();
        }
        private async void LoadTokensAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("UserInfos");
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadFromJsonAsync<List<UserInfo>>();
                    tokensDataGrid.ItemsSource = token;
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

        private async void DeleteToken_Click(object sender, RoutedEventArgs e)
        {
            if (tokensDataGrid.SelectedItem is UserInfo selectedUserInfos)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот токен?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _httpClient.DeleteAsync($"UserInfo/{selectedUserInfos.UserId}");
                    if (response.IsSuccessStatusCode)
                    {
                        LoadTokensAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при удалении токена: {response.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите токен для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
