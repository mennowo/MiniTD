using System;
using System.Collections.Generic;

namespace MiniTD.DataTypes
{
    public class MiniFreeDay
    {
        public DateTime Date { get; set; }
    }

    public class MiniOrganizer
    {
        public long NextID { get; set; }

        public List<MiniTask> TaskInbox { get; set; }
        public List<MiniTask> AllTasks { get; set; }
        public List<MiniTopic> Topics { get; set; }
        public List<MiniFreeDay> FreeDays { get; set; }

        public MiniOrganizer()
        {
            TaskInbox = new List<MiniTask>();
            AllTasks = new List<MiniTask>();
            Topics = new List<MiniTopic>();
            FreeDays = new List<MiniFreeDay>();
            NextID = 0;
        }
    }
}
