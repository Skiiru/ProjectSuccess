﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api.Types;


namespace ProjectSuccessWPF.Redmine
{
    public class RedmineProject : IProject
    {
        public string ProjectName { get; private set; }
        public int ProjectId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int TasksCount { get { return Tasks.Count; } }
        public int ResourcesCount { get { return Resources.Count; } }

        public List<TaskInformation> Tasks { get; private set; }
        public List<ResourceInformation> Resources { get; private set; }
        public List<string> Trackers { get; private set; }
        public ProjectRate Rate { get; private set; }
        public string Status { get; private set; }

        public RedmineProject(Project project, List<Issue> issues, List<User> users)
        {
            ProjectName = project.Name;
            ProjectId = project.Id;
            Tasks = new List<TaskInformation>();
            Resources = new List<ResourceInformation>();
            StartDate = DateTime.MaxValue;
            EndDate = DateTime.MinValue;
            Status = project.Status.ToString();

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

            for (int i = 0; i < Tasks.Count; ++i)
            {
                if (Tasks[i].Dates.StartDate < StartDate)
                    StartDate = Tasks[i].Dates.StartDate;
                if (Tasks[i].Dates.FinishDate > EndDate)
                    EndDate = Tasks[i].Dates.FinishDate;
            }

            Rate = new ProjectRate(this);
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
