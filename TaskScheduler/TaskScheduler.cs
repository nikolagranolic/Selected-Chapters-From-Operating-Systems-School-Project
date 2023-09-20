using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;

namespace TaskScheduler
{
    public class TaskScheduler
    {
        static private System.Timers.Timer? timer;
        static public int timeSlice = 50; // in milliseconds
        static public Dictionary<string, TaskContext> taskResourceConnection = new();
        public int MaxConcurrentTasks { get; set; } = 1;
        static public IQueue queue;
        public IQueue taskQueue;
        public static object queueLock = new();

        private HashSet<TaskContext> runningTasks = new();
        public ObservableCollection<TaskContext> AllTasks { get; set; } = new();
        public Dictionary<TaskContext, SchedulingOptions> taskOptionsPairs = new();
        public Dictionary<TaskContext, TaskSpecification> taskSpecificationPairs = new();

        private object schedulerLock = new();

        public TaskScheduler(IQueue queue, int maxConcurrentTasks)
        {
            this.taskQueue = queue;
            TaskScheduler.queue = queue;
            this.MaxConcurrentTasks = maxConcurrentTasks;
        }

        public void ScheduleWithoutStarting(TaskSpecification taskSpecification, SchedulingOptions options)
        {
            TaskContext taskContext = new(
                name: taskSpecification.UserTask.Name,
                userTask: taskSpecification.UserTask,
                priority: taskSpecification.Priority,
                queue: taskQueue,
                addOptions: taskSpecification.AddOptions,
                taskFinishedOrTerminatedAction: HandleTaskFinishedOrTerminated,
                taskPausedAction: HandleTaskPaused,
                taskContinueRequestedAction: HandleTaskContinueRequested);

            AllTasks.Add(taskContext);
            taskOptionsPairs.Add(taskContext, options);
            taskSpecificationPairs.Add(taskContext, taskSpecification);
        }
        public Task ScheduleWithOptions(TaskSpecification taskSpecification, SchedulingOptions options = null)
        {
            TaskContext taskContext = new(
                name: taskSpecification.UserTask.Name,
                userTask: taskSpecification.UserTask,
                priority: taskSpecification.Priority,
                queue: taskQueue,
                addOptions: taskSpecification.AddOptions,
                taskFinishedOrTerminatedAction: HandleTaskFinishedOrTerminated,
                taskPausedAction: HandleTaskPaused,
                taskContinueRequestedAction: HandleTaskContinueRequested);

            // if start date and time of scheduling are set do this:
            if (options != null && options.startTimeAndDate != null && options.startTimeAndDate.IsValid())
            {
                DateTime nowTime = DateTime.Now;
                DateTime scheduledTime = new DateTime(options.startTimeAndDate.Year, options.startTimeAndDate.Month, options.startTimeAndDate.Day, options.startTimeAndDate.Hour, options.startTimeAndDate.Minute, options.startTimeAndDate.Second);
                double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
                if (tickTime < 0)
                {
                    throw new InvalidOperationException("Invalid date and time.");
                }
                timer = new System.Timers.Timer(tickTime);


                timer.Elapsed += (o, e) =>
                {
                    lock (schedulerLock)
                    {
                        if (runningTasks.Count < MaxConcurrentTasks)
                        {
                            runningTasks.Add(taskContext);
                            taskContext.Start();
                        }
                        else
                        {
                            lock (queueLock)
                            {
                                taskQueue.Enqueue(taskContext, taskSpecification.Priority);
                            }
                        }
                    }
                };
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();

                return new Task(taskContext);
            }
            // if start date and time are not set schedule task immediately:
            lock (schedulerLock)
            {
                if (runningTasks.Count < MaxConcurrentTasks)
                {
                    runningTasks.Add(taskContext);
                    taskContext.Start();
                }
                else
                {
                    lock (queueLock)
                    {
                        taskQueue.Enqueue(taskContext, taskSpecification.Priority);
                    }
                }
            }
            // start timer for total execution time (if chosen as option):
            if (options != null && options.totalExecutionTime > 0)
            {
                timer = new System.Timers.Timer(options.totalExecutionTime);
                timer.Elapsed += (o, e) => taskContext.Terminate();
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();
            }
            // if end time is set set up a timer to terminate task at a specific time:
            if (options != null && options.endTimeAndDate != null && options.endTimeAndDate.IsValid())
            {
                DateTime nowTime = DateTime.Now;
                DateTime scheduledTime = new DateTime(options.endTimeAndDate.Year, options.endTimeAndDate.Month, options.endTimeAndDate.Day, options.endTimeAndDate.Hour, options.endTimeAndDate.Minute, options.endTimeAndDate.Second);
                double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;

                timer = new System.Timers.Timer(tickTime);
                timer.Elapsed += (o, e) => taskContext.Terminate();
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();
            }
            return new Task(taskContext);
        }

