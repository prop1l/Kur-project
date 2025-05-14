using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Grade_project.Windows
{
    public partial class ConfirmEmailWindow : Window
    {
        public readonly int _id;
        public ConfirmEmailWindow(int id)
        {
            InitializeComponent();
            _id = id;
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

        private async void ResendTokenButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите отправить токен повторно?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            using (HttpClient _client = new HttpClient())
            {
                if (result == MessageBoxResult.Yes)
                {
                    var resendUrl = $"http://localhost:5172/Users/ResendToken?userId={_id}";

                    try
                    {
                        var response = await _client.GetAsync(resendUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Токен отправлен повторно.");
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Ошибка: {error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка отправки запроса: {ex.Message}");
                    }
                }
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string token = TokenBox.Text;
            string apiUrl = $"http://localhost:5172/Users/VerifyToken?userId={_id}&verifyToken={token}";


            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Пожалуйста, введите токен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (HttpClient client = new HttpClient())
            {

                var checkToken = await client.GetAsync(apiUrl);


                if (checkToken.IsSuccessStatusCode)
                {
                    try
                    {
                        var response = await client.GetAsync(apiUrl);

                        var welWin = new WelcomeWindow(_id);
                        welWin.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Токен введён не верно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}