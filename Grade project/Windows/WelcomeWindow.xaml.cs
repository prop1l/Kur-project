using System.Windows;
using System.Windows.Input;


namespace Grade_project.Windows
{
    public partial class WelcomeWindow : Window
    {
        private string UserRole { get; set; } = "User";
        public WelcomeWindow(int id)
        {
            InitializeComponent();

            XID.Content = id;
            LoadUserData();
        }

        private void LoadUserData()
        {
            UserRole = "Admin";

            if (UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.Show();
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
    }
}