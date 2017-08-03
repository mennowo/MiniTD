/**
Copyright(c) 2016 Menno van der Woude

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
**/

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
