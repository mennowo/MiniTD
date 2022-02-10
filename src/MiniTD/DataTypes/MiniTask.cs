using MiniTD.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MiniTD.DataTypes
{
    public enum MiniTaskStatus
    {
        [Description("ASAP")]
        ASAP,
        [Description("Inactive")]
        Inactive,
        [Description("Scheduled")]
        Scheduled,
        [Description("Delegated")]
        Delegated
    }

    public enum MiniTaskType
    {
        [Description("Task")]
        Task,
        [Description("Project")]
        Project
    }

    public class MiniTask
    {
        public long ID { get; set; }
        public MiniTaskStatus Status { get; set; }
        public MiniTaskType Type { get; set; }
        public string Title { get; set; }
        public string Outcome { get; set; }
        public string DelegatedTo { get; set; }
        public long TopicID { get; set; }
        public long ProjectID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateDone { get; set; }
        [XmlIgnore]
        public TimeSpan Duration { get; set; }
        [XmlElement("DurationSer")]
        public long SerializedDuration
        {
            get { return Duration.Ticks; }
            set
            {
                Duration = new TimeSpan(value);
            }
        }
        public bool Done { get; set; }

        public List<MiniTaskNote> Notes { get; set; }
        public List<MiniTask> AllTasks { get; set; }

        public MiniTask()
        {
            Notes = new List<MiniTaskNote>();
            AllTasks = new List<MiniTask>();

            DateCreated = DateTime.Now;
            DateDue = DateTime.Now;

            ID = IDProvider.GetNextID();
        }
    }
}
