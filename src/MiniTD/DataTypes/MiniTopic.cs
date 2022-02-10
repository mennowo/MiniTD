using MiniTD.Helpers;

namespace MiniTD.DataTypes
{
    public class MiniTopic
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public System.Windows.Media.Color Color { get; set; }

        public MiniTopic()
        {
            ID = IDProvider.GetNextID();
        }
    }
}
