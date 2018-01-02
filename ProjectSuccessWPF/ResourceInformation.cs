using net.sf.mpxj;
using System;
using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    class ResourceInformation
    {
        public Resource resource;
        public List<Task> tasks;
        public Duration overtimeWorkDuration;
        public Duration workDuration;

        public ResourceInformation(Resource resource, List<Task> tasks)
        {
            this.resource = resource;
            this.tasks = tasks;
            overtimeWorkDuration = resource.getActualOvertimeWork() == null ? resource.getOvertimeWork() : resource.getActualOvertimeWork();
            workDuration = resource.getActualWork() == null ? resource.getWork() : resource.getActualWork();
        }

        public ResourceInformation(Resource resource)
        {
            new ResourceInformation(resource, new List<Task>());
        }

        public Duration GetConvertedWorkDuration(TimeUnit type, ProjectProperties properties)
        {
            return workDuration.convertUnits(type, properties);
        }

        public Duration GetConvertedWorkDuration(ProjectProperties properties)
        {
            return GetConvertedWorkDuration(TimeUnit.HOURS, properties);
        }

        public Duration GetConvertedOverworkDuration(TimeUnit type, ProjectProperties props)
        {
            return overtimeWorkDuration.convertUnits(type, props);
        }

        public Duration GetConvertedOverworkDuration(ProjectProperties props)
        {
            return overtimeWorkDuration.convertUnits(TimeUnit.HOURS, props);
        }
    }
}
