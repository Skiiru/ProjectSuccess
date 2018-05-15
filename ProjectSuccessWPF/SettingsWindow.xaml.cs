using System.Windows;

namespace ProjectSuccessWPF
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            if (AppSettings.Settings.Default.RedmineConnectionType != "API")
                LoginRB.IsChecked = true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (int.TryParse(DaysInSprintTB.Text, out int days) && days > 0)
            {
                AppSettings.Settings.Default.RedmineHost = HostTB.Text;
                AppSettings.Settings.Default.RedmineApiKey = ApiKeyTB.Text;
                AppSettings.Settings.Default.RedmineLogin = RedmineLoginTB.Text;
                AppSettings.Settings.Default.RedminePassword = RedminePasswordTB.Text;
                AppSettings.Settings.Default.DaysInSprint = days;
                if (ApiRB.IsChecked.HasValue && ApiRB.IsChecked.Value)
                    AppSettings.Settings.Default.RedmineConnectionType = "API";
                else
                    AppSettings.Settings.Default.RedmineConnectionType = "LOGIN";
                AppSettings.Settings.Default.Save();
                Close();
            }
            else
            {
                MessageWorker.ShowError("Неверно утсновлен параметр \"Кол-во дней в спринте\"");
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
 