using System.Collections.Generic;
using net.sf.mpxj;

namespace ProjectSuccessWPF.src.MSProjectSrc
{
    class MSProjectProject : IProject
    {
        public string ProjectName { get; private set; }

        public int ProjectId { get; private set; }

        public List<TaskInformation> Tasks { get; private set; }

        public List<ResourceInformation> Resources { get; private set; }

        public ProjectRate Rate { get; private set; }

        public MSProjectProject(ProjectFile project)
        {

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
