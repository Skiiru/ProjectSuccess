using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectSuccessWPF
{
    /// <summary>
    /// Логика взаимодействия для OpenProjectWindow.xaml
    /// </summary>
    public partial class OpenProjectWindow : Window
    {
        FileDialogWorker FileDialogWorker;
        RedmineWorker redmineWorker;
        List<Redmine.RedmineProject> Projects { get; set; }

        public OpenProjectWindow()
        {
            InitializeComponent();
            FileDialogWorker = new FileDialogWorker();
            redmineWorker = new RedmineWorker();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                this.Close();
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new HelpWindow().ShowDialog();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (FileDialogWorker.OpenFile())
            {
                ProjectWindow projectWindow = new ProjectWindow(FileDialogWorker);
                projectWindow.ShowDialog();
            }

        }

        void LoadRedmineProjects()
        {
            try
            {
                Projects = redmineWorker.LoadProjects();
                ProjectsListBox.ItemsSource = Projects;
            }
            catch (Exception ex)
            {
                MessageWorker.ShowError("Загрузка проектов из Redmine не удалась. Проверьте настройки подключения.");
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadRedmineProjects();
        }

        private void LoadRedmineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadRedmineProjects();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProjectWindow projectWindow = new ProjectWindow((sender as ListBoxItem).Content as Redmine.RedmineProject);
            projectWindow.ShowDialog();
        }
    }
}
