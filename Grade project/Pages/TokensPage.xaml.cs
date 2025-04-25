using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;


namespace Grade_project.Pages
{
    public partial class TokensPage : Page
    {
        private readonly HttpClient _httpClient;
        public TokensPage()
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
                var response = await _httpClient.GetAsync("Tokens");
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadFromJsonAsync<List<Tokenss>>();
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
            if (tokensDataGrid.SelectedItem is Tokenss selectedToken)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот токен?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _httpClient.DeleteAsync($"Tokens/{selectedToken.TokenId}");
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

        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот токен?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var response = await _httpClient.DeleteAsync($"Tokens/All");
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
    }
}
