using MiniTD.DataTypes;
using System;

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
