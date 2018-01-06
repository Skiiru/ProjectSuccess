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
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ReportSavedWindow : Window
    {
        string reportPath;

        public ReportSavedWindow() : this(string.Empty)
        {
        }

        public ReportSavedWindow(string path)
        {
            InitializeComponent();
            reportPath = path;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(reportPath);
            this.Close();
        }
    }
}
