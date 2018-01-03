using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class TaskInformation
    {
        Task task;
        public List<Resource> resources;
        public List<TaskInformation> childTasks;

        public string taskName;
        public string duration;
        public string overtimeWork;
        public int cost;
        public float overCost;
        public int completePecrentage;

        public TaskInformation(Task task, List<Resource> resources)
        {
            this.task = task;
            this.resources = resources;
            childTasks = new List<TaskInformation>();

            taskName = task.getName();
            cost = task.getCost().intValue();
            //duration = task.getDuration().toString();
            completePecrentage = task.getPercentageComplete().intValue();
            if (task.getOvertimeWork() != null)
                overtimeWork = task.getOvertimeWork().toString();
            else
                overtimeWork = "0.0";
            if (task.getOvertimeCost() != null)
                overCost = task.getOvertimeCost().floatValue();
            else
                overCost = 0.0f;
        }

        //Don't working when retun type is Task
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
            string result = string.Empty;
            result = "продолжительность - " + duration;
            if (overtimeWork != "0.0")
                result += ", переработка - " + overtimeWork;
            return result;
        }

        public int SubTusksCount()
        {
            int count = 0;
            foreach(TaskInformation t in childTasks)
            {
                count++;
                count += t.SubTusksCount();
            }
            return count;
        }
    }
}
