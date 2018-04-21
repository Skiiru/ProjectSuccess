using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using net.sf.mpxj;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ProjectSuccessWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        MSProjectFileWorker fileWorker;
        MSProjectFileParser msProjectParser;
        PDFBuilder builder;

        FileDialogWorker fileDialog;
        IProject Project { get; set; }

        List<ChartContainer> charts;
        string currencySymbol;
        string resourcesWorktimeSymbol;
        string taskDurationSymbol;
        double hoursPerDay;

        public ProjectWindow()
        {
            InitializeComponent();
        }

        public ProjectWindow(FileDialogWorker worker)
        {
            InitializeComponent();
            fileDialog = worker;
            

            builder = new PDFBuilder();
            charts = new List<ChartContainer>();

            fileWorker = new MSProjectFileWorker();
            fileWorker.ReadFile(fileDialog.FilePath);
            msProjectParser = new MSProjectFileParser(fileWorker.ProjectFile);
            Project = new MSProjectProject(msProjectParser);
        }

        public ProjectWindow(Redmine.RedmineProject project)
        {
            InitializeComponent();
            Project = project;

            //Hide cost params
            ResourcesCostPerUseChartTab.Visibility = Visibility.Hidden;
            TaskCostChartTab.Visibility = Visibility.Hidden;
            ResourcesCostPieChartTab.Visibility = Visibility.Hidden;
            TasksOverworkCostChartTab.Visibility = Visibility.Hidden;
        }

        void CreateTreeViewItems(List<TaskInformation> tasks, ref TreeViewItem item)
        {
            foreach (TaskInformation taskInformation in tasks)
            {
                TreeViewItem treeViewItem = new TreeViewItem
                {
                    IsExpanded = true,
                    Header = "Задача \"" + taskInformation.TaskName + "\" (" + taskInformation.CompletePercentage + "%)"
                };
                treeViewItem.Items.Add("Плановая продолжительность: " + taskInformation.Duration.Estimated);
                if (taskInformation.Duration.TotalDuration() != 0)
                    treeViewItem.Items.Add("Продолжительность: " + taskInformation.Duration.Spent);
                if (taskInformation.Duration.Overtime != 0)
                    treeViewItem.Items.Add("Переработка: " + taskInformation.Duration.Overtime);
                if (taskInformation.Cost != 0)
                    treeViewItem.Items.Add("Стоимость: " + taskInformation.Cost + currencySymbol);
                if (taskInformation.OverCost != 0.0)
                    treeViewItem.Items.Add("Перерасход: " + taskInformation.OverCost + currencySymbol);
                TreeViewItem resourcesItem = new TreeViewItem
                {
                    IsExpanded = true
                };
                if (taskInformation.Resources.Count != 0)
                {
                    resourcesItem.Header = "Ресурсы";
                    foreach (ResourceInformation resource in taskInformation.Resources)
                    {
                        resourcesItem.Items.Add(resource.ResourceName);

                    }
                    treeViewItem.Items.Add(resourcesItem);
                }
                if (taskInformation.ChildTasks.Count != 0)
                {
                    TreeViewItem subTusks = new TreeViewItem
                    {
                        IsExpanded = true,
                        Header = "Подзадачи"
                    };
                    CreateTreeViewItems(taskInformation.ChildTasks, ref subTusks);
                    treeViewItem.Items.Add(subTusks);
                }
                item.IsExpanded = true;
                item.Items.Add(treeViewItem);
            }
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

        private void PrepareWindowContent()
        {
            //Tree view
            TasksTreeView.Items.Clear();
            TreeViewItem firstItem = new TreeViewItem
            {
                Header = "Текущий проект"
            };

            if (Project.Rate != null)
            {
                if (!double.IsNaN(Project.Rate.ProjectOverCostPercentage))
                {
                    string s = "Оценка перерасхода средств: " + Math.Round(Project.Rate.ProjectOverCostPercentage, 2) + "%, " + Project.Rate.GetOvercostRateString() + ".";
                    firstItem.Items.Add(s);
                }

                if (!double.IsNaN(Project.Rate.MeanTaskDurationRate))
                {
                    string s = "Оценка средней продолжительность задач: " + Math.Round(Project.Rate.MeanTaskDurationRate, 2) + "%, " + Project.Rate.GetMeanTaskDurationString() + '.';
                    firstItem.Items.Add(s);
                }

                if (!double.IsNaN(Project.Rate.ProjectOvertimeRate))
                {
                    string s = "Оценка перерасхода времени: " + Math.Round(Project.Rate.ProjectOvertimeRate, 2) + "%, " + Project.Rate.GetProjectOvertimeString() + '.';
                    firstItem.Items.Add(s);
                }

                if (!double.IsNaN(Project.Rate.RecourcesTotalOverworkTime))
                {
                    string s = "Общее время переработки ресурсов: " + Math.Round(Project.Rate.RecourcesTotalOverworkTime, 2) + "ч.";
                    firstItem.Items.Add(s);
                }
            }

            CreateTreeViewItems(Project.Tasks, ref firstItem);
            TasksTreeView.Items.Add(firstItem);

            //Data grid
            ResourcesDataGrid.ItemsSource = Project.Resources;

            //Symbols
            hoursPerDay = msProjectParser.GetProjectProperties().getMinutesPerDay().doubleValue() / 60;
            currencySymbol = msProjectParser.GetProjectProperties().getCurrencySymbol();
            string forTaskSymbol = msProjectParser.GetProjectProperties().getBaselineDuration().toString();
            taskDurationSymbol = forTaskSymbol[forTaskSymbol.Length - 1].ToString();
            string forResourcesSymbol = "h";
            resourcesWorktimeSymbol = forResourcesSymbol[forResourcesSymbol.Length - 1].ToString();

            //Charts
            if (Project is MSProjectProject)
            {
                ChartSeriesCreator.CreateTasksCostColumnSeries(Project.Tasks, TaskCostChart, currencySymbol);
                ChartSeriesCreator.CreateTasksOverworkCostCoulumnSeries(Project.Tasks, TasksOverworkCostChart, currencySymbol);
                ChartSeriesCreator.CreateResourcesCostPieSeries(Project.Resources, ResourcesCostPieChart);
                ChartSeriesCreator.CreateResourceCostPerTimeUnitColumnSeries(Project.Resources, ResourcesCostPerUseChart, currencySymbol);
            }

            ChartSeriesCreator.CreateTasksDurationColumnSeries(Project.Tasks, TasksDurationChart, taskDurationSymbol);
            ChartSeriesCreator.CreateTasksOverworkDurationCoulumnSeries(Project.Tasks, TasksOverworkDurationChart, taskDurationSymbol);
            ChartSeriesCreator.CreateResourcesWortimePieSeries(Project.Resources, ResourcesWorktimePieChart);
            ChartSeriesCreator.CreateTasksCountLineSeries(Project.Tasks, TasksCountChart);

            //Enable buttons
            CreateReportMenuItem.IsEnabled = true;
        }

        private void CreateReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (fileDialog.SaveFile())
            {
                charts.Clear();
                charts = WinFormsChartCreator.GetCharts(msProjectParser.GetTasksWithoutHierarhy(), Project.Resources);
                builder.CreateReport(fileDialog.ReportFilePath, Project, charts);
                (new ReportSavedWindow(fileDialog.ReportFilePath)).ShowDialog();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            PrepareWindowContent();
        }
    }
}




