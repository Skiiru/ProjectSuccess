using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.IO;

namespace ProjectSuccessWPF
{
    class WinFormsChartCreator
    {
        public static List<ChartContainer> GetCharts(List<TaskInformation> taskWithourHierarhy, List<ResourceInformation> resources)
        {
            string areaName = "Default";
            int width = 500;
            int height = 400;

            List<ChartContainer> result = new List<ChartContainer>();
            Chart tasksCostChart = new Chart
            {
                Width = width,
                Height = height
            };
            tasksCostChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() {Interval = 1} });
            List<string> taskCostChartAxisX = new List<string>();
            List<double> tasksCostChartAxisY = new List<double>();

            Chart tasksOvercostChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            tasksOvercostChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<string> tasksOvercostChartAxisX = new List<string>();
            List<double> tasksOvercostChartAxisY = new List<double>();

            Chart tasksDurationChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            tasksDurationChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<string> taskDurationChartAxisX = new List<string>();
            List<double> tasksDurationChartAxisY = new List<double>();

            Chart tasksOverworkDurationChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            tasksOverworkDurationChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<string> taskOverworkDurationChartAxisX = new List<string>();
            List<double> tasksOverworkDurationChartAxisY = new List<double>();

            tasksCostChart.Series.Add(new Series() { Name = "Стоимость задач", ChartArea = areaName, ChartType = SeriesChartType.Column });
            tasksOvercostChart.Series.Add(new Series() { Name = "Перерасход средств на задачи", ChartArea = areaName, ChartType = SeriesChartType.Column });
            tasksDurationChart.Series.Add(new Series() { Name = "Продолжительность задач", ChartArea = areaName, ChartType = SeriesChartType.Column });
            tasksOverworkDurationChart.Series.Add(new Series() { Name = "Отклонение по времени выполнения задач", ChartArea = areaName, ChartType = SeriesChartType.Column });

            foreach (TaskInformation t in taskWithourHierarhy)
            {
                taskCostChartAxisX.Add(t.TaskName);
                tasksCostChartAxisY.Add(t.Cost);

                if (t.OverCost != 0)
                {
                    tasksOvercostChartAxisX.Add(t.TaskName);
                    tasksOvercostChartAxisY.Add(t.OverCost);
                }

                taskDurationChartAxisX.Add(t.TaskName);
                tasksDurationChartAxisY.Add(t.DurationValue);

                if (t.OvertimeWorkValue != 0)
                {
                    taskOverworkDurationChartAxisX.Add(t.TaskName);
                    tasksOverworkDurationChartAxisY.Add(t.OvertimeWorkValue);
                }
            }
            tasksCostChart.Series[0].Points.DataBindXY(taskCostChartAxisX, tasksCostChartAxisY);
            tasksOvercostChart.Series[0].Points.DataBindXY(tasksOvercostChartAxisX, tasksOvercostChartAxisY);
            tasksDurationChart.Series[0].Points.DataBindXY(taskDurationChartAxisX, tasksDurationChartAxisY);
            tasksOverworkDurationChart.Series[0].Points.DataBindXY(taskOverworkDurationChartAxisX, tasksOverworkDurationChartAxisY);


            Chart resourcesCostChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            resourcesCostChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<double> resourcesCostList = new List<double>();
            Chart resourcesWorktimeChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            resourcesWorktimeChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<double> resourcesWorktimeList = new List<double>();
            Chart resourcesCostPerUseChart = new Chart
            {
                Width = width,
                Height = height
            }; ;
            resourcesCostPerUseChart.ChartAreas.Add(new ChartArea() { Name = areaName, AxisX = new Axis() { Interval = 1 } });
            List<double> resourcesCostPerUseList = new List<double>();

            List<string> resNames = new List<string>();

            resourcesCostChart.Series.Add(new Series() { Name = "Затраты на ресурсы", ChartArea = areaName, ChartType = SeriesChartType.Pie });
            resourcesWorktimeChart.Series.Add(new Series() { Name = "Время работы ресурсов", ChartArea = areaName, ChartType = SeriesChartType.Pie });
            resourcesCostPerUseChart.Series.Add(new Series() { Name = @"Затраты на ресурсы за одно использование / одну единицу времени", ChartArea = areaName, ChartType = SeriesChartType.Pie });

            foreach (ResourceInformation r in resources)
            {
                resNames.Add(r.ResourceName);
                resourcesCostList.Add(r.Cost);
                resourcesWorktimeList.Add(r.WorkDurationValue);
                resourcesCostPerUseList.Add(r.CostPerTimeUnit);
            }
            resourcesCostChart.Series[0].Points.DataBindXY(resNames, resourcesCostList);
            resourcesWorktimeChart.Series[0].Points.DataBindXY(resNames, resourcesWorktimeList);
            resourcesCostPerUseChart.Series[0].Points.DataBindXY(resNames, resourcesCostPerUseList);

            //Tasks
            MemoryStream ms = new MemoryStream();
            tasksCostChart.Update();
            tasksCostChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(tasksCostChart.Series[0].Name, new Bitmap(ms)));

            ms = new MemoryStream();
            tasksDurationChart.Update();
            tasksDurationChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(tasksDurationChart.Series[0].Name, new Bitmap(ms)));

            ms = new MemoryStream();
            tasksOvercostChart.Update();
            tasksOvercostChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(tasksOvercostChart.Series[0].Name, new Bitmap(ms)));

            ms = new MemoryStream();
            tasksOverworkDurationChart.Update();
            tasksOverworkDurationChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(tasksOverworkDurationChart.Series[0].Name, new Bitmap(ms)));

            //Resources
            ms = new MemoryStream();
            resourcesCostChart.Update();
            resourcesCostChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(resourcesCostChart.Series[0].Name, new Bitmap(ms)));

            ms = new MemoryStream();
            resourcesCostPerUseChart.Update();
            resourcesCostPerUseChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(resourcesCostPerUseChart.Series[0].Name, new Bitmap(ms)));

            ms = new MemoryStream();
            resourcesWorktimeChart.Update();
            resourcesWorktimeChart.SaveImage(ms, ChartImageFormat.Bmp);
            result.Add(new ChartContainer(resourcesWorktimeChart.Series[0].Name, new Bitmap(ms)));

            return result;
        }
    }
}