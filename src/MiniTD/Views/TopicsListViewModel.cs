using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class TopicsListViewModel : ViewModelBase
    {

        #region Fields

        private readonly MiniOrganizerViewModel _OrganizerVM;
        private string _NewTopicTitle;

        #endregion // Fields

        #region Properties

        public string NewTopicTitle
        {
            get { return _NewTopicTitle; }
            set
            {
                _NewTopicTitle = value;
                OnPropertyChanged("NewTopicTitle");
            }
        }


        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get
            {
                return _OrganizerVM.Topics;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddNewTopicCommand;
        public ICommand AddNewTopicCommand
        {
            get
            {
                if (_AddNewTopicCommand == null)
                {
                    _AddNewTopicCommand = new RelayCommand(AddNewTopicCommand_Executed, AddNewTopicCommand_CanExecute);
                }
                return _AddNewTopicCommand;
            }
        }
        
        #endregion // Commands

        #region Command functionality

        void AddNewTopicCommand_Executed(object prm)
        {
            var t = new MiniTopic();
            var r = new Random(DateTime.Now.GetHashCode());
            t.Color = System.Windows.Media.Color.FromRgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
            t.Title = NewTopicTitle;
            var tvm = new MiniTopicViewModel(t, _OrganizerVM);
            Topics.Add(tvm);

            NewTopicTitle = "";
        }

        bool AddNewTopicCommand_CanExecute(object prm)
        {
            return !string.IsNullOrWhiteSpace(NewTopicTitle);
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public TopicsListViewModel(MiniOrganizerViewModel _origanizervm)
        {
            _OrganizerVM = _origanizervm;
        }

        #endregion // Constructor
    }
}
