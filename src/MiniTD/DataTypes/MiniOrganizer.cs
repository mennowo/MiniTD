using System.Collections.Generic;

namespace MiniTD.DataTypes
{
    public class MiniOrganizer
    {
        public long NextID { get; set; }

        public List<MiniTask> TaskInbox { get; set; }
        public List<MiniTask> AllTasks { get; set; }
        public List<MiniTopic> Topics { get; set; }

        public MiniOrganizer()
        {
            TaskInbox = new List<MiniTask>();
            AllTasks = new List<MiniTask>();
            Topics = new List<MiniTopic>();
            NextID = 0;
        }
    }
}
