using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public class FIFOQueue : IQueue
    {
        private Queue<TaskContext> queue = new();

        public void Enqueue(TaskContext taskContext, int priority = 0)
        {
            queue.Enqueue(taskContext);
        }

        public TaskContext Dequeue()
        {
            return queue.Dequeue();
        }

        public int Count()
        {
            return queue.Count;
        }

        public int HighestPriority()
        {
            int highestPriority = int.MaxValue;
            foreach (TaskContext task in queue)
            {
                if (task.Priority < highestPriority)
                {
                    highestPriority = task.Priority;
                }
            }
            return highestPriority;
        }
    }
}
