using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace Grade_project.Windows
{
    public partial class AddEditUserWindow : Window
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };
        public User User { get; private set; }

        // Конструктор для добавления нового пользователя
        public AddEditUserWindow()
        {
            InitializeComponent();
            User = new User();
            this.DataContext = this;
        }

        // Конструктор для редактирования существующего пользователя
        public AddEditUserWindow(User user) : this()
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User.UserId = user.UserId;
            User.Login = user.Login;
            User.Email = user.Email;
            User.IsEmailConfirmed = user.IsEmailConfirmed;
            LoginTextBox.Text = user.Login;
            EmailTextBox.Text = user.Email;
            IsEmailConfirmedCheckBox.IsChecked = user.IsEmailConfirmed;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Имя пользователя и Email обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                User.Login = login;
                User.Email = email;
                User.IsEmailConfirmed = IsEmailConfirmedCheckBox.IsChecked ?? false;

                if (User.UserId == 0) 
                {
                    var response = await _client.PostAsJsonAsync("Users", User);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Пользователь успешно добавлен.");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Не удалось добавить пользователя:\n{error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else 
                {
                    var response = await _client.PutAsJsonAsync($"Users/{User.UserId}", User);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Данные пользователя обновлены.");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Не удалось обновить данные:\n{error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}