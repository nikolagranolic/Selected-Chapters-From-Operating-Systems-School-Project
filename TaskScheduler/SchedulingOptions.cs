using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    public class SchedulingOptions
    {
        public TimeAndDate startTimeAndDate = null;
        public int totalExecutionTime = 0; // seconds
        public TimeAndDate endTimeAndDate = null;

        public SchedulingOptions() { }

        public SchedulingOptions(TimeAndDate startTimeAndDate, int totalExecutionTime, TimeAndDate endDateAndTime)
        {
            this.startTimeAndDate = startTimeAndDate;
            this.totalExecutionTime = totalExecutionTime;
            this.endTimeAndDate = endDateAndTime;
        }
    }

    public class TimeAndDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        public TimeAndDate(int year, int month, int day, int hour, int minute, int second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public bool IsValid()
        {
            if (Year == 0 || Month == 0 || Day == 0)
            {
                return false;
            }
            return true;
        }
    }
}
