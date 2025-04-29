using System.Windows;

namespace Grade_project
{

    public partial class App : Application
    {
        public void ChangeTheme(string themeName)
        {
            // Загружаем новый словарь ресурсов
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative)
            };

            // Очищаем текущие ресурсы и добавляем новые
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dict);

            // Принудительно обновляем все открытые окна
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsLoaded)
                {
                    window.Resources.MergedDictionaries.Clear();
                    window.Resources.MergedDictionaries.Add(dict);
                }
            }
        }
    }

}
