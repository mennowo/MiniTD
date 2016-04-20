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

using MiniTD.DataAccess;
using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class TaskProcessViewModel : ViewModelBase
    {
        #region Fields

        private MiniOrganizerViewModel _OrganizerVM;
        private string _NewProjectTitle;
        
        #endregion // Fields

        #region Properties

        public MiniTaskViewModel CurrentTask
        {
            get
            {
                if (_OrganizerVM.GatheredTasks.Count > 0)
                    return _OrganizerVM.GatheredTasks[0];
                else
                    return null;
            }
        }

        public int GatheredTaskCount
        {
            get { return _OrganizerVM.GatheredTasks.Count == 0 ? 0 : _OrganizerVM.GatheredTasks.Count; }
        }

        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get { return _OrganizerVM.Topics; }
        }

        public ObservableCollection<MiniTaskViewModel> AllTasks
        {
            get { return _OrganizerVM.AllTasks; }
        }

        public IEnumerable<MiniTaskViewModel> AllProjects
        {
            get { return from task in _OrganizerVM.AllTasks where task.Type == MiniTaskType.Project select task; }
        }

        public bool IsAddedToNewProject
        {
            get { return !string.IsNullOrWhiteSpace(NewProjectTitle); }
        }
        public bool IsAddedToExistingProject
        {
            get { return string.IsNullOrWhiteSpace(NewProjectTitle); }
        }

        public MiniTaskStatus CurrentTaskStatus
        {
            get { return CurrentTask.Status; }
            set
            {
                CurrentTask.Status = value;
                OnPropertyChanged("CurrentTaskStatus");
                OnPropertyChanged("CurrentTaskStatusHasDelegatedTo");
                OnPropertyChanged("CurrentTaskStatusHasDueDate");
            }
        }

        public bool CurrentTaskDone
        {
            get { return CurrentTask.Done; }
            set
            {
                CurrentTask.Done = value;
                OnPropertyChanged("CurrentTaskDone");
            }
        }

        public bool CurrentTaskStatusHasDelegatedTo
        {
            get { return CurrentTask.Status == MiniTaskStatus.Delegated; }
        }


        public bool CurrentTaskStatusHasDueDate
        {
            get { return CurrentTask.Status == MiniTaskStatus.Delegated || CurrentTask.Status == MiniTaskStatus.Scheduled; }
        }

        public IEnumerable<ValueDescription> StatusOptions
        {
            get
            {
                return EnumHelper.GetAllValuesAndDescriptions<MiniTaskStatus>();
            }
        }

        public string NewProjectTitle
        {
            get { return _NewProjectTitle; }
            set
            {
                _NewProjectTitle = value;
                OnPropertyChanged("NewProjectTitle");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _ProcessCurrentTaskCommand;
        public ICommand ProcessCurrentTaskCommand
        {
            get
            {
                if (_ProcessCurrentTaskCommand == null)
                {
                    _ProcessCurrentTaskCommand = new RelayCommand(ProcessCurrentTaskCommand_Executed, ProcessCurrentTaskCommand_CanExecute);
                }
                return _ProcessCurrentTaskCommand;
            }
        }
        
        #endregion // Commands

        #region Command functionality

        void ProcessCurrentTaskCommand_Executed(object prm)
        {
            MiniTaskViewModel mtvm = CurrentTask;
            if(!string.IsNullOrWhiteSpace(NewProjectTitle))
            {
                // Create and add project
                MiniTask p = new MiniTask();
                p.Title = NewProjectTitle;
                p.Type = MiniTaskType.Project;

                // Create view model for project, add task to project, add project to organizer
                MiniTaskViewModel tvm = new MiniTaskViewModel(p, _OrganizerVM, null);
                tvm.AllTasks.Add(CurrentTask);
                AllTasks.Add(tvm);
                
                // Remove from gathered list
                _OrganizerVM.GatheredTasks.Remove(CurrentTask);

                NewProjectTitle = "";
                OnPropertyChanged("GatheredTaskCount");
                OnPropertyChanged("AllProjects");
            }
            else
            {
                MiniTaskViewModel task = null;
                foreach (MiniTaskViewModel tvm in AllProjects)
                {
                    if(tvm.ID == CurrentTask.ProjectID)
                    {
                        task = tvm;
                    }
                }
                if(task != null)
                {
                    // Add task to project
                    task.AllTasks.Add(CurrentTask);

                    // Remove from gathered list
                    _OrganizerVM.GatheredTasks.Remove(CurrentTask);
                }
            }
        }
        
        bool ProcessCurrentTaskCommand_CanExecute(object prm)
        {
            return  CurrentTask != null && 
                    !string.IsNullOrWhiteSpace(CurrentTask.Title) &&
                    !(CurrentTask.ProjectID == 0 && string.IsNullOrWhiteSpace(NewProjectTitle));
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Collection Changed

        private void AllTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update the view!
            OnPropertyChanged("GatheredTaskCount");
            OnPropertyChanged("CurrentTask");
            OnPropertyChanged("CurrentTaskStatus");
            OnPropertyChanged("CurrentTaskStatusHasDelegatedTo");
            OnPropertyChanged("CurrentTaskStatusHasDueDate");
            OnPropertyChanged("CurrentTaskDone");
        }

        private void GatheredTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update the view!
            OnPropertyChanged("GatheredTaskCount");
            OnPropertyChanged("CurrentTask");
            OnPropertyChanged("CurrentTaskStatus");
            OnPropertyChanged("CurrentTaskStatusHasDelegatedTo");
            OnPropertyChanged("CurrentTaskStatusHasDueDate");
            OnPropertyChanged("CurrentTaskDone");
        }

        #endregion // Collection Changed

        #region Constructor

        public TaskProcessViewModel(MiniOrganizerViewModel _organizervm)
        {
            _OrganizerVM = _organizervm;
            _OrganizerVM.GatheredTasks.CollectionChanged += GatheredTasks_CollectionChanged;
            _OrganizerVM.AllTasks.CollectionChanged += AllTasks_CollectionChanged;

        }

        #endregion // Constructor
    }
}