        public Task ScheduleGUI(TaskContext taskContext, TaskSpecification taskSpecification, SchedulingOptions options = null)
        {

            // if start date and time of scheduling are set do this:
            if (options != null && options.startTimeAndDate != null && options.startTimeAndDate.IsValid())
            {
                DateTime nowTime = DateTime.Now;
                DateTime scheduledTime = new DateTime(options.startTimeAndDate.Year, options.startTimeAndDate.Month, options.startTimeAndDate.Day, options.startTimeAndDate.Hour, options.startTimeAndDate.Minute, options.startTimeAndDate.Second);
                double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
                if (tickTime < 0)
                {
                    throw new InvalidOperationException("Invalid date and time.");
                }
                timer = new System.Timers.Timer(tickTime);


                timer.Elapsed += (o, e) =>
                {
                    lock (schedulerLock)
                    {
                        if (runningTasks.Count < MaxConcurrentTasks)
                        {
                            runningTasks.Add(taskContext);
                            taskContext.Start();
                        }
                        else
                        {
                            lock (queueLock)
                            {
                                taskQueue.Enqueue(taskContext, taskSpecification.Priority);
                            }
                        }
                    }
                };
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();

                return new Task(taskContext);
            }
            // if start date and time are not set schedule task immediately:
            lock (schedulerLock)
            {
                if (runningTasks.Count < MaxConcurrentTasks)
                {
                    runningTasks.Add(taskContext);
                    taskContext.Start();
                }
                else
                {
                    lock (queueLock)
                    {
                        taskQueue.Enqueue(taskContext, taskSpecification.Priority);
                    }
                }
            }
            // start timer for total execution time (if chosen as option):
            if (options != null && options.totalExecutionTime > 0)
            {
                timer = new System.Timers.Timer(options.totalExecutionTime);
                timer.Elapsed += (o, e) => taskContext.Terminate();
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();
            }
            // if end time is set set up a timer to terminate task at a specific time:
            if (options != null && options.endTimeAndDate != null && options.endTimeAndDate.IsValid())
            {
                DateTime nowTime = DateTime.Now;
                DateTime scheduledTime = new DateTime(options.endTimeAndDate.Year, options.endTimeAndDate.Month, options.endTimeAndDate.Day, options.endTimeAndDate.Hour, options.endTimeAndDate.Minute, options.endTimeAndDate.Second);
                double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;

                timer = new System.Timers.Timer(tickTime);
                timer.Elapsed += (o, e) => taskContext.Terminate();
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Start();
            }
            return new Task(taskContext);
        }

        private void HandleTaskFinishedOrTerminated(TaskContext taskContext)
        {
            lock(schedulerLock)
            {
                runningTasks.Remove(taskContext);
                if (taskQueue.Count() > 0)
                {
                    TaskContext dequeuedTaskContext;
                    lock (queueLock)
                    {
                        dequeuedTaskContext = taskQueue.Dequeue();
                    }
                    runningTasks.Add(dequeuedTaskContext);
                    dequeuedTaskContext.Start();
                }
            }
        }

        private void HandleTaskPaused(TaskContext taskContext)
        {
             lock(schedulerLock)
            {
                runningTasks.Remove(taskContext);
                if (taskQueue.Count() > 0)
                {
                    TaskContext dequeuedTaskContext;
                    lock (queueLock)
                    {
                        dequeuedTaskContext = taskQueue.Dequeue();
                    }
                    runningTasks.Add(dequeuedTaskContext);
                    dequeuedTaskContext.Start();
                    if (TaskScheduler.queue.Count() > 0 || TaskContext.firstTask)
                    {
                        TaskContext.firstTask = false;
                        System.Timers.Timer? timer = new System.Timers.Timer((11 - dequeuedTaskContext.Priority) * TaskScheduler.timeSlice);
                        timer.Elapsed += (o, e) => dequeuedTaskContext.RequestPause();
                        timer.AutoReset = false;
                        timer.Enabled = true;
                        timer.Start();
                    }
                }
            }
        }

        private void HandleTaskContinueRequested(TaskContext taskContext)
        {
            lock(schedulerLock)
            {
                if (runningTasks.Count < MaxConcurrentTasks || runningTasks.Contains(taskContext))
                {
                    runningTasks.Add(taskContext);

                    //if (TaskScheduler.queue.Count() > 0 || TaskContext.firstTask)
                    //{
                    //    TaskContext.firstTask = false;
                    //    System.Timers.Timer? timer = new System.Timers.Timer((11 - taskContext.Priority) * TaskScheduler.timeSlice);
                    //    timer.Elapsed += (o, e) => taskContext.RequestPause();
                    //    timer.AutoReset = false;
                    //    timer.Enabled = true;
                    //    timer.Start();
                    //}
                    taskContext.Start();
                }
                else
                {
                    lock (queueLock)
                    {
                        taskQueue.Enqueue(taskContext, taskContext.Priority);
                    }
                }
            }
        }
    }
}
