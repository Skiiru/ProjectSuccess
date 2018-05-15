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

        public static void CreateTasksCountLineSeries(List<TaskInformation> tasks, CartesianChart chart)
        {
            chart.AxisX.Clear();
            chart.AxisY.Clear();

            SeriesCollection collection = new SeriesCollection();
            List<LineSeries> trackersInwork = new List<LineSeries>();
            List<LineSeries> trackersClosed = new List<LineSeries>();
            //Add all tracker names and then count their tasks
            Dictionary<string, InWorkClosedCounter> trackersCount = new Dictionary<string, InWorkClosedCounter>();

            LineSeries inWorkTasks = new LineSeries
            {
                Title = "Открытые задачи",
                Values = new ChartValues<int>(),
                PointGeometry = null
            };
            LineSeries closedTasks = new LineSeries
            {
                Title = "Закрытые задачи",
                Values = new ChartValues<int>(),
                PointGeometry = null
            };
            List<string> labels = new List<string>();
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;

            for (int i = 0; i < tasks.Count; ++i)
            {
                if ((tasks[i].Tracker != "Undefined" && tasks[i].Tracker != string.Empty) && !trackersCount.ContainsKey(tasks[i].Tracker))
                    trackersCount.Add(tasks[i].Tracker, new InWorkClosedCounter());

                if (tasks[i].Dates.StartDate < startDate)
                    startDate = tasks[i].Dates.StartDate;
                if (tasks[i].Dates.FinishDate > endDate)
                    endDate = tasks[i].Dates.FinishDate;
            }

            foreach (var kvp in trackersCount)
            {
                trackersInwork.Add(new LineSeries
                {
                    Title = kvp.Key + "(в работе)",
                    Name = kvp.Key,
                    Values = new ChartValues<int>(),
                    PointGeometry = null
                });
                trackersClosed.Add(new LineSeries
                {
                    Title = kvp.Key + "(закрытые)",
                    Name = kvp.Key,
                    Values = new ChartValues<int>(),
                    PointGeometry = null
                });
            }

            InWorkClosedCounter allTasksCounter = new InWorkClosedCounter();

            for (DateTime i = startDate; i <= endDate.AddDays(AppSettings.Settings.Default.DaysInSprint); i = i.AddDays(AppSettings.Settings.Default.DaysInSprint))
            {
                //Reset count
                allTasksCounter.Clear();
                foreach (var tracker in trackersCount)
                    tracker.Value.Clear();

                labels.Add(i.ToShortDateString());
                foreach (TaskInformation t in tasks)
                {
                    //if (t.Dates.StartDate >= i)
                    //    if (t.Dates.FinishDate <= i)
                    //        inWork++;
                    //    else if (t.Status == TaskInformation.TaskStatus.Closed)
                    //        closed++;
                    //    else
                    //        inWork++;
                    if (t.Dates.StartDate <= i)
                    {
                        if (t.Status == TaskInformation.TaskStatus.Closed && t.Dates.FinishDate != null && t.Dates.FinishDate <= i)
                        {
                            allTasksCounter.ClosedCount++;
                            if (t.Tracker != "Undefined" && t.Tracker != string.Empty)
                                trackersCount[t.Tracker].ClosedCount++;
                        }
                        else
                        {
                            allTasksCounter.InWorkCount++;
                            if (t.Tracker != "Undefined" && t.Tracker != string.Empty)
                                trackersCount[t.Tracker].InWorkCount++;
                        }
                    }

                }
                //Count
                closedTasks.Values.Add(allTasksCounter.ClosedCount);
                inWorkTasks.Values.Add(allTasksCounter.InWorkCount);
                foreach (var tracker in trackersInwork)
                    tracker.Values.Add(trackersCount[tracker.Name].InWorkCount);
                foreach (var tracker in trackersClosed)
                    tracker.Values.Add(trackersCount[tracker.Name].ClosedCount);
            }

            //chart.AxisX[0].Title = "Время";
            //chart.AxisY[0].Title = "Кол-во задач";
            //chart.AxisX[0].ShowLabels = true;
            //chart.AxisX[0].Labels = labels;

            chart.AxisX.Add(new Axis() { Title = "Дата" });
            chart.AxisY.Add(new Axis() { Title = "Кол-во задач" });
            chart.AxisX[0].ShowLabels = true;
            chart.AxisX[0].Labels = labels;

            collection.Add(inWorkTasks);
            collection.Add(closedTasks);
            collection.AddRange(trackersInwork);
            collection.AddRange(trackersClosed);
            chart.Series = collection;
        }
    }
}
