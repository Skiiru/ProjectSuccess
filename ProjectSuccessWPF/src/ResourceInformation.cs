using net.sf.mpxj;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    public class ResourceInformation
    {
        Resource resource;
        List<Task> tasks;
        public int ID { get; private set; }
        public string ResourceName { get; private set; }
        public string GroupName { get; private set; }
        public double CostPerTimeUnit { get; private set; }
        public double Cost { get; private set; }
        public WorkDuration Duration { get; private set; }
        public double OvertimeWorkCost { get; private set; }
        


        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            ID = resource.getID().intValue();
            ResourceName = resource.getName() ?? "Undefined";
            this.tasks = tasks ?? new List<Task>();
            Cost = resource.getCost().floatValue();
            GroupName = resource.getGroup();

            Duration baselineWork = resource.getBaselineWork();
            double duration = 0;
            if (baselineWork != null)
            {
                if (resource.getType().toString() == "Work")
                    foreach (Task t in tasks)
                        //Sometimes there is a null task
                        if (t != null && t.getBaselineDuration() != null)
                            duration += t.getDuration().getDuration() - t.getBaselineDuration().getDuration();
            }
            Duration = new WorkDuration(TimeUnitStringConverter.ConvertTime(resource.getBaselineWork().toString()), duration);
            CostPerTimeUnit = Convert.ToSingle(Cost / Duration.TotalDuration());
            OvertimeWorkCost = Duration.Overtime * CostPerTimeUnit;

        }

        public ResourceInformation(Resource resource) : this(resource, new List<Task>())
        { }

        //TODO: groups
        public ResourceInformation(Project project, User user, WorkDuration duration)
        {
            ResourceName = user.Login;
            ID = user.Id;
            Duration = duration;
            foreach(var cfield in user.CustomFields)
            {
                if (cfield.Name.ToLower() == "ставка" || cfield.Name.ToLower() == "salary")
                    try
                    {
                        CostPerTimeUnit = int.Parse(cfield.Values[0].Info);
                    }
                    catch
                    {
                        CostPerTimeUnit = 0;
                    }
            }
            Cost = CostPerTimeUnit * Duration.TotalDuration();
            OvertimeWorkCost = CostPerTimeUnit * Duration.Overtime;

        }
    }
}
