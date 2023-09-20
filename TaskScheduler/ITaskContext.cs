using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public interface ITaskContext
    {
        public double Progress { get; set; }
        public void CheckForPause();
    }
}
