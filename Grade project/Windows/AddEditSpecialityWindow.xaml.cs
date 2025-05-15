using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Grade_project.Database.Models;

namespace Grade_project.Windows
{
    public partial class AddEditSpecialityWindow : Window
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };

        public Speciality Speciality { get; set; }

        public AddEditSpecialityWindow()
        {
            InitializeComponent();

            Speciality = new Speciality
            {
                DateFormation = DateTime.Now
            };

            DataContext = this;
        }

        public AddEditSpecialityWindow(Speciality speciality) : this()
        {
            if (speciality == null)
                throw new ArgumentNullException(nameof(speciality));

            Speciality.SpecialityId = speciality.SpecialityId;
            Speciality.SpecName = speciality.SpecName;
            Speciality.DateFormation = speciality.DateFormation;
            Speciality.DateDisbandment = speciality.DateDisbandment;

            SpecNameTextBox.Text = speciality.SpecName;
            DateDisbandmentTextBox.Text = speciality.DateDisbandment?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string specName = SpecNameTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(specName))
                {
                    MessageBox.Show("Пожалуйста, заполните название специальности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Speciality.SpecName = specName;
                if (DateTime.TryParse(DateDisbandmentTextBox.Text, out var disbandmentDate))
                    Speciality.DateDisbandment = disbandmentDate;
                else
                    Speciality.DateDisbandment = null;

                if (Speciality.SpecialityId == 0)
                {
                    var response = await _client.PostAsJsonAsync("Specialities", Speciality);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Специальность успешно добавлена.");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Не удалось добавить специальность:\n{error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var response = await _client.PutAsJsonAsync($"Specialities/{Speciality.SpecialityId}", Speciality);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Специальность обновлена.");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Не удалось обновить специальность:\n{error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            DialogResult = false;
            Close();
        }
    }
}