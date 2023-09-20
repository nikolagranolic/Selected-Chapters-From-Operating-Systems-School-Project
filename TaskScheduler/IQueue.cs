using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public interface IQueue
    {
        public void Enqueue(TaskContext taskContext, int priority);

        public TaskContext Dequeue();

        public int Count();

        public int HighestPriority();
    }
}
