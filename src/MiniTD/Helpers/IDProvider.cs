using MiniTD.DataTypes;

namespace MiniTD.Helpers
{
    public static class IDProvider
    {
        public static MiniOrganizer Organizer { get; set; }

        public static long GetNextID()
        {
            if (Organizer != null)
            {
                Organizer.NextID++;
                return Organizer.NextID;
            }
            else
            {
                return 0;
            }
        }
    }
}
