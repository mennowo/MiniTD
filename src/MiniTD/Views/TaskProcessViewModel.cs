using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class TaskProcessViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizerViewModel _organizerVM;
        private string _newProjectTitle;
        private long _newProjectTopicID;
        private RelayCommand _processCurrentTaskCommand;
        
        #endregion // Fields

        #region Properties

        public MiniTaskViewModel CurrentTask => _organizerVM.GatheredTasks.Count > 0 ? _organizerVM.GatheredTasks[0] : null;

        public bool HasUnprocessedTasks => _organizerVM.GatheredTasks.Count > 0;

        public int GatheredTaskCount => _organizerVM.GatheredTasks.Count == 0 ? 0 : _organizerVM.GatheredTasks.Count;

        public ObservableCollection<MiniTopicViewModel> Topics => _organizerVM.Topics;

        public ObservableCollection<MiniTaskViewModel> AllTasks => _organizerVM.AllTasks;

        public IEnumerable<MiniTaskViewModel> AllProjects
        {
            get { return AllTasks.Where(x => x.Type == MiniTaskType.Project && x.Done == false).SelectMany(x => x.GetAllProjects()); }
        }

        public bool IsAddedToNewProject => !string.IsNullOrWhiteSpace(NewProjectTitle);

        public bool IsAddedToExistingProject => string.IsNullOrWhiteSpace(NewProjectTitle);

        public MiniTaskStatus CurrentTaskStatus
        {
            get => CurrentTask?.Status ?? MiniTaskStatus.ASAP;
            set
            {
                CurrentTask.Status = value;
                OnPropertyChanged(null);
            }
        }

        public bool CurrentTaskDone
        {
            get => CurrentTask != null && CurrentTask.Done;
            set
            {
                CurrentTask.Done = value;
                OnPropertyChanged("CurrentTaskDone");
            }
        }

        public bool CurrentTaskStatusHasDelegatedTo => CurrentTask?.Status == MiniTaskStatus.Delegated;

        public bool CurrentTaskStatusHasDueDate
        {
            get
            {
                if (CurrentTask != null)
                    return CurrentTask.Status == MiniTaskStatus.Delegated || CurrentTask.Status == MiniTaskStatus.Scheduled;
                return false;
            }
        }

        public string NewProjectTitle
        {
            get => _newProjectTitle;
            set
            {
                _newProjectTitle = value;
                OnPropertyChanged("NewProjectTitle");
            }
        }

	    public long NewProjectTopicID
	    {
		    get => _newProjectTopicID;
		    set
		    {
			    _newProjectTopicID = value;
			    OnPropertyChanged("NewProjectTopicID");
		    }
	    }

		#endregion // Properties

		#region Commands

        public ICommand ProcessCurrentTaskCommand => _processCurrentTaskCommand ??= new RelayCommand(ProcessCurrentTaskCommand_Executed, ProcessCurrentTaskCommand_CanExecute);

        #endregion // Commands

        #region Command functionality

        void ProcessCurrentTaskCommand_Executed(object prm)
        {
            if(!string.IsNullOrWhiteSpace(NewProjectTitle))
            {
                // Create and add project
                var p = new MiniTask
                {
                    Title = NewProjectTitle,
                    Type = MiniTaskType.Project,
                    DateDone = DateTime.Now,
					TopicID = NewProjectTopicID
                };

                // Create view model for project, add task to project, add project to organizer
                var tvm = new MiniTaskViewModel(p, _organizerVM, null);
	            CurrentTask.ParentTaskVM = tvm;
                tvm.AllTasks.Add(CurrentTask);
                AllTasks.Add(tvm);
                
                // Remove from gathered list
                _organizerVM.GatheredTasks.Remove(CurrentTask);

                NewProjectTitle = "";
                OnPropertyChanged("GatheredTaskCount");
                OnPropertyChanged("AllProjects");
            }
            else
            {
                MiniTaskViewModel task = null;
                foreach (var tvm in AllProjects)
                {
                    if(tvm.ID == CurrentTask.ProjectID)
                    {
                        task = tvm;
                    }
                }
                if (task == null) return;
                // Add task to project
	            CurrentTask.ParentTaskVM = task;
                task.AllTasks.Add(CurrentTask);

				// Remove from gathered list
				_organizerVM.GatheredTasks.Remove(CurrentTask);
            }
        }

        private bool ProcessCurrentTaskCommand_CanExecute(object prm)
        {
            return  !string.IsNullOrWhiteSpace(CurrentTask?.Title) && !(CurrentTask.ProjectID == 0 && string.IsNullOrWhiteSpace(NewProjectTitle));
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Collection Changed

        private void GatheredTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update the view!
            OnPropertyChanged(null);
        }

        #endregion // Collection Changed

        #region Constructor

        public TaskProcessViewModel(MiniOrganizerViewModel organizervm)
        {
            _organizerVM = organizervm;
            _organizerVM.GatheredTasks.CollectionChanged += GatheredTasks_CollectionChanged;
            _organizerVM.TasksChanged += _OrganizerVM_TasksChanged;

        }

        private void _OrganizerVM_TasksChanged(object sender, EventArgs e)
        {
            // Update the view!
            OnPropertyChanged(null);
        }

        #endregion // Constructor
    }
}
