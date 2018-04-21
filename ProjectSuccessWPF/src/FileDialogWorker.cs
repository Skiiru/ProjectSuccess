using System;
using System.Windows.Forms;

namespace ProjectSuccessWPF
{
    public class FileDialogWorker
    {
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;

        public string FilePath { get; private set; }
        public string ReportFilePath { get; private set; }

        public FileDialogWorker()
        {
            openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы MSProject|*.mpp"
            };
            saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF|*.pdf"
            };
        }

        public bool OpenFile()
        {
            bool result = openFileDialog.ShowDialog() == DialogResult.OK ? true : false;
            if (result)
                if (openFileDialog.FileName != null && openFileDialog.FileName != string.Empty)
                {
                    FilePath = openFileDialog.FileName;
                }
                else
                    MessageWorker.ShowError("Ошибка при открытии файла.");
            return result;
        }

        public bool SaveFile()
        {
            bool result = saveFileDialog.ShowDialog() == DialogResult.OK ? true : false;
            if (result)
                if (saveFileDialog.FileName != null && saveFileDialog.FileName != string.Empty)
                    ReportFilePath = saveFileDialog.FileName;
                else
                    MessageWorker.ShowError("Ошибка при сохранении файла.");
            return result;
        }
    }
}
