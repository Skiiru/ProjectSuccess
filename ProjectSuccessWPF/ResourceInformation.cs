using net.sf.mpxj;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    class ResourceInformation
    {
        Resource resource;
        List<Task> tasks;
        int ID;
        public string ResourceName { get; private set; }
        public string GroupName { get; private set; }
        public double CostPerTimeUnit { get; private set; }
        public double Cost { get; private set; }
        public string WorkDuration { get; private set; }
        public double WorkDurationValue { get; private set; }
        public string OvertimeWorkDuration { get; private set; }
        public double OvertimeWorkDurationValue { get; private set; }
        public double OvertimeWorkCost { get; private set; }


        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            ID = resource.getID().intValue();
            ResourceName = resource.getName() ?? "Undefined";
            this.tasks = tasks ?? new List<Task>();
            Cost = resource.getCost().floatValue();
            WorkDuration = resource.getBaselineWork().toString();
            WorkDurationValue = TimeUnitStringConverter.ConvertTime(WorkDuration);
            CostPerTimeUnit = Convert.ToSingle(Cost / Convert.ToDouble(WorkDuration.Remove(WorkDuration.Length - 3)));
            GroupName = resource.getGroup();

            Duration baselineWork = resource.getBaselineWork();
            if (baselineWork != null)
            {
                double duration = 0;
                if (resource.getType().toString() == "Work")
                    foreach (Task t in tasks)
                        //Sometimes there is a null task
                        if (t != null && t.getBaselineDuration() != null)
                            duration += t.getDuration().getDuration() - t.getBaselineDuration().getDuration();
                OvertimeWorkDurationValue = duration * 8;
                OvertimeWorkDuration = OvertimeWorkDurationValue + baselineWork.getUnits().toString();
                OvertimeWorkCost = OvertimeWorkDurationValue * CostPerTimeUnit;
            }
            else
                OvertimeWorkDuration = "Undefined";
        }

        public ResourceInformation(Resource resource) : this(resource, new List<Task>())
        { }

        public ResourceInformation(Project project, User user)
        {
            ResourceName = user.FirstName + " " + user.LastName;
            ID = user.Id;
        }
    }
}
