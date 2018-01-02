using Microsoft.Win32;
using net.sf.mpxj;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ProjectSuccessWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MSProjectFileWorker fileWorker;
        MSProjectAnalyzer projectAnalyzer;
        PDFBuilder builder;

        List<TaskInformation> tasks;
        List<ResourceInformation> resources;

        //Files
        string mppFilePath;
        OpenFileDialog openFileDialog;
        string reportFilePath;
        SaveFileDialog saveFileDialog;

        public MainWindow()
        {
            InitializeComponent();
            fileWorker = new MSProjectFileWorker();
            projectAnalyzer = new MSProjectAnalyzer();
            builder = new PDFBuilder();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.mpp";

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.pdf";

            #region Testing
            fileWorker.ReadFile(@"C:\Users\Skiiru\Documents\Visual Studio 2017\Projects\ProjectSuccess\ProjectSuccessWPF\test.mpp");
            projectAnalyzer.project = fileWorker.projectFile;
            tasks = projectAnalyzer.GetTasksWithHierarhy();
            TreeViewItem firstItem = new TreeViewItem();
            firstItem.Header = "Текущий проект";
            CreateTreeViewItems(tasks, ref firstItem);
            TasksTreeView.Items.Add(firstItem);
            builder.CreateReport(@"test.pdf", tasks, projectAnalyzer.GetProjectProperties());
            #endregion
        }

        void CreateTreeViewItems(List<TaskInformation> tasks, ref TreeViewItem item)
        {
            foreach (TaskInformation taskInformation in tasks)
            {
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.IsExpanded = true;
                treeViewItem.Header = "Задача \"" + taskInformation.task.getName() + '\"';
                treeViewItem.Items.Add("Продолжительность: " + taskInformation.GetConvertedDuration(projectAnalyzer.project.getProjectProperties()));
                TreeViewItem resourcesItem = new TreeViewItem();
                resourcesItem.IsExpanded = true;
                if (taskInformation.resources.Count != 0)
                {
                    resourcesItem.Header = "Ресурсы";
                    foreach (Resource resource in taskInformation.resources)
                    {
                        resourcesItem.Items.Add(resource.getName());

                    }
                    treeViewItem.Items.Add(resourcesItem);
                }
                if (taskInformation.childTasks.Count != 0)
                {
                    TreeViewItem subTusks = new TreeViewItem();
                    subTusks.IsExpanded = true;
                    subTusks.Header = "Подзадачи";
                    CreateTreeViewItems(taskInformation.childTasks, ref subTusks);
                    treeViewItem.Items.Add(subTusks);
                }
                item.IsExpanded = true;
                item.Items.Add(treeViewItem);
            }
        }

        void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OpenFile()
        {
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != null && openFileDialog.FileName != string.Empty)
            {
                mppFilePath = openFileDialog.FileName;
            }
            else
                ShowError("Ошибка при открытии файла.");
            
        }

        void SaveFile()
        {
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != null && saveFileDialog.FileName != string.Empty)
                reportFilePath = saveFileDialog.FileName;
            else
                ShowError("Ошибка при сохранении файла.");
        }
    }
}
