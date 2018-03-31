using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api.Types;


namespace ProjectSuccessWPF.Redmine
{
    class RedmineProject : IProject
    {
        public string ProjectName { get; private set; }
        public int ProjectId { get; private set; }
        public List<TaskInformation> Tasks { get; private set; }
        public List<ResourceInformation> Resources { get; private set; }
        public List<string> Trackers { get; private set; }
        public ProjectRate Rate { get; private set; }

        public RedmineProject(Project project, List<Issue> issues, List<User> users)
        {
            ProjectName = project.Name;
            ProjectId = project.Id;
            Tasks = new List<TaskInformation>();
            Resources = new List<ResourceInformation>();

            NameValueCollection parameters = new NameValueCollection();
            Dictionary<int, WorkDuration> usersWorkDuration = new Dictionary<int, WorkDuration>();


            //Users
            foreach (Issue issue in issues)
            {
                if (issue.Project.Id == ProjectId)
                {
                    if (!usersWorkDuration.ContainsKey(issue.AssignedTo.Id))
                    {
                        usersWorkDuration.Add(issue.AssignedTo.Id, new WorkDuration());
                    }
                    else
                    {
                        if (issue.EstimatedHours.HasValue)
                            usersWorkDuration[issue.AssignedTo.Id].Estimated += issue.EstimatedHours.Value;
                        if (issue.SpentHours.HasValue)
                            usersWorkDuration[issue.AssignedTo.Id].Spent += issue.SpentHours.Value;
                    }
                }
            }

            foreach (KeyValuePair<int, WorkDuration> kvp in usersWorkDuration)
            {
                Resources.Add(new ResourceInformation(project, users.Find(x => x.Id == kvp.Key), kvp.Value));
            }

            foreach (Issue issue in issues)
            {
                if (issue.Project.Id == ProjectId)
                {
                    Tasks.Add(new TaskInformation(issue, Resources.Find(x => x.ID == issue.AssignedTo.Id)));
                }
            }
        }

        public void AddTask(TaskInformation t)
        {
            Tasks.Add(t);
        }

        public void AddResource(ResourceInformation r)
        {
            Resources.Add(r);
        }
    }
}
