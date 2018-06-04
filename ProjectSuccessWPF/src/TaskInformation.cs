using net.sf.mpxj;
using Redmine.Net.Api.Types;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    public class TaskInformation
    {
        Task task;
        public List<TaskInformation> ChildTasks { get; private set; }

        static string ERRMSG_ISNOT_IN_BASELINE = "Этой задачи нет в базовом плане.";

        public enum TaskStatus { InWork, Closed };

        public string TaskName { get; private set; }
        int ID;
        public string Tracker { get; private set; }
        public TaskStatus Status { get; private set; }

        public List<ResourceInformation> Resources { get; private set; }

        public ProjectDates Dates { get; private set; }

        public WorkDuration Duration { get; private set; }

        //Cost
        public int Cost { get; private set; }
        public int ActualCost { get; private set; }
        public int RemainingCost { get; private set; }
        public double OverCost { get; private set; }

        public int CompletePercentage { get; private set; }

        public bool IsAnomaly { get; private set; }
        public bool HaveDeviation { get; private set; }

        public TaskInformation(Task task, List<Resource> resources)
        {
            Resources = new List<ResourceInformation>();
            this.task = task;
            foreach (Resource res in resources)
                this.Resources.Add(new ResourceInformation(res));
            ChildTasks = new List<TaskInformation>();

            //Dates
            Dates = new ProjectDates(task.getStart(), task.getFinish());
            if (task.getBaselineStart() != null && task.getBaselineFinish() != null)
            {
                Dates.SetBaseline(task.getBaselineStart(), task.getBaselineFinish());
            }
            else
            {
                //TODO: ERR_MSG_NOT_NIN_BASELINE
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
            if (CompletePercentage == 100)
                Status = TaskStatus.Closed;
            else
                Status = TaskStatus.InWork;
            Tracker = "Undefined";
            Dates.SetCreatedDate(task.getCreateDate());
            SetAnomaly();
            SetDeviation();
        }

        public TaskInformation(Issue issue, ResourceInformation assignedTo)
        {
            TaskName = issue.Subject;
            ID = issue.Id;
            Tracker = issue.Tracker.Name;

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
            Dates = new ProjectDates(issue.StartDate, issue.DueDate);

            Dates.SetCreatedDate(issue.CreatedOn);

            //Costs
            double costPerHour = assignedTo.CostPerTimeUnit;
            Cost = (int)(costPerHour * Duration.TotalDuration());
            ActualCost = (int)(costPerHour * Duration.Spent);
            if (CompletePercentage == 100)
                RemainingCost = 0;
            else
                RemainingCost = (int)(costPerHour * (Duration.Estimated - Duration.Spent));
            OverCost = (int)(costPerHour * Duration.Overtime);

            //TODO: issue status
            if (CompletePercentage == 100)
                Status = TaskStatus.Closed;
            else
                Status = TaskStatus.InWork;


            ChildTasks = new List<TaskInformation>();
            SetAnomaly();
            SetDeviation();

        }

        //TODO: remove
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
            string result = "плановая продолжительность - " + Duration.Estimated+ " (" + Dates.StartDate + " - " + Dates.FinishDate + ")";
            result += ", продолжительность - " + Duration;
            if (Duration.Overtime!= 0)
                result += ", переработка - " + Duration.Overtime;
            return result;
        }

        private void SetAnomaly()
        {
            IsAnomaly = (Status == TaskStatus.Closed && (CompletePercentage != 100 ^ Dates.FinishDate != null)) ;
        }

        private void SetDeviation()
        {
            HaveDeviation = Duration.Overtime != 0;
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
