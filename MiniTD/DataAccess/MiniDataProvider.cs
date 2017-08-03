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
