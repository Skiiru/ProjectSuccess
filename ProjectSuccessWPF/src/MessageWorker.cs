using System.Windows;

namespace ProjectSuccessWPF
{
    class MessageWorker
    {
        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
