using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TaskScheduler
{
    public class Task
    {
        private TaskContext taskContext;

        internal Task(TaskContext taskContext)
        {
            this.taskContext = taskContext;
        }
        public void Wait()
        {
            taskContext.Wait();
        }

        public void RequestPause()
        {
            taskContext.RequestPause();
        }

        public void RequestContinue()
        {
            taskContext.RequestContinue();
        }

        public void Terminate()
        {
            taskContext.Terminate();
        }

        public TaskContext.TaskState GetState()
        {
            return (TaskContext.TaskState)taskContext.GetState();
        }
    }
}
