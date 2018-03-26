using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api.Types;


namespace ProjectSuccessWPF.Redmine
{
    class RedmineProject
    {
        public string ProjectName { get; private set; }
        public int ProjectId { get; private set; }
        public List<TaskInformation> Tasks { get; private set; }
        public List<ResourceInformation> Resources { get; private set; }
        public List<string> Trackers { get; private set; }

        public RedmineProject(Project project, List<Issue> issues, List<User> users)
        {
            ProjectName = project.Name;
            ProjectId = project.Id;
            Tasks = new List<TaskInformation>();
            Resources = new List<ResourceInformation>();

            foreach(ProjectTracker tracker in project.Trackers)
            {
                Trackers.Add(tracker.Name);
            }

            NameValueCollection parameters = new NameValueCollection();
            List<int> usersIds = new List<int>();

            foreach(Issue issue in issues)
            {
                if(issue.Project.Id == ProjectId)
                {
                    TaskInformation t = new TaskInformation(issue);
                    if (!usersIds.Contains(issue.AssignedTo.Id))
                        usersIds.Add(issue.AssignedTo.Id);
                    Tasks.Add(t);
                }
            }

            foreach(int userId in usersIds)
            {
                Resources.Add(new ResourceInformation( project,users.Find(x => x.Id == userId)));
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
