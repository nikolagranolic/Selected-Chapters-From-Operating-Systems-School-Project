using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public class TaskSpecification
    {
        public AdditionalOptions AddOptions { get; }
        internal IUserTask UserTask { get; }
        public int Priority { get; init; } = 0;
        public TaskSpecification(AdditionalOptions addOptions, IUserTask userTask)
        {
            AddOptions = addOptions;
            UserTask = userTask;
        }
    }
}
