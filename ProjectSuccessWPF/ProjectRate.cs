using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class ProjectRate
    {
        /// <summary>
        /// Normal value less then 130%, can be from 0% to +inf, overcost is too much
        /// </summary>
        public double TasksOverCostPercentage { get; private set; }

        public double TasksOverCost { get; private set; }

        public double MeanTaskDurationRate { get; private set; }

        public double MeanTaskDuration { get; private set; }

        public double ProjectOvertimeRate { get; private set; }

        public double ProjectOvertime { get; private set; }

        public double RecourcesTotalOverworkTime { get; private set; }

        static int MAX_TASK_DURATION = 16;

        public ProjectRate(List<TaskInformation> tasksWithoutHierarhy, List<ResourceInformation> recources)
        {
            double projectCost = 0;
            double projectOverCost = 0;
            double duration = 0;
            double baselineDuration = 0;
            double overtime = 0;
            
            foreach (TaskInformation t in tasksWithoutHierarhy)
            {
                baselineDuration += t.BaselineDurationValue;
                overtime += t.OvertimeWorkValue;
                projectCost += t.Cost;
                projectOverCost += t.OverCost;
                duration += t.DurationValue;
            }
            double recOvertime = 0;
            foreach(ResourceInformation r in recources)
            {
                recOvertime += r.OvertimeWorkDurationValue;
            }
            RecourcesTotalOverworkTime = recOvertime;
            MeanTaskDuration = duration / tasksWithoutHierarhy.Count;
            MeanTaskDurationRate = 100 * MeanTaskDuration / MAX_TASK_DURATION;
            TasksOverCost = projectOverCost;
            TasksOverCostPercentage = (projectOverCost / projectCost) * 100;
            ProjectOvertime = overtime;
            ProjectOvertimeRate = (overtime / baselineDuration) * 100;
        }

        /// <summary>
        /// Returns overcost rating string. If overcost less then 10 - perfect, less then 30% - its good, less then 50% - normal, in other cases - bad.
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
