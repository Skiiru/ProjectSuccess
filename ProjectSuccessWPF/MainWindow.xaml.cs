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
            builder = new PDFBuilder();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы MSProject|*.mpp";

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF|*.pdf";

            #region Testing
            fileWorker.ReadFile(@"C:\Users\Skiiru\Documents\Visual Studio 2017\Projects\ProjectSuccess\ProjectSuccessWPF\test2.mpp");
            projectAnalyzer = new MSProjectAnalyzer(fileWorker.projectFile);
            tasks = projectAnalyzer.GetTasksWithHierarhy();
            TreeViewItem firstItem = new TreeViewItem();
            firstItem.Header = "Текущий проект";
            CreateTreeViewItems(tasks, ref firstItem);
            TasksTreeView.Items.Add(firstItem);

            resources = projectAnalyzer.GetResources();

            builder.CreateReport(@"test.pdf", tasks, resources, projectAnalyzer.GetProjectProperties());
            #endregion
        }

        void CreateTreeViewItems(List<TaskInformation> tasks, ref TreeViewItem item)
        {
            foreach (TaskInformation taskInformation in tasks)
            {
                TreeViewItem treeViewItem = new TreeViewItem
                {
                    IsExpanded = true,
                    Header = "Задача \"" + taskInformation.taskName + "\" (" + taskInformation.completePecrentage + "%)"
                };
                treeViewItem.Items.Add("Продолжительность: " + taskInformation.duration);
                if (taskInformation.overtimeWork != "0.0")
                    treeViewItem.Items.Add("Переработка: " + taskInformation.overtimeWork);
                if (taskInformation.overCost != 0.0)
                    treeViewItem.Items.Add("Перерасход: " + taskInformation.overCost);
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
