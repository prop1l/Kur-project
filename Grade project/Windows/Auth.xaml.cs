using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Grade_project.Windows
{
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
        }


        private bool isDarkTheme = true;

        private void ThemeSwitch_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;

            if (isDarkTheme)
            {
                app.ChangeTheme("LightTheme");
            }
            else
            {
                app.ChangeTheme("DarkTheme");
            }

            isDarkTheme = !isDarkTheme;
        }


        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }


        private void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }


        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized) Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }


        private void UpdatePlaceholder(object sender, TextBlock placeholder)
        {
            if (sender is TextBox textBox)
            {
                placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (sender is PasswordBox passwordBox)
            {
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Parent is Grid grid)
            {
                var placeholder = grid.Children.OfType<TextBlock>().FirstOrDefault();
                if (placeholder != null)
                {
                    placeholder.Visibility = Visibility.Collapsed;
                }
            }
        }


        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Parent is Grid grid)
            {
                var placeholder = grid.Children.OfType<TextBlock>().FirstOrDefault();
                if (placeholder != null)
                {
                    UpdatePlaceholder(sender, placeholder);
                }
            }
        }


        private async void Auth_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "http://localhost:5172/Users/Authenticate";

            try
            {
                string email = Email.Text; 
                string password = PasswordBox.Password;

                if (string.IsNullOrWhiteSpace(email) || email == "Почта" ||
                    string.IsNullOrWhiteSpace(password) || password == "Пароль")
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var queryParams = new Dictionary<string, string>
                {
                    { "email", email },
                    { "password", password }
                };

                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var fullUrl = $"{apiUrl}?{queryString}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(fullUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var user = await response.Content.ReadFromJsonAsync<User>();

                        if (user != null)
                        {
                            var mainWindow = new WelcomeWindow(user.UserId);
                            mainWindow.Show();

                            this.Close();
                        }
                        else
                        {
                            MyMessageBox.Show("Не удалось получить данные пользователя.", "Ошибка", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        MyMessageBox.Show("Ошибка", $"Ошибка авторизации: {errorResponse}", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RegLink_Click(object sender, MouseButtonEventArgs e)
        {
            var regWindow = new RegWindow();

            regWindow.Show();
            this.Close();
        }
    }
}