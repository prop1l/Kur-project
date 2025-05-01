using System.Windows;
using Grade_project.Database.Models;

namespace Grade_project.Windows
{
    public partial class AddEditUserWindow : Window
    {
        public User User { get; private set; }

        public AddEditUserWindow()
        {
            InitializeComponent();
            User = new User();
        }

        public AddEditUserWindow(User user)
        {
            InitializeComponent();
            User = user;

            LoginTextBox.Text = user.Login;
            EmailTextBox.Text = user.Email;
            IsEmailConfirmedCheckBox.IsChecked = user.IsEmailConfirmed;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User.Login = LoginTextBox.Text;
            User.Email = EmailTextBox.Text;
            User.IsEmailConfirmed = IsEmailConfirmedCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}