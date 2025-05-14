using System.Windows;
using System.Windows.Controls;

namespace Grade_project.Windows
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void EntityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pageName)
            {
                try
                {
                    switch (pageName)
                    {
                        case "GroupsPage":
                            MainFrame.Navigate(new Uri("/Pages/GroupsPage.xaml", UriKind.Relative));
                            break;
                        case "RatingsPage":
                            MainFrame.Navigate(new Uri("/Pages/RatingsPage.xaml", UriKind.Relative));
                            break;
                        case "SpecialitiesPage":
                            MainFrame.Navigate(new Uri("/Pages/SpecialitiesPage.xaml", UriKind.Relative));
                            break;
                        case "SubjectsPage":
                            MainFrame.Navigate(new Uri("/Pages/SubjectsPage.xaml", UriKind.Relative));
                            break;
                        case "TeachersPage":
                            MainFrame.Navigate(new Uri("/Pages/TeachersPage.xaml", UriKind.Relative));
                            break;
                        case "UsersPage":
                            MainFrame.Navigate(new Uri("/Pages/UsersPage.xaml", UriKind.Relative));
                            break;
                        case "TokensPage":
                            MainFrame.Navigate(new Uri("/Pages/TokensPage.xaml", UriKind.Relative));
                            break;
                        case "UserInfosPage":
                            MainFrame.Navigate(new Uri("/Pages/UserInfosPage.xaml", UriKind.Relative));
                            break;
                        default:
                            throw new ArgumentException($"Неизвестная страница: {pageName}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки страницы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Drag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}