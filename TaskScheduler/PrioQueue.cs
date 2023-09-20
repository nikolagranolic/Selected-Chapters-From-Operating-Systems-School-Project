using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public class PrioQueue : IQueue
    {
        private PriorityQueue<TaskContext, int> queue = new();

        public void Enqueue(TaskContext context, int priority)
        {
            queue.Enqueue(context, priority);
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
            return queue.Peek().Priority;
        }
    }
}
