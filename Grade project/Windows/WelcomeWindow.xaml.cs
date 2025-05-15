using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;

namespace Grade_project.Windows
{
    public partial class WelcomeWindow : Window
    {
        private readonly int _userId;
        public string UserLogin { get; set; }

        public WelcomeWindow(int id)
        {
            InitializeComponent();
            _userId = id;
            XID.Content = id;
            LoadUserDataAsync();
            //LoadRatingsAsync();
            //LoadFavoriteRatingsAsync();
        }

        //private async void LoadRatingsAsync()
        //{
        //    try
        //    {
        //        using var client = new HttpClient();
        //        var response = await client.GetAsync("http://localhost:5172/api/Ratings");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();
        //            RatingsList.ItemsSource = ratings;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка загрузки рейтингов: {ex.Message}");
        //    }
        //}

        //private async void LoadFavoriteRatingsAsync()
        //{
        //    try
        //    {
        //        using var client = new HttpClient();
        //        var response = await client.GetAsync($"http://localhost:5172/api/FavoriteRatings/{_userId}");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteRating>>();
        //            FavoriteRatingsList.ItemsSource = favorites;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка загрузки избранных рейтингов: {ex.Message}");
        //    }
        //}

        private async void LoadUserDataAsync()
        {
            try
            {
                using var client = new HttpClient();
                var userResponse = await client.GetAsync($"http://localhost:5172/api/Users/{_userId}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Не удалось загрузить данные пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var user = await userResponse.Content.ReadFromJsonAsync<User>();
                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                UserLogin = user.Login;
                this.DataContext = this; 

                var roleResponse = await client.GetAsync($"http://localhost:5172/api/UserRoles/{_userId}");
                if (!roleResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Не удалось загрузить роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var roles = await roleResponse.Content.ReadFromJsonAsync<List<UserRole>>();
                if (roles != null && roles.Any(r => r.Role?.RoleName == "Admin"))
                {
                    AdminButton.Visibility = Visibility.Visible;
                }
                else
                {
                    AdminButton.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow(_userId);
            adminWindow.Show();
            this.Close();
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
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }
}