using net.sf.mpxj;
using System;
using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    class ResourceInformation
    {
        Resource resource;
        List<Task> tasks;
        public string ResourceName { get; private set; }
        public string GroupName { get; private set; }
        public float CostPerTimeUnit { get; private set; }
        public float Cost { get; private set; }
        public string WorkDuration { get; private set; }
        public string OvertimeWorkDuration { get; private set; }
        public float OvertimeWorkCost { get; private set; }


        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            ResourceName = resource.getName() ?? "Undefined";
            this.tasks = tasks ?? new List<Task>();
            Cost = resource.getCost().floatValue();
            WorkDuration = resource.getBaselineWork().toString();
            CostPerTimeUnit = Convert.ToSingle(Cost / Convert.ToDouble(WorkDuration.Remove(WorkDuration.Length - 3)));
            GroupName = resource.getGroup();

            Duration baselineWork = resource.getBaselineWork();
            if (baselineWork != null)
            {
                double duration = 0;
                foreach (Task t in tasks)
                {
                    //Sometimes there is a null task
                    if (t != null)
                    {
                        duration += t.getWork().getDuration();
                    }
                }
                OvertimeWorkDuration = (duration - baselineWork.getDuration()).ToString() + baselineWork.getUnits().toString();
                OvertimeWorkCost = float.Parse(OvertimeWorkDuration.Remove(OvertimeWorkDuration.Length - 1)) * CostPerTimeUnit;
            }
            else
                OvertimeWorkDuration = "Undefined";
        }

        public ResourceInformation(Resource resource) : this(resource, new List<Task>())
        { }
    }
}
