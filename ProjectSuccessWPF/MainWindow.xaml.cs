using LiveCharts;
using LiveCharts.Wpf;
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

        string currencySymbol;
        string resourcesWorktimeSymbol;
        string taskDurationSymbol;
        double hoursPerDay;

        //Files
        string mppFilePath;
        OpenFileDialog openFileDialog;
        string reportFilePath;
        SaveFileDialog saveFileDialog;

        //Charts
        public string[] TaskChartAxisXLabels { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            fileWorker = new MSProjectFileWorker();
            builder = new PDFBuilder();
            openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы MSProject|*.mpp"
            };

            saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF|*.pdf"
            };

            #region Testing
            fileWorker.ReadFile(@"C:\Users\Skiiru\Documents\Visual Studio 2017\Projects\ProjectSuccess\ProjectSuccessWPF\test2.mpp");
            projectAnalyzer = new MSProjectAnalyzer(fileWorker.projectFile);
            tasks = projectAnalyzer.GetTasksWithHierarhy();
            TreeViewItem firstItem = new TreeViewItem
            {
                Header = "Текущий проект"
            };
            CreateTreeViewItems(tasks, ref firstItem);
            TasksTreeView.Items.Add(firstItem);

            resources = projectAnalyzer.GetResources();

            builder.CreateReport(@"test.pdf", tasks, resources, projectAnalyzer.GetProjectProperties());
            #endregion

            hoursPerDay = projectAnalyzer.GetProjectProperties().getMinutesPerDay().doubleValue() / 60;
            ResourcesDataGrid.ItemsSource = resources;
            currencySymbol = projectAnalyzer.GetProjectProperties().getCurrencySymbol();
            string forTaskSymbol = projectAnalyzer.GetProjectProperties().getBaselineDuration().toString();
            taskDurationSymbol = forTaskSymbol[forTaskSymbol.Length - 1].ToString();
            string forResourcesSymbol = resources[0].WorkDuration;
            resourcesWorktimeSymbol = forResourcesSymbol[forResourcesSymbol.Length - 1].ToString();

            //Charts
            TaskCostChart.Series = CreateTasksCostCostColumnSeries();
            TasksDurationChart.Series = CreateTasksDurationCostColumnSeries();
            ResourcesCostPieChart.Series = CreateResourcesCostPieSeries();
            ResourcesWorktimePieChart.Series = CreateResourcesWortimePieSeries();

        }

        void CreateTreeViewItems(List<TaskInformation> tasks, ref TreeViewItem item)
        {
            foreach (TaskInformation taskInformation in tasks)
            {
                TreeViewItem treeViewItem = new TreeViewItem
                {
                    IsExpanded = true,
                    Header = "Задача \"" + taskInformation.TaskName + "\" (" + taskInformation.CompletePecrentage + "%)"
                };
                treeViewItem.Items.Add("Плановая продолжительность: " + taskInformation.BaselineDuration);
                if (!taskInformation.Duration.StartsWith("0.0"))
                    treeViewItem.Items.Add("Продолжительность: " + taskInformation.Duration);
                if (taskInformation.OvertimeWork != "0.0")
                    treeViewItem.Items.Add("Переработка: " + taskInformation.OvertimeWork);
                if (taskInformation.Cost != 0)
                    treeViewItem.Items.Add("Стоимость: " + taskInformation.Cost);
                if (taskInformation.OverCost != 0.0)
                    treeViewItem.Items.Add("Перерасход: " + taskInformation.OverCost);
                TreeViewItem resourcesItem = new TreeViewItem
                {
                    IsExpanded = true
                };
                if (taskInformation.Resources.Count != 0)
                {
                    resourcesItem.Header = "Ресурсы";
                    foreach (Resource resource in taskInformation.Resources)
                    {
                        resourcesItem.Items.Add(resource.getName());

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

        SeriesCollection CreateTasksCostCostColumnSeries()
        {
            List<TaskInformation> tasks = projectAnalyzer.GetTasksWithoutHierarhy();
            TaskChartAxisXLabels = new string[tasks.Count];
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                collection.Add(new ColumnSeries
                {
                    Title = tasks[i].TaskName,
                    Values = new ChartValues<double> { tasks[i].Cost },
                });
                TaskChartAxisXLabels[i] = tasks[i].TaskName;
            }
            TaskCostChart.AxisX[0].Title = "Задачи";
            TaskCostChart.AxisY[0].Title = "Цена, " + currencySymbol;
            TaskCostChart.AxisX[0].Labels = TaskChartAxisXLabels;
            return collection;
        }

        SeriesCollection CreateTasksDurationCostColumnSeries()
        {
            List<TaskInformation> tasks = projectAnalyzer.GetTasksWithoutHierarhy();
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                collection.Add(new ColumnSeries
                {
                    Title = tasks[i].TaskName,
                    Values = new ChartValues<double> { tasks[i].DurationValue },
                });
                TaskChartAxisXLabels[i] = tasks[i].TaskName;
            }
            TasksDurationChart.AxisX[0].Title = "Задачи";
            TasksDurationChart.AxisY[0].Title = "Продолжительность, " + taskDurationSymbol;
            return collection;
        }

        SeriesCollection CreateResourcesCostPieSeries()
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < resources.Count; ++i)
            {
                collection.Add(new PieSeries
                {
                    Title = resources[i].ResourceName,
                    Values = new ChartValues<double> { resources[i].Cost },
                    DataLabels = true
                });
            }
            return collection;
        }

        SeriesCollection CreateResourcesWortimePieSeries()
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < resources.Count; ++i)
            {
                collection.Add(new PieSeries
                {
                    Title = resources[i].ResourceName,
                    Values = new ChartValues<double> { resources[i].WorkDurationValue },
                    DataLabels = true
                });
            }
            return collection;
        }

        private void TasksChart_DataHover(object sender, ChartPoint chartPoint)
        {
        }
    }
}
