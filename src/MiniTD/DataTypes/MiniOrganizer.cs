using System;
using System.Collections.Generic;

namespace MiniTD.DataTypes
{
    public class MiniDayDetails
    {
        public DateTime Date { get; set; }
        public bool IsFree { get; set; }
        public bool ManualNumberOfHours { get; set; }
        public double AvailableHours { get; set; } = 8;
    }

    public class MiniOrganizer
    {
        public long NextID { get; set; }

        public List<MiniTask> TaskInbox { get; set; }
        public List<MiniTask> AllTasks { get; set; }
        public List<MiniTopic> Topics { get; set; }
        public List<MiniDayDetails> DaysDetails { get; set; }

        public MiniOrganizer()
        {
            TaskInbox = new List<MiniTask>();
            AllTasks = new List<MiniTask>();
            Topics = new List<MiniTopic>();
            DaysDetails = new List<MiniDayDetails>();
            NextID = 0;
        }
    }
}
