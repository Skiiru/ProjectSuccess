using net.sf.mpxj;
using Redmine.Net.Api.Types;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class TaskInformation
    {
        Task task;
        public List<TaskInformation> ChildTasks { get; private set; }

        static string ERRMSG_ISNOT_IN_BASELINE = "Этой задачи нет в базовом плане.";

        public string TaskName { get; private set; }
        int ID;

        public List<ResourceInformation> Resources { get; private set; }

        //Dates
        public string StartDate { get; private set; }
        public string StartDateBaseline { get; private set; }
        public string FinishDate { get; private set; }
        public string FinishDateBaseline { get; private set; }

        public WorkDuration Duration { get; private set; }

        //Cost
        public int Cost { get; private set; }
        public int ActualCost { get; private set; }
        public int RemainingCost { get; private set; }
        public double OverCost { get; private set; }

        public int CompletePercentage { get; private set; }

        public TaskInformation(Task task, List<Resource> resources)
        {
            Resources = new List<ResourceInformation>();
            this.task = task;
            foreach (Resource res in resources)
                this.Resources.Add(new ResourceInformation(res));
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

            if (task.getBaselineDuration() != null && task.getDuration() != null)
            {
                Duration = new WorkDuration(
                    TimeUnitStringConverter.ConvertTime(task.getBaselineDuration().toString()), 
                    TimeUnitStringConverter.ConvertTime(task.getDuration().toString()));
            }
            else
                Duration = new WorkDuration();


            OverCost = task.getCost().doubleValue() - task.getBaselineCost().doubleValue();
            TaskName = task.getName();
            Cost = task.getCost().intValue();
            ActualCost = task.getActualCost().intValue();
            RemainingCost = task.getRemainingCost().intValue();
            CompletePercentage = task.getPercentageComplete().intValue();
        }

        public TaskInformation(Issue issue, ResourceInformation assignedTo)
        {
            TaskName = issue.Subject;
            ID = issue.Id;

            double est = 0, spent = 0;
            if (issue.EstimatedHours.HasValue)
                est = issue.EstimatedHours.Value;
            if (issue.SpentHours.HasValue)
                spent = issue.SpentHours.Value;
            Duration = new WorkDuration(est, spent);
            CompletePercentage = int.Parse(issue.DoneRatio.Value.ToString());
            if (CompletePercentage == 100 && Duration.Spent == 0)
            {
                Duration.Spent = Duration.Estimated;
                Duration.ReCalculateOvertime();
            }
            StartDateBaseline = StartDate = issue.StartDate == null ? issue.StartDate.ToString() : "Unknown";
            FinishDateBaseline = FinishDate = issue.DueDate == null ? issue.DueDate.ToString() : "Unknown";

            //Costs
            double costPerHour = assignedTo.CostPerTimeUnit;
            Cost = (int)(costPerHour * Duration.TotalDuration());
            ActualCost = (int)(costPerHour * Duration.Spent);
            if (CompletePercentage == 100)
                RemainingCost = 0;
            else
                RemainingCost = (int)(costPerHour * (Duration.Estimated - Duration.Spent));
            OverCost = (int)(costPerHour * Duration.Overtime);

            ChildTasks = new List<TaskInformation>();

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
            string result = TaskName + "(" + CompletePercentage + "): D - " + Duration + ", OD - " + Duration.Overtime+ ", OC - " + OverCost;
            return base.ToString();
        }

        public string GetDurations()
        {
            string result = "плановая продолжительность - " + Duration.Estimated+ " (" + StartDate + " - " + FinishDate + ")";
            result += ", продолжительность - " + Duration;
            if (Duration.Overtime!= 0)
                result += ", переработка - " + Duration.Overtime;
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
