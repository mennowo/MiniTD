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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTD.ViewModels
{
    public class MiniTaskNoteViewModel : ViewModelBase
    {
        #region Fields

        private MiniTaskNote _Note;

        #endregion // Fields

        #region Properties

        public MiniTaskNote Note
        {
            get { return _Note; }
            set
            {
                _Note = value;
                OnPropertyChanged("Note");
            }
        }

        public string NoteText
        {
            get
            {
                return _Note.Note;
            }
            set
            {
                _Note.Note = value;
                OnPropertyChanged("Note");
            }
        }

        public DateTime DateCreated
        {
            get
            { return _Note.DateCreated; }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public MiniTaskNoteViewModel(MiniTaskNote note)
        {
            Note = note;
        }

        public MiniTaskNoteViewModel()
        {
        }

        #endregion // Constructor
    }
}
