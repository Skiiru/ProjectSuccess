using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class MSProjectAnalyzer
    {
        public ProjectFile project;

        #region Tasks
        public List<TaskInformation> GetTasksWithoutHierarhy()
        {
            List<TaskInformation> taskList = new List<TaskInformation>();
            foreach (Task task in project.getAllTasks())
            {
                var assigments = task.getResourceAssignments().toArray();
                List<Resource> list = new List<Resource>();
                for (int i = 0; i < assigments.Length; ++i)
                {
                    list.Add((assigments[i] as ResourceAssignment).getResource());
                }
                taskList.Add(new TaskInformation(task, list));
            }

            return taskList;
        }

        public List<TaskInformation> GetTasksWithHierarhy()
        {
            List<TaskInformation> taskList = new List<TaskInformation>();
            string levelStr = string.Empty;

            int level = 0;

            foreach (Task firstOrderTask in project.getChildTasks().toArray())
            {
                level++;
                var assigments = firstOrderTask.getResourceAssignments().toArray();
                List<Resource> list = new List<Resource>();
                for (int i = 0; i < assigments.Length; ++i)
                {
                    list.Add((assigments[i] as ResourceAssignment).getResource());
                }
                TaskInformation task = new TaskInformation(firstOrderTask, list);
                GetTaskHierarhy(ref task);
                taskList.Add(task);
            }

            return taskList;
        }

        private void GetTaskHierarhy(ref TaskInformation task)
        {
            foreach (Task t in task.task.getChildTasks().toArray())
            {
                var assigments = t.getResourceAssignments().toArray();
                List<Resource> list = new List<Resource>();
                for (int i = 0; i < assigments.Length; ++i)
                {
                    list.Add((assigments[i] as ResourceAssignment).getResource());
                }
                TaskInformation taskInf = new TaskInformation(t, list);
                GetTaskHierarhy(ref taskInf);
                task.childTasks.Add(taskInf);
            }
        }
        #endregion

        #region Resources
        public List<ResourceInformation> GetResources()
        {
            List<ResourceInformation> list = new List<ResourceInformation>();
            foreach(Resource res in project.getAllResources())
            {
                ResourceInformation information = new ResourceInformation(res);
                var assigments = res.getTaskAssignments().toArray();
                foreach(Task assigment in assigments)
                {
                    information.tasks.Add(assigment);
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
