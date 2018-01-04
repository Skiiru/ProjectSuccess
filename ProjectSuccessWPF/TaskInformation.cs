﻿using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class TaskInformation
    {
        Task task;
        public List<Resource> Resources { get; private set; }
        public List<TaskInformation> ChildTasks { get; private set; }

        static string ERRMSG_ISNOT_IN_BASELINE = "Этой задачи нет в базовом плане.";

        public string TaskName { get; private set; }

        //Dates
        public string StartDate { get; private set; }
        public string StartDateBaseline { get; private set; }
        public string FinishDate { get; private set; }
        public string FinishDateBaseline { get; private set; }
        public string Duration { get; private set; }


        public string OvertimeWork { get; private set; }
        public string BaselineDuration { get; private set; }
        public int Cost { get; private set; }
        public int ActualCost { get; private set; }
        public int RemainingCost { get; private set; }
        public float OverCost { get; private set; }
        public int CompletePecrentage { get; private set; }

        public TaskInformation(Task task, List<Resource> resources)
        {
            this.task = task;
            this.Resources = resources;
            ChildTasks = new List<TaskInformation>();

            //Dates
            StartDate = task.getStart().toString();
            FinishDate = task.getFinish().toString();
            if (task.getBaselineStart() != null && task.getBaselineFinish() != null)
            {
                StartDateBaseline = task.getBaselineStart().toString();
                FinishDateBaseline = task.getBaselineFinish().toString();
            }
            else
            {
                StartDateBaseline = FinishDateBaseline = ERRMSG_ISNOT_IN_BASELINE;
            }

            Duration baselineDuration = task.getBaselineDuration();
            if (baselineDuration != null)
            {
                Duration duration = task.getDuration();
                this.BaselineDuration = baselineDuration.toString();
                if (baselineDuration.getUnits() == duration.getUnits())
                    OvertimeWork = (duration.getDuration() - baselineDuration.getDuration()).ToString() + duration.getUnits().toString();
                else
                {
                    if (baselineDuration.compareTo(duration) > 0)
                        OvertimeWork = "Convertation problems. Plese make sure that all duration parameters have same time units.";
                    else
                        OvertimeWork = "0.0";
                }
            }
            else
                this.BaselineDuration = OvertimeWork = ERRMSG_ISNOT_IN_BASELINE;
            OverCost = task.getCost().floatValue() - task.getBaselineCost().floatValue();
            TaskName = task.getName();
            Cost = task.getCost().intValue();
            ActualCost = task.getActualCost().intValue();
            RemainingCost = task.getRemainingCost().intValue();
            this.Duration = task.getDuration().toString();
            CompletePecrentage = task.getPercentageComplete().intValue();
        }

        //Don't working when retun type is Task
        /// <summary>
        /// Returning child tasks as array
        /// </summary>
        /// <returns>Array of child tasks, return type - Task</returns>
        public object[] GetChildTasks()
        {
            return task.getChildTasks().toArray();
        }

        public override string ToString()
        {
            string result = TaskName + "(" + CompletePecrentage + "): D - " + Duration + ", OD - " + OvertimeWork + ", OC - " + OverCost;
            return base.ToString();
        }

        public string GetDurations()
        {
            string result = "плановая продолжительность - " + BaselineDuration + " (" + StartDate + " - " + FinishDate + ")";
            result += ", продолжительность - " + Duration;
            if (OvertimeWork != "0.0")
                result += ", переработка - " + OvertimeWork;
            return result;
        }

        public int SubTusksCount()
        {
            int count = 0;
            foreach (TaskInformation t in ChildTasks)
            {
                count++;
                count += t.SubTusksCount();
            }
            return count;
        }
    }
}
