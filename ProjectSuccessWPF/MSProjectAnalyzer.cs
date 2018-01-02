using net.sf.mpxj;
using System.Collections.Generic;

namespace ProjectSuccessWPF
{
    class MSProjectAnalyzer
    {
        public ProjectFile project;

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

        public ProjectProperties GetProjectProperties()
        {
            if (project != null)
                return project.getProjectProperties();
            else
                return null;
        }
    }
}
