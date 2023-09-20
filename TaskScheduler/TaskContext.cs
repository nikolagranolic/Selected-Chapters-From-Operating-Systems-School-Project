using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public enum AdditionalOptions
    {
        None,
        Preemptive,
        TimeSlicing
    }
    //public enum TaskState
    //{
    //    NotStarted,
    //    Queued,
    //    Running,
    //    RunningWithPauseRequested,
    //    WaitingToResume,
    //    Paused,
    //    Finished,
    //    Terminated
    //}
    public class TaskContext : ITaskContext, INotifyPropertyChanged
    {
        public enum TaskState
        {
            NotStarted,
            Queued,
            Running,
            RunningWithPauseRequested,
            WaitingToResume,
            Paused,
            Finished,
            Terminated
        }

        private double progress;
        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                NotifyPropertyChanged();
            }
        }
        public static bool firstTask = true;
        private AdditionalOptions additionalOptions;
        private TaskState taskState = TaskState.NotStarted;
        public TaskState State
        {
            get
            {
                return taskState;
            }
            set
            {
                taskState = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsStartable));
                NotifyPropertyChanged(nameof(IsPausable));
                NotifyPropertyChanged(nameof(IsStoppable));
            }
        }
        public bool IsStartable
        {
            get
            {
                return State == TaskState.NotStarted || State == TaskState.Paused || State == TaskState.Queued;
            }
        }
        public bool IsPausable
        {
            get
            {
                return State == TaskState.Running;
            }
        }
        public bool IsStoppable
        {
            get
            {
                return State != TaskState.NotStarted && State != TaskState.Terminated && State != TaskState.Finished;
            }
        }
        private Thread thread;
        public string Name { get; set; }
        private object taskContextLock = new();
        private Action<TaskContext> taskFinishedOrTerminatedAction;
        private Action<TaskContext> taskPausedAction;
        private Action<TaskContext> taskContinueRequestedAction;
        internal int Priority { get; init; }
        private SemaphoreSlim finishedSemaphore = new(0);
        private SemaphoreSlim resumeSemaphore = new(0);
        private SemaphoreSlim terminatedSemaphore = new(0);
        private int numOfWaiters = 0;
        private IQueue queueOfWaitingTasks;

        public TaskContext(string name, AdditionalOptions addOptions, IUserTask userTask, int priority, IQueue queue, Action<TaskContext> taskFinishedOrTerminatedAction, Action<TaskContext> taskPausedAction, Action<TaskContext> taskContinueRequestedAction)
        {
            Name = name;
            thread = new(() =>
            {
                try
                {
                    userTask.Run(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Finish();
                }
            });

            additionalOptions = addOptions;
            Priority = priority;
            queueOfWaitingTasks = queue;
            this.taskFinishedOrTerminatedAction = taskFinishedOrTerminatedAction;
            this.taskPausedAction = taskPausedAction;
            this.taskContinueRequestedAction = taskContinueRequestedAction;

            //taskContexts.Add(name, this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Start()
        {
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.Paused: // ako sam bio pauziran jer je dosao task viseg pr. pa se ponovo startovao jer se neki task u medjuvremenu zavrsio
                        State = TaskState.WaitingToResume;
                        taskContinueRequestedAction(this);
                        break;
                    case TaskState.Queued:
                    case TaskState.NotStarted:
                        State = TaskState.Running;

                        thread.Start();

                        break;
                    case TaskState.RunningWithPauseRequested:
                        break;
                    case TaskState.Running:
                        break;
                        throw new InvalidOperationException("Task has already started.");
                    case TaskState.Finished:
                        break;
                    case TaskState.Terminated:
                        break;
                    case TaskState.WaitingToResume:
                        State = TaskState.Running;
                        resumeSemaphore.Release();

                        break;
                    default:
                        throw new InvalidOperationException("Invalid task state.");
                }
            }
        }

        public void Finish()
        {
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.NotStarted:
                        throw new InvalidOperationException("Job not started.");
                    case TaskState.Running:
                        State = TaskState.Finished;
                        if(numOfWaiters > 0)
                        {
                            finishedSemaphore.Release(numOfWaiters);
                        }
                        taskFinishedOrTerminatedAction(this);
                        break;
                    case TaskState.Terminated: // Task was already terminated
                        return;
                    case TaskState.Finished:
                        throw new InvalidOperationException("Job already finished.");
                    default:
                        break;
                }
            }
        }

        public void Wait()
        {
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.NotStarted:
                    case TaskState.RunningWithPauseRequested:
                    case TaskState.Running:
                        numOfWaiters++;
                        break;
                    case TaskState.Finished:
                        return;
                    case TaskState.Terminated:
                        return;
                    default:
                        throw new InvalidOperationException("Invalid task state.");
                }
            }

            finishedSemaphore.Wait();
        }

        public void RequestPause()
        {
            Console.WriteLine(new StackTrace());
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.NotStarted:
                        break;
                    case TaskState.Running:
                        State = TaskState.RunningWithPauseRequested;
                        break;
                    case TaskState.RunningWithPauseRequested:
                        break;
                    case TaskState.Paused:
                        break;
                    case TaskState.Finished:
                        break;
                    default:
                        throw new InvalidOperationException("Invalid task state.");
                }
            }
        }

        public void RequestContinue()
        {
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.NotStarted:
                        break;
                    case TaskState.Running:
                        break;
                    case TaskState.RunningWithPauseRequested:
                        State = TaskState.Running;
                        break;
                    case TaskState.Paused:
                        State = TaskState.WaitingToResume;
                        taskContinueRequestedAction(this);
                        break;
                    case TaskState.Finished:
                        break;
                    default:
                        throw new InvalidOperationException("Invalid task state.");
                }
            }
        }

        public void Terminate()
        {
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.Terminated:
                        throw new InvalidOperationException("Job already terminated.");
                    case TaskState.Finished:
                        throw new InvalidOperationException("Job already finished.");
                    default:
                        State = TaskState.Terminated;
                        if (numOfWaiters > 0)
                        {
                            finishedSemaphore.Release(numOfWaiters);
                        }
                        taskFinishedOrTerminatedAction(this);
                        break;
                }
            }
        }

        public void CheckForPause()
        {
            bool shouldPause = false;
            lock (taskContextLock)
            {
                switch (State)
                {
                    case TaskState.Running:
                        break;
                    case TaskState.RunningWithPauseRequested:
                        State = TaskState.Paused;
                        taskPausedAction(this);
                        shouldPause = true;
                        break;
                    case TaskState.Terminated:
                        throw new Exception("Task terminated.");
                    default:
                        break;
                }
            }

            // checking if there is a higher priority task waiting
            if (additionalOptions == AdditionalOptions.Preemptive)
            {
                lock (TaskScheduler.queueLock)
                {
                    if (IsHigherPriorityWaiting())
                    {
                        State = TaskState.Paused;
                        taskPausedAction(this);
                        shouldPause = true;
                    }
                }
            }

            if (shouldPause)
            {
                lock (TaskScheduler.queueLock)
                {
                    queueOfWaitingTasks.Enqueue(this, this.Priority);
                }
                resumeSemaphore.Wait();
            }
            
        }

        private bool IsHigherPriorityWaiting()
        {
            if (queueOfWaitingTasks.HighestPriority() < this.Priority)
            {
                return true;
            }
            return false;
        }

        public TaskState GetState()
        {
            return State;
        }
    }
}
