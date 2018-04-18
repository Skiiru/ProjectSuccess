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
    public partial class MainWindow : Window
    {
        MSProjectFileWorker fileWorker;
        MSProjectFileParser projectAnalyzer;
        PDFBuilder builder;
        ProjectRate rate;

        IEnumerable<IProject> Projects { get; set; }

        List<TaskInformation> tasks;
        List<ResourceInformation> resources;
        List<ChartContainer> charts;
        string currencySymbol;
        string resourcesWorktimeSymbol;
        string taskDurationSymbol;
        double hoursPerDay;

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
            charts = new List<ChartContainer>();
            openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы MSProject|*.mpp"
            };

            saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF|*.pdf"
            };

            RedmineWorker redmine = new RedmineWorker();
            Projects = redmine.LoadProjects();
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
                if (taskInformation.Duration.Overtime !=0 )
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

        void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        bool OpenFile()
        {
            bool result = openFileDialog.ShowDialog() == true ? true : false;
            if (result)
                if (openFileDialog.FileName != null && openFileDialog.FileName != string.Empty)
                {
                    mppFilePath = openFileDialog.FileName;
                }
                else
                    ShowError("Ошибка при открытии файла.");
            return result;
        }

        bool SaveFile()
        {
            bool result = saveFileDialog.ShowDialog() == true ? true : false;
            if (result)
                if (saveFileDialog.FileName != null && saveFileDialog.FileName != string.Empty)
                    reportFilePath = saveFileDialog.FileName;
                else
                    ShowError("Ошибка при сохранении файла.");
            return result;
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

        private void OpenFilMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OpenFile())
            {
                fileWorker.ReadFile(mppFilePath);
                projectAnalyzer = new MSProjectFileParser(fileWorker.ProjectFile);
                tasks = projectAnalyzer.GetTasksWithHierarhy();
                resources = projectAnalyzer.GetResources();

                rate = new ProjectRate(projectAnalyzer.GetTasksWithoutHierarhy(), resources);

                //Tree view
                TasksTreeView.Items.Clear();
                TreeViewItem firstItem = new TreeViewItem
                {
                    Header = "Текущий проект"
                };

                if (rate != null)
                {
                    if (!double.IsNaN(rate.ProjectOverCostPercentage))
                    {
                        string s = "Оценка перерасхода средств: " + Math.Round(rate.ProjectOverCostPercentage, 2) + "%, " + rate.GetOvercostRateString() + ".";
                        firstItem.Items.Add(s);
                    }

                    if (!double.IsNaN(rate.MeanTaskDurationRate))
                    {
                        string s = "Оценка средней продолжительность задач: " + Math.Round(rate.MeanTaskDurationRate, 2) + "%, " + rate.GetMeanTaskDurationString() + '.';
                        firstItem.Items.Add(s);
                    }

                    if(!double.IsNaN(rate.ProjectOvertimeRate))
                    {
                        string s = "Оценка перерасхода времени: " + Math.Round(rate.ProjectOvertimeRate, 2) + "%, " + rate.GetProjectOvertimeString() + '.';
                        firstItem.Items.Add(s);
                    }

                    if (!double.IsNaN(rate.RecourcesTotalOverworkTime))
                    {
                        string s = "Общее время переработки ресурсов: " + Math.Round(rate.RecourcesTotalOverworkTime, 2) + "ч.";
                        firstItem.Items.Add(s);
                    }
                }

                CreateTreeViewItems(tasks, ref firstItem);
                TasksTreeView.Items.Add(firstItem);

                //Data grid
                ResourcesDataGrid.ItemsSource = resources;

                //Symbols
                hoursPerDay = projectAnalyzer.GetProjectProperties().getMinutesPerDay().doubleValue() / 60;
                currencySymbol = projectAnalyzer.GetProjectProperties().getCurrencySymbol();
                string forTaskSymbol = projectAnalyzer.GetProjectProperties().getBaselineDuration().toString();
                taskDurationSymbol = forTaskSymbol[forTaskSymbol.Length - 1].ToString();
                string forResourcesSymbol = "h";
                resourcesWorktimeSymbol = forResourcesSymbol[forResourcesSymbol.Length - 1].ToString();

                //Charts
                var tasksWithoutH = projectAnalyzer.GetTasksWithoutHierarhy();
                ChartSeriesCreator.CreateTasksCostColumnSeries(tasksWithoutH, TaskCostChart, currencySymbol);
                ChartSeriesCreator.CreateTasksDurationColumnSeries(tasksWithoutH, TasksDurationChart, taskDurationSymbol);
                ChartSeriesCreator.CreateTasksOverworkDurationCoulumnSeries(tasksWithoutH, TasksOverworkDurationChart, taskDurationSymbol);
                ChartSeriesCreator.CreateTasksOverworkCostCoulumnSeries(tasksWithoutH, TasksOverworkCostChart, currencySymbol);
                ChartSeriesCreator.CreateResourcesCostPieSeries(resources, ResourcesCostPieChart);
                ChartSeriesCreator.CreateResourcesWortimePieSeries(resources, ResourcesWorktimePieChart);
                ChartSeriesCreator.CreateResourceCostPerTimeUnitColumnSeries(resources, ResourcesCostPerUseChart, currencySymbol);

                //Enable buttons
                CreateReportMenuItem.IsEnabled = true;
            }
        }

        private void CreateReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SaveFile())
            {
                charts.Clear();
                charts = WinFormsChartCreator.GetCharts(projectAnalyzer.GetTasksWithoutHierarhy(), resources);
                builder.CreateReport(reportFilePath, tasks, resources, projectAnalyzer.GetProjectProperties(), charts, rate);
                (new ReportSavedWindow(reportFilePath)).ShowDialog();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}




