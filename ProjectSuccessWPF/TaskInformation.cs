using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class TaskInformation
    {
        Task task;
        public List<Resource> resources;
        public List<TaskInformation> childTasks;

        static string ERR_ISNOT_IN_BASELINE = "This task is not in baseline.";

        public string taskName;
        public string startDate;
        public string startDateBaseline;
        public string finishDate;
        public string finishDateBaseline;
        public string duration;
        public string overtimeWork;
        public string baselineDuration;
        public int cost;
        public int actualCost;
        public int remainingCost;
        public float overCost;
        public int completePecrentage;

        public TaskInformation(Task task, List<Resource> resources)
        {
            this.task = task;
            this.resources = resources;
            childTasks = new List<TaskInformation>();

            //Dates
            startDate = task.getStart().toString();
            finishDate = task.getFinish().toString();
            if (task.getBaselineStart() != null && task.getBaselineFinish() != null)
            {
                startDateBaseline = task.getBaselineStart().toString();
                finishDateBaseline = task.getBaselineFinish().toString();
            }
            else
            {
                startDateBaseline = finishDateBaseline = ERR_ISNOT_IN_BASELINE;
            }

            Duration baselineDuration = task.getBaselineDuration();
            if (baselineDuration != null)
            {
                Duration duration = task.getDuration();
                this.baselineDuration = baselineDuration.toString();
                if (baselineDuration.getUnits() == duration.getUnits())
                    overtimeWork = (duration.getDuration() - baselineDuration.getDuration()).ToString() + duration.getUnits().toString();
                else
                {
                    if (baselineDuration.compareTo(duration) > 0)
                        overtimeWork = "Convertation problems. Plese make sure that all duration parameters have same time units.";
                    else
                        overtimeWork = "0.0";
                }
            }
            else
                this.baselineDuration = overtimeWork = ERR_ISNOT_IN_BASELINE;
            overCost = task.getCost().floatValue() - task.getBaselineCost().floatValue();
            taskName = task.getName();
            cost = task.getCost().intValue();
            actualCost = task.getActualCost().intValue();
            remainingCost = task.getRemainingCost().intValue();
            this.duration = task.getDuration().toString();
            completePecrentage = task.getPercentageComplete().intValue();
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
            string result = taskName + "(" + completePecrentage + "): D - " + duration + ", OD - " + overtimeWork + ", OC - " + overCost;
            return base.ToString();
        }

        public string GetDurations()
        {
            string result = "плановая продолжительность - " + baselineDuration + " (" + startDate + " - " + finishDate + ")";
            result += ", продолжительность - " + duration;
            if (overtimeWork != "0.0")
                result += ", переработка - " + overtimeWork;
            return result;
        }

        public int SubTusksCount()
        {
            int count = 0;
            foreach (TaskInformation t in childTasks)
            {
                count++;
                count += t.SubTusksCount();
            }
            return count;
        }
    }
}
