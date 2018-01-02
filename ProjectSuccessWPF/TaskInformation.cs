using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class TaskInformation
    {
        public Task task;
        public List<Resource> resources;
        public Duration duration;
        public Duration overWorkDuration;
        public List<TaskInformation> childTasks;

        public TaskInformation(Task task, List<Resource> resources)
        {
            this.task = task;
            this.resources = resources;
            duration = task.getActualDuration() == null? task.getDuration():task.getActualDuration();
            overWorkDuration = task.getActualOvertimeWork() == null ? task.getOvertimeWork() : task.getActualOvertimeWork();
            childTasks = new List<TaskInformation>();
        }

        public Duration GetConvertedDuration(TimeUnit type, ProjectProperties properties)
        {
            return duration.convertUnits(type, properties);
        }

        public Duration GetConvertedDuration(ProjectProperties properties)
        {
            return GetConvertedDuration(TimeUnit.HOURS, properties);
        }

        public Duration GetConvertedOverwork(TimeUnit type, ProjectProperties props)
        {
            return overWorkDuration.convertUnits(type, props);
        }

        public Duration GetConvertedOverwork(ProjectProperties props)
        {
            return overWorkDuration.convertUnits(TimeUnit.HOURS, props);
        }
    }
}
