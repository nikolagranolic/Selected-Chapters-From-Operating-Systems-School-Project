using System.Threading.Tasks;
using TaskScheduler;
using TaskSchedulerDemo;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FIFOTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 5,
                SleepTime = 200,
            }));

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 5,
                SleepTime = 200,
            }));

            TaskScheduler.Task taskC = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskC",
                NumIterations = 5,
                SleepTime = 200,
            }));

            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.NotStarted);
            Thread.Sleep(1100);
            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.Running);
            Assert.IsTrue(taskC.GetState() == TaskContext.TaskState.NotStarted);
        }

        [TestMethod]
        public void PriorityTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new PrioQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 10,
                SleepTime = 200,
            })
            {
                Priority = 1
            });

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 10,
                SleepTime = 200,
            })
            {
                Priority = 9
            });

            TaskScheduler.Task taskC = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskC",
                NumIterations = 10,
                SleepTime = 200,
            })
            {
                Priority = 3
            });

            Thread.Sleep(2500);

            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.NotStarted);
            Assert.IsTrue(taskC.GetState() == TaskContext.TaskState.Running);
        }

        [TestMethod]
        public void WaitOnTaskEndTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 15,
                SleepTime = 200,
            }));

            taskA.Wait();
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Finished);

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 10,
                SleepTime = 200,
            }));
        }

        [TestMethod]
        public void PauseTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 15,
                SleepTime = 200,
            }));

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 10,
                SleepTime = 200,
            }));

            taskA.RequestPause();
            Thread.Sleep(400);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Paused);
            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.Running);
        }

        [TestMethod]
        public void ContinueTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 15,
                SleepTime = 200,
            }));

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 10,
                SleepTime = 200,
            }));

            taskA.RequestPause();
            taskA.RequestContinue();
            Thread.Sleep(400);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Running);
            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.NotStarted);
        }

        [TestMethod]
        public void TerminateTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 15,
                SleepTime = 200,
            }));

            taskA.Terminate();
            Thread.Sleep(300);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Terminated);
        }

        [TestMethod]
        public void AllowedRunningTimeTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);
            TaskScheduler.SchedulingOptions options = new SchedulingOptions(null, 500, null);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None   , new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 20,
                SleepTime = 200,
            }), options);

            Thread.Sleep(1000);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Terminated);
        }

        [TestMethod]
        public void StartTimeTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);
            TimeAndDate timeAndDate = new(2023, 2, 2, 1, 41, 0);
            TaskScheduler.SchedulingOptions options = new SchedulingOptions(timeAndDate, 0, null);

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(AdditionalOptions.None, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 20,
                SleepTime = 200,
            }), options);

            Thread.Sleep(500);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.NotStarted);
        }

        [TestMethod]
        public void PreemptiveTest()
        {
            int maxNumOfConcurrentTasks = 1;
            IQueue queue = new FIFOQueue();
            TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, maxNumOfConcurrentTasks);

            const int MAX_DEGREE_OF_PARALLELISM = 1;
            AdditionalOptions addOptions = AdditionalOptions.Preemptive;

            TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
            {
                Name = "TaskA",
                NumIterations = 20,
                SleepTime = 200,
            })
            { Priority = 3 });

            TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
            {
                Name = "TaskB",
                NumIterations = 20,
                SleepTime = 200,
            })
            { Priority = 2 });

            Thread.Sleep(500);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Paused);
            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.Running);

            TaskScheduler.Task taskC = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
            {
                Name = "TaskC",
                NumIterations = 20,
                SleepTime = 200,
            })
            { Priority = 1 });

            Thread.Sleep(500);
            Assert.IsTrue(taskA.GetState() == TaskContext.TaskState.Paused);
            Assert.IsTrue(taskB.GetState() == TaskContext.TaskState.Paused);
            Assert.IsTrue(taskC.GetState() == TaskContext.TaskState.Running);
        }
    }
}