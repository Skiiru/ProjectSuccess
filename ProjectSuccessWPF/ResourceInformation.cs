using net.sf.mpxj;
using System;
using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    class ResourceInformation
    {
        Resource resource;
        public List<Task> tasks;
        public string overtimeWorkDuration;
        public string workDuration;
        public float cost;

        public string resourceName;

        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            resourceName = resource.getName() ?? "Undefined";
            this.tasks = tasks ?? new List<Task>();
            cost = resource.getCost().floatValue();

            Duration baselineWork = resource.getBaselineWork();
            if (baselineWork != null)
            {
                double duration = 0;
                foreach (Task t in tasks)
                {
                    //Sometimes there is a null task
                    if (t != null)
                        duration += t.getWork().getDuration();
                }
                overtimeWorkDuration = (duration - baselineWork.getDuration()).ToString() + baselineWork.getUnits().toString();
            }
            else
                overtimeWorkDuration = "Undefined";
            workDuration = resource.getBaselineWork().toString() ?? "Undefined";
        }

        public ResourceInformation(Resource resource) : this(resource, new List<Task>())
        { }
    }
}
