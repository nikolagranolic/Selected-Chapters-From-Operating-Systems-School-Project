using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskScheduler;

namespace GUI
{
    /// <summary>
    /// Interaction logic for SchedulerWindow.xaml
    /// </summary>
    public partial class SchedulerWindow : Window
    {
        private TaskScheduler.TaskScheduler scheduler;
        internal static AdditionalOptions addOptions = AdditionalOptions.None;
        public SchedulerWindow(string schedulingOptions, int maxNoOfConcurrentTasks)
        {
            TaskScheduler.IQueue queue;
            switch(schedulingOptions)
            {
                case "FIFO":
                    queue = new TaskScheduler.FIFOQueue();
                    break;
                case "Time Slicing":
                    addOptions = AdditionalOptions.TimeSlicing;
                    queue = new TaskScheduler.FIFOQueue();
                    break;
                default:
                    queue = new TaskScheduler.PrioQueue();
                    break;
            }
            if (schedulingOptions == "Preemptive")
            {
                addOptions = AdditionalOptions.Preemptive;
            }
            scheduler = new TaskScheduler.TaskScheduler(queue, maxNoOfConcurrentTasks);
            InitializeComponent();
            TasksListView.ItemsSource = scheduler.AllTasks;

            DataContext = this;
        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTaskWindow taskWindow = new(scheduler);
            taskWindow.Show();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            object obj = new();
            if (button != null)
            {
                obj = button.DataContext;
            }
            TaskContext tc = (TaskContext)obj;
            if (tc.State == TaskContext.TaskState.Paused)
            {
                tc.RequestContinue();
            }
            else
            {
                tc.State = TaskContext.TaskState.Queued;
                scheduler.ScheduleGUI(tc, scheduler.taskSpecificationPairs[tc], scheduler.taskOptionsPairs[tc]);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            object obj = new();
            if (button != null)
            {
                obj = button.DataContext;
            }
            TaskContext tc = (TaskContext)obj;
            if (tc.State == TaskContext.TaskState.Running)
            {
                tc.RequestPause();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            object obj = new();
            if (button != null)
            {
                obj = button.DataContext;
            }
            TaskContext tc = (TaskContext)obj;
            tc.Terminate();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            object obj = new();
            if (button != null)
            {
                obj = button.DataContext;
            }
            TaskContext tc = (TaskContext)obj;
            scheduler.AllTasks.Remove(tc);
        }
    }
}
