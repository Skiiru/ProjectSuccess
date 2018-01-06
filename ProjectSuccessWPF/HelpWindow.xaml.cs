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
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        string helpText = "Данная программа создана в рамках обучения по курсу Программной инженерии (магистры) в УлГТУ в 2017-2018гг. " + Environment.NewLine +
            "Автор - Полбин Алексей Евгеньевич, студент группы ПИмд-21." + Environment.NewLine +
            "Научный руководитель - Афанасьева Татьяна Васильевна, профессор кафедры ИС.";

        public HelpWindow()
        {
            InitializeComponent();
            InformationTextBlock.Text = helpText;
        }
    }
}
