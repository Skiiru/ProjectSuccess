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

        public MainWindow()
        {
            InitializeComponent();
            fileWorker = new MSProjectFileWorker();
            projectAnalyzer = new MSProjectAnalyzer();
            builder = new PDFBuilder();

            fileWorker.ReadFile(@"C:\Users\Skiiru\Documents\Visual Studio 2017\Projects\ProjectSuccess\ProjectSuccessWPF\test.mpp");
            projectAnalyzer.project = fileWorker.projectFile;
            List<TaskInformation> tasks = projectAnalyzer.GetTasksWithHierarhy();
            TreeViewItem firstItem = new TreeViewItem();
            firstItem.Header = "Текущий проект";
            CreateTreeViewItems(tasks, ref firstItem);
            TasksTreeView.Items.Add(firstItem);
            builder.CreateReport(@"test.pdf", tasks, projectAnalyzer.GetProjectProperties());
        }

        void CreateTreeViewItems(List<TaskInformation> tasks, ref TreeViewItem item)
        {
            foreach (TaskInformation taskInformation in tasks)
            {
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = "Задача \"" + taskInformation.task.getName() + '\"';
                treeViewItem.Items.Add("Продолжительность: " + taskInformation.GetConvertedDuration(projectAnalyzer.project.getProjectProperties()));
                TreeViewItem resourcesItem = new TreeViewItem();
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
                    subTusks.Header = "Подзадачи";
                    CreateTreeViewItems(taskInformation.childTasks, ref subTusks);
                    treeViewItem.Items.Add(subTusks);
                }
                item.Items.Add(treeViewItem);
            }
        }
    }
}
