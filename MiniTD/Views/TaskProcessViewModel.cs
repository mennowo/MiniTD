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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;

namespace MiniTD.ViewModels
{
    public class TaskProcessViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizerViewModel _organizerVM;
        private string _newProjectTitle;
        private long _newProjectTopicID;
        
        #endregion // Fields

        #region Properties

        [UsedImplicitly]
        public MiniTaskViewModel CurrentTask => _organizerVM.GatheredTasks.Count > 0 ? _organizerVM.GatheredTasks[0] : null;

        [UsedImplicitly]
        public bool HasUnprocessedTasks => _organizerVM.GatheredTasks.Count > 0;

        [UsedImplicitly]
        public int GatheredTaskCount => _organizerVM.GatheredTasks.Count == 0 ? 0 : _organizerVM.GatheredTasks.Count;

        [UsedImplicitly]
        public ObservableCollection<MiniTopicViewModel> Topics => _organizerVM.Topics;

        [UsedImplicitly]
        public ObservableCollection<MiniTaskViewModel> AllTasks => _organizerVM.AllTasks;

        [UsedImplicitly]
        public IEnumerable<MiniTaskViewModel> AllProjects
        {
            get { return AllTasks.Where(x => x.Type == MiniTaskType.Project && x.Done == false).SelectMany(x => x.GetAllProjects()); }
        }

        [UsedImplicitly]
        public bool IsAddedToNewProject => !string.IsNullOrWhiteSpace(NewProjectTitle);

        [UsedImplicitly]
        public bool IsAddedToExistingProject => string.IsNullOrWhiteSpace(NewProjectTitle);

        [UsedImplicitly]
        public MiniTaskStatus CurrentTaskStatus
        {
            get => CurrentTask?.Status ?? MiniTaskStatus.ASAP;
            set
            {
                CurrentTask.Status = value;
                OnPropertyChanged(null);
            }
        }

        [UsedImplicitly]
        public bool CurrentTaskDone
        {
            get => CurrentTask != null && CurrentTask.Done;
            set
            {
                CurrentTask.Done = value;
                OnPropertyChanged("CurrentTaskDone");
            }
        }

        [UsedImplicitly]
        public bool CurrentTaskStatusHasDelegatedTo => CurrentTask?.Status == MiniTaskStatus.Delegated;

        [UsedImplicitly]
        public bool CurrentTaskStatusHasDueDate
        {
            get
            {
                if (CurrentTask != null)
                    return CurrentTask.Status == MiniTaskStatus.Delegated || CurrentTask.Status == MiniTaskStatus.Scheduled;
                return false;
            }
        }

        [UsedImplicitly]
        public string NewProjectTitle
        {
            get => _newProjectTitle;
            set
            {
                _newProjectTitle = value;
                OnPropertyChanged("NewProjectTitle");
            }
        }

	    [UsedImplicitly]
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

		RelayCommand _processCurrentTaskCommand;
        [UsedImplicitly]
        public ICommand ProcessCurrentTaskCommand => _processCurrentTaskCommand ?? (_processCurrentTaskCommand =
                                                         new RelayCommand(ProcessCurrentTaskCommand_Executed, ProcessCurrentTaskCommand_CanExecute));

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
