using System.Collections.Generic;


namespace ProjectSuccessWPF
{
    public interface IProject
    {
        string ProjectName { get; }
        int ProjectId { get; }
        List<TaskInformation> Tasks { get; }
        List<ResourceInformation> Resources { get; }
        ProjectRate Rate { get; }

        void AddTask(TaskInformation t);
        void AddResource(ResourceInformation r);
    }
}
