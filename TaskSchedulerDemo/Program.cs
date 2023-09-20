using TaskScheduler;
using TaskSchedulerDemo;

IQueue queue = new FIFOQueue();
TaskScheduler.TaskScheduler taskScheduler = new TaskScheduler.TaskScheduler(queue, 1);

//const int MAX_DEGREE_OF_PARALLELISM = 2;
//AdditionalOptions addOptions = AdditionalOptions.Preemptive;
//TaskScheduler.Task taskL = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions,
//    new MaxPoolingTask("..\\..\\..\\..\\PrimjeriSlika", "..\\..\\..\\..\\PrimjeriSlikaOUT", 2, MAX_DEGREE_OF_PARALLELISM)
//    {
//        Name = "Maximum Pooling Task"
//    })
//{ Priority = 1 });

//TaskScheduler.Task taskM = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions,
//    new MinPoolingTask("..\\..\\..\\..\\PrimjeriSlika", "..\\..\\..\\..\\PrimjeriSlikaOUT", 2, MAX_DEGREE_OF_PARALLELISM)
//    {
//        Name = "Minimum Pooling Task"
//    })
//{ Priority = 2 });

//TaskScheduler.Task taskN = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions,
//    new AvgPoolingTask("..\\..\\..\\..\\PrimjeriSlika", "..\\..\\..\\..\\PrimjeriSlikaOUT", 2, MAX_DEGREE_OF_PARALLELISM)
//    {
//        Name = "Average Pooling Task"
//    })
//{ Priority = 3 });

AdditionalOptions addOptions = AdditionalOptions.TimeSlicing;

TaskScheduler.Task taskA = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
{
    Name = "TaskA",
    NumIterations = 20,
    SleepTime = 200,
})
{ Priority = 4 });

TaskScheduler.Task taskB = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
{
    Name = "TaskB",
    NumIterations = 20,
    SleepTime = 200,
})
{ Priority = 2 });

TaskScheduler.Task taskC = taskScheduler.ScheduleWithOptions(new TaskSpecification(addOptions, new DemoUserTask()
{
    Name = "TaskC",
    NumIterations = 20,
    SleepTime = 200,
})
{ Priority = 6 });