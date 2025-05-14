using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Grade_project.Windows
{
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized) this.WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        //private void UpdatePlaceholder(object sender, TextBlock placeholder)
        //{
        //    if (sender is TextBox textBox)
        //    {
        //        placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //    else if (sender is PasswordBox passwordBox)
        //    {
        //        placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var parentGrid = textBox?.Parent as Grid;

            if (parentGrid != null)
            {
                var placeholder = parentGrid.Children.OfType<TextBlock>().FirstOrDefault();
                placeholder.Visibility = Visibility.Collapsed;
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var parentGrid = textBox?.Parent as Grid;

            if (parentGrid != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                var placeholder = parentGrid.Children.OfType<TextBlock>().FirstOrDefault();
                placeholder.Visibility = Visibility.Visible;
            }
        }

        private void Input_GotFocus1(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var parentGrid = passwordBox?.Parent as Grid;

            if (parentGrid != null)
            {
                var placeholder = parentGrid.Children.OfType<TextBlock>().FirstOrDefault();
                placeholder.Visibility = Visibility.Collapsed;
            }
        }

        private void Input_LostFocus1(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var parentGrid = passwordBox?.Parent as Grid;

            if (parentGrid != null && string.IsNullOrEmpty(passwordBox.Password))
            {
                var placeholder = parentGrid.Children.OfType<TextBlock>().FirstOrDefault();
                placeholder.Visibility = Visibility.Visible;
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "http://localhost:5172/Api";

            string login = LoginBox.Text;
            string email = EmailBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || login == "Логин" ||
                string.IsNullOrWhiteSpace(email) || email == "Почта" ||
                string.IsNullOrWhiteSpace(password) || password == "Пароль")
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (confirmPassword == null || confirmPassword != password)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new
            {
                Login = login,
                Email = email,
                Password = password
            };

            string jsonContent = JsonSerializer.Serialize(newUser);
            HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                        if (responseData != null)
                        {
                            var response1 = await client.GetAsync("http://localhost:5172/Users");
                            var id = (await response1.Content.ReadFromJsonAsync<List<User>>()).FirstOrDefault(u => u.Email == email);

                            


                            var confirmEmailWindow = new ConfirmEmailWindow(id.UserId);
                            confirmEmailWindow.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при регистрации. Подробности: {errorResponse}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            var logWin = new Auth();
            logWin.Show();
            this.Close();
        }
    }

    public class RegisterResponse
    {
        public string Message { get; set; }
        public string Token { get; set; }
    }
}