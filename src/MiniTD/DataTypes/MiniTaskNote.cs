using System;

namespace MiniTD.DataTypes
{
    public class MiniTaskNote
    {
        public DateTime DateCreated { get; set; }
        public string Note { get; set; }

        public MiniTaskNote()
        {
            DateCreated = DateTime.Now;
        }
    }
}
