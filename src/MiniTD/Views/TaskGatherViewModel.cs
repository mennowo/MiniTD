using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class TaskGatherViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizerViewModel _OrganizerVM;
        private string _NewTaskTitle;
        private MiniTopicViewModel _NewTaskTopic;
        private MiniTaskViewModel _SelectedTask;

        #endregion // Fields

        #region Properties

        public string NewTaskTitle
        {
            get { return _NewTaskTitle; }
            set
            {
                _NewTaskTitle = value;
                OnPropertyChanged("NewTaskTitle");
            }
        }

        public MiniTopicViewModel NewTaskTopic
        {
            get { return _NewTaskTopic; }
            set
            {
                _NewTaskTopic = value;
                OnPropertyChanged("NewTaskTopic");
            }
        }

        public MiniTaskViewModel SelectedTask
        {
            get { return _SelectedTask; }
            set
            {
                _SelectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        public ObservableCollection<MiniTaskViewModel> GatheredTasks
        {
            get { return _OrganizerVM.GatheredTasks; }
        }

        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get { return _OrganizerVM.Topics; }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddTaskCommand;
        public ICommand AddTaskCommand
        {
            get
            {
                if (_AddTaskCommand == null)
                {
                    _AddTaskCommand = new RelayCommand(AddTaskCommand_Executed, AddTaskCommand_CanExecute);
                }
                return _AddTaskCommand;
            }
        }
        
        #endregion // Commands

        #region Command functionality

        void AddTaskCommand_Executed(object prm)
        {
            // Create Task
            var t = new MiniTask
            {
                Title = NewTaskTitle,
                TopicID = NewTaskTopic.ID,
                DateCreated = DateTime.Now
            };

            // Add to viewmodel
            var tvm = new MiniTaskViewModel(t, _OrganizerVM, null);
            GatheredTasks.Add(tvm);

            // Reset new title
            NewTaskTitle = "";
        }

        bool AddTaskCommand_CanExecute(object prm)
        {
            return !string.IsNullOrWhiteSpace(NewTaskTitle) && !(NewTaskTopic == null);
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region CollectionChanged

        #endregion // CollectionChanged

        #region Constructor

        public TaskGatherViewModel(MiniOrganizerViewModel _organizervm)
        {
            _OrganizerVM = _organizervm;
        }

        #endregion // Constructor
    }

    public class ColorToSolidColorBrushValueConverter : System.Windows.Data.IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is System.Windows.Media.Color)
            {
                var color = (System.Windows.Media.Color)value;
                return new System.Windows.Media.SolidColorBrush(color);
            }
            // You can support here more source types if you wish
            // For the example I throw an exception

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If necessary, here you can convert back. Check if which brush it is (if its one),
            // get its Color-value and return it.

            throw new NotImplementedException();
        }
    }
}
