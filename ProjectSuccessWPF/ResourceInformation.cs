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
        public string cost;

        public string resourceName;

        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            resourceName = resource.getName() ?? "Undefined";
            this.tasks = tasks ?? new List<Task>();
            cost = resource.getCost().toString() ?? "Undefined";
            overtimeWorkDuration = resource.getOvertimeWork().toString() ?? "Undefined";
            workDuration = resource.getWorkVariance().toString() ?? "Undefined";
        }

        public ResourceInformation(Resource resource) : this(resource, new List<Task>())
        { }
    }
}
