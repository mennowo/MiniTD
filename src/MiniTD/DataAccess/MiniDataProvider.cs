using MiniTD.DataTypes;
using MiniTD.Helpers;

namespace MiniTD.DataAccess
{
    public class MiniDataProvider
    {
        #region Fields

        private MiniOrganizer _Organizer;
        private string _FileName;

        #endregion // Fields

        #region Properties

        public MiniOrganizer Organizer
        {
            get { return _Organizer; }
            set
            {
                _Organizer = value;
            }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
            }
        }

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Public methods

        public void NewOrganizer()
        {
            Organizer = new MiniOrganizer();
            IDProvider.Organizer = Organizer; 
            FileName = null;
        }

        public void LoadOrganizer()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                var deserializer = new DeserializeT<MiniOrganizer>();
                Organizer = deserializer.DeSerializeGZip(FileName);
                IDProvider.Organizer = Organizer;
            }
        }

        public void SaveOrganizer()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                var serializer = new SerializeT<MiniOrganizer>();
                serializer.SerializeGZip(FileName, Organizer);
            }
        }

        public void CloseOrganizer()
        {
            Organizer = null;
            IDProvider.Organizer = null;
            FileName = null;
        }

        #endregion // Public methods

        #region Constructor

        public MiniDataProvider()
        {

        }

        #endregion // Constructor
    }
}
