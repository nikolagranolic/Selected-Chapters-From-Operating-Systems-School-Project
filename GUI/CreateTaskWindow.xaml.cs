using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using TaskSchedulerDemo;

namespace GUI
{
    /// <summary>
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        TaskScheduler.TaskScheduler taskScheduler;
        public CreateTaskWindow(TaskScheduler.TaskScheduler taskScheduler)
        {
            InitializeComponent();
            IEnumerable<Type> taskTypes = new List<Type> { typeof(TaskSchedulerDemo.DemoUserTask), typeof(TaskSchedulerDemo.AvgPoolingTask), typeof(TaskSchedulerDemo.MaxPoolingTask), typeof(TaskSchedulerDemo.MinPoolingTask) }; // TODO Ubrizgati tipove posla
            TaskScheduler.TaskSpecification job = new TaskScheduler.TaskSpecification(TaskScheduler.AdditionalOptions.None, new DemoUserTask());
            DataContext = job; // Kontekst za vezivanja -- podrazumijevani element za koji se vezuje, ako nije specificiran naziv elementa
            TaskTypeListBox.ItemsSource = taskTypes;
            TaskTypeListBox.SelectedIndex = 0; // Podrazumijevano odabran prvi
            this.taskScheduler = taskScheduler;
        }

        private void TaskTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO Obraditi događaj promjene izbora posla

            Type? selectedUserTaskType = (Type?)e.AddedItems[0]; // Podrazumijevano odabran prvi mogući tip posla
            Debug.Assert(selectedUserTaskType != null);
            IUserTask? userTask = (IUserTask?)Activator.CreateInstance(selectedUserTaskType); // TODO interfejs za poslove umjesto Object
            Debug.Assert(userTask != null);
            UserTaskJsonTextBox.Text = JsonSerializer.Serialize(userTask, userTask.GetType(), jsonSerializerOptions);
            // TODO Pri kreiranju posla deserijalizovati tekst (JsonSerializer.Deserialize)
            // Odabrani posao će na kraju biti (Type)JobTypeListBox.SelectedItem;
        }

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            IUserTask task;
            
            switch (TaskTypeListBox.SelectedIndex)
            {
                case 0:
                    task = (DemoUserTask)JsonSerializer.Deserialize(UserTaskJsonTextBox.Text, typeof(TaskSchedulerDemo.DemoUserTask));
                    break;
                case 1:
                    task = (AvgPoolingTask)JsonSerializer.Deserialize(UserTaskJsonTextBox.Text, typeof(TaskSchedulerDemo.AvgPoolingTask));
                    break;
                case 2:
                    task = (MaxPoolingTask)JsonSerializer.Deserialize(UserTaskJsonTextBox.Text, typeof(TaskSchedulerDemo.MaxPoolingTask));
                    break;
                default:
                    task = (MinPoolingTask)JsonSerializer.Deserialize(UserTaskJsonTextBox.Text, typeof(TaskSchedulerDemo.MinPoolingTask));
                    break;
            }

            TaskSpecification specification = new TaskSpecification(SchedulerWindow.addOptions, task)
            {
                Priority = (int)PrioritySlider.Value
            };
            SchedulingOptions options = new();
            if ((bool)DeadlineCheckBox.IsChecked)
            {
                options.endTimeAndDate = new TimeAndDate(DeadlineDatePicker.DisplayDate.Year, DeadlineDatePicker.DisplayDate.Month, DeadlineDatePicker.DisplayDate.Day, int.Parse(DeadlineTimePicker.Text.Split('-')[0]), int.Parse(DeadlineTimePicker.Text.Split('-')[1]), int.Parse(DeadlineTimePicker.Text.Split('-')[2]));
            }
            if ((bool)StartDateAndTimeCheckBox.IsChecked)
            {
                options.startTimeAndDate = new TimeAndDate(StartDateAndTimeDatePicker.DisplayDate.Year, StartDateAndTimeDatePicker.DisplayDate.Month, StartDateAndTimeDatePicker.DisplayDate.Day, int.Parse(StartDateAndTimeTimePicker.Text.Split('-')[0]), int.Parse(StartDateAndTimeTimePicker.Text.Split('-')[1]), int.Parse(StartDateAndTimeTimePicker.Text.Split('-')[2]));
            }
            if ((bool)AllowedExecutionTimeCheckBox.IsChecked)
            {
                options.totalExecutionTime = int.Parse(AllowedExecutionTimeTextBox.Text);
            }
            
            taskScheduler.ScheduleWithoutStarting(specification, options);
        }
    }
}
