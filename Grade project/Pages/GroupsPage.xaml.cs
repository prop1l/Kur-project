using Grade_project.Database.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace Grade_project.Pages
{
    public partial class GroupsPage : Page
    {
        private readonly HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5172/api/") };

        public GroupsPage()
        {
            InitializeComponent();
            LoadGroupsAsync();
        }

        private async void LoadGroupsAsync()
        {
            var groups = await _client.GetFromJsonAsync<List<Group>>("Groups");
            GroupsDataGrid.ItemsSource = groups;
        }

        //private void AddGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddEditGroupWindow();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        LoadGroupsAsync();
        //    }
        //}

        //private void EditGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    if (GroupsDataGrid.SelectedItem is Group selected)
        //    {
        //        var dialog = new AddEditGroupWindow(selected);
        //        if (dialog.ShowDialog() == true)
        //        {
        //            LoadGroupsAsync();
        //        }
        //    }
        //}

        private async void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            if (GroupsDataGrid.SelectedItem is Group selected)
            {
                var result = MessageBox.Show("Удалить группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _client.DeleteAsync($"Groups/{selected.GroupId}");
                    LoadGroupsAsync();
                }
            }
        }
    }
}