using System.Collections.Generic;
using net.sf.mpxj;

namespace ProjectSuccessWPF
{
    class MSProjectProject : IProject
    {
        public string ProjectName { get; private set; }

        public int ProjectId { get; private set; }

        public List<TaskInformation> Tasks { get; private set; }

        public List<ResourceInformation> Resources { get; private set; }

        public ProjectRate Rate { get; private set; }

        public string Status { get; private set; }

        public MSProjectProject(MSProjectFileParser parser)
        {
            Tasks = parser.GetTasksWithoutHierarhy();
            Resources = parser.GetResources();
            ProjectName = parser.GetProjectProperties().getName();
            Status = parser.GetProjectProperties().getContentStatus();
            //Only one project in file
            ProjectId = -1;
            Rate = new ProjectRate(this);

        }

        public void AddResource(ResourceInformation r)
        {
            Resources.Add(r);
        }

        public void AddTask(TaskInformation t)
        {
            Tasks.Add(t);
        }
    }
}
