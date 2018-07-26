using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.umundo.core;

namespace DistributedTaskCal.Model
{
    public class CalEvent
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateofEvent { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Location { get; set; }

        public bool IsAllDay { get; set; }

        public CalEventType EventType { get; set; }
    }

    public class Reminder 
    {
        public string ID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string AppID { get; set; }

        public List<Reminder> ReminderList { get; set; }
    }

    public enum CalEventType
    {
        NORMAL = 0,
        BIRTHDAY = 1,
        MEETING = 2,
        PUBLIC_HOLIDAY = 3
    }
}
