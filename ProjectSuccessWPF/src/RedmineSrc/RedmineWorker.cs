using ProjectSuccessWPF.Redmine;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ProjectSuccessWPF
{
    class RedmineWorker
    {
        public List<RedmineProject> LoadProjects()
        {
            List<RedmineProject> result = new List<RedmineProject>();

            RedmineManager manager = new RedmineManager(AppSettings.Settings.Default.RedmineHost, AppSettings.Settings.Default.RedmineApiKey);
            List<Issue> issues = manager.GetObjects<Issue>();
            List<User> users = manager.GetObjects<User>();

            foreach (Project project in manager.GetObjects<Project>())
            {
                RedmineProject p = new RedmineProject(project, issues, users);
                result.Add(p);
            }

            return result;
        }

    }
}
