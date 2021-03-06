﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    public class ProjectRate
    {
        /// <summary>
        /// Normal value less then 130%, can be from 0% to +inf, overcost is too much
        /// </summary>
        public double ProjectOverCostPercentage { get; private set; }

        public double ProjectOverCost { get; private set; }

        public double MeanTaskDurationRate { get; private set; }

        public double MeanTaskDuration { get; private set; }

        public double ProjectOvertimeRate { get; private set; }

        public double ProjectOvertime { get; private set; }

        public double RecourcesTotalOverworkTime { get; private set; }

        public int AnomalyTasksCount { get; private set; } //Anomaly types: finished but whithout finish date or percentage etc.

        public int DeviationTasksCount { get; private set; }

        double PreferTaskDuration;

        public ProjectRate(IProject project) : this(project.Tasks, project.Resources) { }

        public ProjectRate(List<TaskInformation> tasksWithoutHierarhy, List<ResourceInformation> recources)
        {
            double projectCost = 0;
            double projectOverCost = 0;
            double duration = 0;
            double baselineDuration = 0;
            double overtime = 0;
            AnomalyTasksCount = 0;
            DeviationTasksCount = 0;

            foreach (TaskInformation t in tasksWithoutHierarhy)
            {
                baselineDuration += t.Duration.Estimated;
                overtime += t.Duration.Overtime;
                projectCost += t.Cost;
                projectOverCost += t.OverCost;
                duration += t.Duration.TotalDuration();

                if (t.IsAnomaly)
                    AnomalyTasksCount++;

                if (t.HaveDeviation)
                    DeviationTasksCount++;
            }

            double recOvertime = 0;
            foreach (ResourceInformation r in recources)
            {
                recOvertime += r.Duration.Overtime;
            }
            RecourcesTotalOverworkTime = recOvertime;
            MeanTaskDuration = duration / tasksWithoutHierarhy.Count;
            MeanTaskDurationRate = 100 * MeanTaskDuration / PreferTaskDuration;
            ProjectOverCost = projectOverCost;
            ProjectOverCostPercentage = (projectOverCost / projectCost) * 100;
            ProjectOvertime = overtime;
            ProjectOvertimeRate = (overtime / baselineDuration) * 100;
            PreferTaskDuration =
                AppSettings.Settings.Default.PreferTaskDurationSource == "MEAN" ?
                MeanTaskDuration :
                AppSettings.Settings.Default.PreferTaskDuration;
        }

        /// <summary>
        /// Returns overcost rating string. If overcost less then 10 - perfect, less then 30% - its good, less then 50% - normal, in other cases - bad.
        /// </summary>
        /// <returns></returns>
        public string GetOvercostRateString()
        {
            string result = string.Empty;
            if (!double.IsNaN(ProjectOverCostPercentage))
            {
                if (ProjectOverCostPercentage <= -10)
                    result = "нормальное качество планирования, но большая часть бюджета не был освоена";
                else if (ProjectOverCostPercentage <= 0)
                    result = "хорошее качество планирования, перерасхрод отсутствует";
                else if (ProjectOverCostPercentage <= 10)
                    result = "отличное качество планирования, перерасход минимален";
                else if (ProjectOverCostPercentage <= 30)
                    result = "хорошее качество планирования, перерасход есть, но не слишком велик";
                else if (ProjectOverCostPercentage <= 50)
                    result = "нормальное качество планирования, прерасход есть, его объем как и в большинствен реальных проектов";
                else
                    result = "плохое качество планирования, перерасход вышел за пределы максимально допустимого";
            }
            return result;
        }

        public string GetMeanTaskDurationString()
        {
            string result = Environment.NewLine;
            if (MeanTaskDurationRate < 0)
                throw new ArgumentOutOfRangeException("MeanTaskduration", "Argument is less then zero.");
            else if (MeanTaskDurationRate <= 20)
                result = "плохое качество планирования, задачи слишком сильно раздроблены";
            else if (MeanTaskDurationRate <= 50)
                result = "хорошее качество планирования";
            else if (MeanTaskDurationRate <= 100)
                result = "нормальное качество планирования";
            else
                result = "плохое качество планирования, задачи необходим разделить на подзадачи";
            return result;
        }

        public string GetProjectOvertimeString()
        {
            string result = string.Empty;
            if (!double.IsNaN(ProjectOvertimeRate))
            {
                if (ProjectOvertimeRate <= -10)
                    result = "нормальное качество планирования, но большая часть бюджета не был освоена";
                else if (ProjectOvertimeRate <= 0)
                    result = "хорошее качество планирования, перерасхрод отсутствует";
                else if (ProjectOvertimeRate <= 10)
                    result = "отличное качество планирования, перерасход минимален";
                else if (ProjectOvertimeRate <= 30)
                    result = "хорошее качество планирования, перерасход есть, но не слишком велик";
                else if (ProjectOvertimeRate <= 50)
                    result = "нормальное качество планирования, прерасход есть, его объем как и в большинствен реальных проектов";
                else
                    result = "плохое качество планирования, перерасход вышел за пределы максимально допустимого";
            }
            return result;
        }
    }
}
