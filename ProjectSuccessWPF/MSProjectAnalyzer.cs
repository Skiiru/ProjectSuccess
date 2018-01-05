﻿using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class MSProjectAnalyzer
    {
        ProjectFile project;

        public MSProjectAnalyzer(ProjectFile project)
        {
            this.project = project;
        }

        #region Tasks
        public List<TaskInformation> GetTasksWithoutHierarhy()
        {
            List<TaskInformation> taskList = new List<TaskInformation>();
            foreach (TaskInformation t in this.GetTasksWithHierarhy())
            {
                taskList.AddRange(GetFinalTasks(t));
            }
            return taskList;
        }

        List<TaskInformation> GetFinalTasks(TaskInformation taskInf)
        {
            List<TaskInformation> result = new List<TaskInformation>();
            if (taskInf.ChildTasks.Count == 0)
                result.Add(taskInf);
            else
                foreach (TaskInformation t in taskInf.ChildTasks)
                    result.AddRange(GetFinalTasks(t));
            return result;
        }

        public List<TaskInformation> GetTasksWithHierarhy()
        {
            List<TaskInformation> taskList = new List<TaskInformation>();
            string levelStr = string.Empty;

            int level = 0;

            //First item in hierarhy is project file first name, so we need to ignore it
            foreach (Task firstOrderTask in project.getChildTasks().toArray())
            {
                foreach (Task normalTask in firstOrderTask.getChildTasks().toArray())
                {
                    level++;
                    var assigments = normalTask.getResourceAssignments().toArray();
                    List<Resource> list = new List<Resource>();
                    for (int i = 0; i < assigments.Length; ++i)
                    {
                        list.Add((assigments[i] as ResourceAssignment).getResource());
                    }
                    TaskInformation task = new TaskInformation(normalTask, list);
                    GetTaskHierarhy(ref task);
                    taskList.Add(task);
                }
            }

            return taskList;
        }

        private void GetTaskHierarhy(ref TaskInformation task)
        {
            if (task.GetChildTasks() != null && task.GetChildTasks().Length > 0)
                foreach (Task t in task.GetChildTasks())
                {
                    var assigments = t.getResourceAssignments().toArray();
                    List<Resource> list = new List<Resource>();
                    for (int i = 0; i < assigments.Length; ++i)
                    {
                        list.Add((assigments[i] as ResourceAssignment).getResource());
                    }
                    TaskInformation taskInf = new TaskInformation(t, list);
                    GetTaskHierarhy(ref taskInf);
                    task.ChildTasks.Add(taskInf);
                }
        }
        #endregion

        #region Resources
        public List<ResourceInformation> GetResources()
        {
            List<ResourceInformation> list = new List<ResourceInformation>();
            foreach (Resource res in project.getAllResources())
            {
                List<Task> tasks = new List<Task>();
                var assigments = res.getTaskAssignments().toArray();
                if (res.getName() != null)
                {
                    foreach (var assigment in assigments)
                    {
                        tasks.Add((assigment as ResourceAssignment).getTask());
                    }
                    list.Add(new ResourceInformation(res, tasks));
                }
            }
            return list;
        }

        #endregion

        public ProjectProperties GetProjectProperties()
        {
            if (project != null)
                return project.getProjectProperties();
            else
                return null;
        }
    }
}
