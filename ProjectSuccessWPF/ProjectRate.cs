﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class ProjectRate
    {
        /// <summary>
        /// Normal value less then 30%, can be from -100% to +inf, overcost is too much
        /// </summary>
        public double TasksOverCostPercentage { get; private set; }

        public double MeanTaskDuration { get; private set; }

        public ProjectRate(List<TaskInformation> tasksWithoutHierarhy, List<ResourceInformation> recources)
        {
            double projectCost = 0;
            double projectOverCost = 0;
            double duration = 0;

            foreach (TaskInformation t in tasksWithoutHierarhy)
            {
                projectCost += t.Cost;
                projectOverCost += t.OverCost;
                duration += t.DurationValue;
            }
            MeanTaskDuration = duration / tasksWithoutHierarhy.Count;
            TasksOverCostPercentage = projectOverCost / projectCost * 100;
        }

        /// <summary>
        /// Returns overcost rating string. If overcost less then 10 - perfect, less then 30% - its good, less then 60% - normal, in other cases - bad.
        /// </summary>
        /// <returns></returns>
        public string GetOvercostRateString()
        {
            string result = string.Empty;
            if (!double.IsNaN(TasksOverCostPercentage))
            {
                if (TasksOverCostPercentage <= -10)
                    result = "нормальное качество планирования, но большая часть бюджета не был освоена";
                else if (TasksOverCostPercentage <= 0)
                    result = "хорошее качество планирования, перерасхрод отсутствует";
                else if (TasksOverCostPercentage <= 10)
                    result = "отличное качество планирования, перерасход минимален";
                else if (TasksOverCostPercentage <= 30)
                    result = "хорошее качество планирования, перерасход есть, но не слишком велик";
                else if (TasksOverCostPercentage <= 50)
                    result = "нормальное качество планирования, прерасход есть, его объем как и в большинствен реальных проектов";
                else
                    result = "плохое качество планирования, перерасход вышел за пределы максимально допустимого";
            }
            return result;
        }

        public string GetMeanTaskDurationString()
        {
            string result = Environment.NewLine;
            if (MeanTaskDuration < 0)
                throw new ArgumentOutOfRangeException("MeanTaskduration", "Argument is less then zero.");
            else if (MeanTaskDuration <= 2)
                result = "плохое качество планирования, задачи слишком сильно раздроблены";
            else if (MeanTaskDuration <= 8)
                result = "хорошее качество планирования";
            else if (MeanTaskDuration <= 16)
                result = "нормальное качество планирования";
            else
                result = "плохое качество планирования, задачи необходим разделить на подзадачи";
            return result;
        }
    }
}
