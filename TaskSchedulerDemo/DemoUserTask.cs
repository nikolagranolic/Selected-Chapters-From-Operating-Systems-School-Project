using System.Transactions;
using TaskScheduler;

namespace TaskSchedulerDemo
{
    public class DemoUserTask : IUserTask
    {
        public string Name { get; init; } = "DemoUserTask";
        public int NumIterations { get; init; } = 100;
        public int SleepTime { get; init; } = 50;

        public void Run(ITaskContext taskApi)
        {
            Console.WriteLine($"{Name} started");
            
            for (int i = 0; i < NumIterations; i++)
            {
                Console.WriteLine($"{Name}: {i}");
                
                Thread.Sleep(SleepTime);
                taskApi.Progress = i / (double)NumIterations;
                taskApi.CheckForPause();
            }

            taskApi.Progress = 1;
            Console.WriteLine($"{Name} finished.");
        }
    }
}
