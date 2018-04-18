using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class ChartSeriesCreator
    {
        public static void CreateTasksCostColumnSeries(List<TaskInformation> tasks, CartesianChart chart, string currencySymbol)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                collection.Add(new ColumnSeries
                {
                    Title = tasks[i].TaskName,
                    Values = new ChartValues<double> { tasks[i].Cost },
                });
            }
            chart.AxisX[0].Title = "Задачи";
            chart.AxisY[0].Title = "Цена, " + currencySymbol;
            chart.Series = collection;
        }

        public static void CreateTasksDurationColumnSeries(List<TaskInformation> tasks, CartesianChart chart, string taskDurationSymbol)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                collection.Add(new ColumnSeries
                {
                    Title = tasks[i].TaskName,
                    Values = new ChartValues<double> { tasks[i].Duration.TotalDuration() },
                });
            }
            chart.AxisX[0].Title = "Задачи";
            chart.AxisY[0].Title = "Продолжительность, " + taskDurationSymbol;
            chart.Series = collection;
        }

        public static void CreateResourcesCostPieSeries(List<ResourceInformation> resources, PieChart chart)
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
            chart.Series = collection;
        }

        public static void CreateResourcesWortimePieSeries(List<ResourceInformation> resources, PieChart chart)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < resources.Count; ++i)
            {
                collection.Add(new PieSeries
                {
                    Title = resources[i].ResourceName,
                    Values = new ChartValues<double> { resources[i].Duration.TotalDuration() },
                    DataLabels = true
                });
            }
            chart.Series = collection;
        }

        public static void CreateTasksOverworkCostCoulumnSeries(List<TaskInformation> tasks, CartesianChart chart, string currencySymbol)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                if (tasks[i].OverCost != 0)
                {
                    collection.Add(new ColumnSeries
                    {
                        Title = tasks[i].TaskName,
                        Values = new ChartValues<double> { tasks[i].OverCost },
                    });
                }
            }
            chart.AxisX[0].Title = "Задачи";
            chart.AxisY[0].Title = "Перерасход сресдств, " + currencySymbol;
            chart.Series = collection;
        }

        public static void CreateTasksOverworkDurationCoulumnSeries(List<TaskInformation> tasks, CartesianChart chart, string taskDurationSymbol)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < tasks.Count; ++i)
            {
                if (tasks[i].Duration.Overtime != 0)
                {
                    collection.Add(new ColumnSeries
                    {
                        Title = tasks[i].TaskName,
                        Values = new ChartValues<double> { tasks[i].Duration.Overtime },
                    });
                }
            }
            chart.AxisX[0].Title = "Задачи";
            chart.AxisY[0].Title = "Перерасход времени, " + taskDurationSymbol;
            chart.Series = collection;
        }

        public static void CreateResourceCostPerTimeUnitColumnSeries(List<ResourceInformation> resources, CartesianChart chart, string currencySymbol)
        {
            SeriesCollection collection = new SeriesCollection();
            for (int i = 0; i < resources.Count; ++i)
            {
                collection.Add(new ColumnSeries
                {
                    Title = resources[i].ResourceName,
                    Values = new ChartValues<double> { resources[i].CostPerTimeUnit },
                });
            }
            chart.AxisX[0].Title = "Ресурсы";
            chart.AxisY[0].Title = "Затраты за единицу времени / использования, " + currencySymbol;
            chart.Series = collection;
        }
    }
}
