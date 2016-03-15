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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class MiniTaskViewModel : ViewModelBase
    {
        #region Fields

        private MiniOrganizerViewModel _OrganizerVM;
        private MiniTaskViewModel _ParentTaskVM;
        private MiniTask _Task;
        private ObservableCollection<MiniTaskViewModel> _AllTasks;
        private ObservableCollection<MiniTaskNoteViewModel> _Notes;

        private bool _IsExpanded;
        private bool _IsSelected;

        #endregion // Fields

        #region Properties

        public MiniTaskViewModel ParentTaskVM
        {
            get { return _ParentTaskVM; }
            set
            {
                _ParentTaskVM = value;
                OnPropertyChanged("ParentTaskVM");
            }
        }

        public MiniTask Task
        {
            get { return _Task; }
            set { _Task = value; }
        }

        public string Title
        {
            get { return _Task.Title; }
            set
            {
                _Task.Title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Outcome
        {
            get { return _Task.Outcome; }
            set
            {
                _Task.Outcome = value;
                OnPropertyChanged("Outcome");
            }
        }

        public bool Done
        {
            get { return _Task.Done; }
            set
            {
                _Task.Done = value;
                DateDone = DateTime.Now;
                OnPropertyChanged("Done");
            }
        }

        public DateTime DateDone
        {
            get { return _Task.DateDone; }
            set
            {
                _Task.DateDone = value;
                OnPropertyChanged("DateDone");
            }
        }

        public DateTime DateDue
        {
            get { return _Task.DateDue; }
            set
            {
                _Task.DateDue = value;
                OnPropertyChanged("DateDue");
            }
        }

        public long TopicID
        {
            get { return _Task.TopicID; }
            set
            {
                _Task.TopicID = value;
                OnPropertyChanged("Topic");
                OnPropertyChanged("TopicID");
            }
        }

        public long ProjectID
        {
            get { return _Task.ProjectID; }
            set
            {
                _Task.ProjectID = value;
                OnPropertyChanged("ProjectID");
            }
        }

        public MiniTopicViewModel Topic
        {
            get
            {
                return _OrganizerVM.GetTopicVMFromID(TopicID);
            }
            set
            {
                TopicID = value.ID;
                OnPropertyChanged("Topic");
                OnPropertyChanged("TopicID");
            }
        }

        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get { return _OrganizerVM.Topics; }
        }

        public IEnumerable<ValueDescription> StatusOptions =>
             EnumHelper.GetAllValuesAndDescriptions<MiniTaskStatus>();

        public MiniTaskStatus Status
        {
            get { return _Task.Status; }
            set
            {
                _Task.Status = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusHasDelegatedTo");
                OnPropertyChanged("StatusHasDueDate");
                OnPropertyChanged("IsASAP");
                OnPropertyChanged("IsDelegated");
                OnPropertyChanged("IsInactive");
                OnPropertyChanged("IsScheduled");
            }
        }

        public MiniTaskType Type
        {
            get { return _Task.Type; }
            set
            {
                _Task.Type = value;
                OnPropertyChanged("Type");
                OnPropertyChanged("IsProject");
                OnPropertyChanged("IsTask");
            }
        }

        public bool IsProject
        {
            get { return Type == MiniTaskType.Project; }
        }

        public bool IsTask
        {
            get { return Type == MiniTaskType.Task; }
        }

        public long ID
        {
            get { return _Task.ID; }
        }

        public bool StatusHasDelegatedTo
        {
            get { return _Task.Status == MiniTaskStatus.Delegated; }
        }


        public bool StatusHasDueDate
        {
            get { return _Task.Status == MiniTaskStatus.Delegated || _Task.Status == MiniTaskStatus.Scheduled; }
        }

        public string DelegatedTo
        {
            get { return _Task.DelegatedTo; }
            set
            {
                _Task.DelegatedTo = value;
                OnPropertyChanged("DelegatedTo");
            }
        }

        public bool IsASAP
        {
            get { return Status == MiniTaskStatus.ASAP; }
        }

        public bool IsScheduled
        {
            get { return Status == MiniTaskStatus.Scheduled; }
        }

        public bool IsDelegated
        {
            get { return Status == MiniTaskStatus.Delegated; }
        }

        public bool IsInactive
        {
            get { return Status == MiniTaskStatus.Inactive; }
        }

        public ObservableCollection<MiniTaskNoteViewModel> Notes
        {
            get
            {
                if (_Notes == null)
                {
                    _Notes = new ObservableCollection<MiniTaskNoteViewModel>();
                    OnPropertyChanged("Notes");
                }
                return _Notes;
            }
        }

        public ObservableCollection<MiniTaskViewModel> AllTasks
        {
            get
            {
                if (_AllTasks == null)
                {
                    _AllTasks = new ObservableCollection<MiniTaskViewModel>();
                    OnPropertyChanged("AllTasks");
                }
                return _AllTasks;
            }
            set
            {
                _AllTasks = value;
                OnPropertyChanged("AllTasks");
            }
        }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                _IsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public MiniOrganizerViewModel OrganizerVM
        {
            get { return _OrganizerVM; }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _RemoveMeCommand;
        public ICommand RemoveMeCommand
        {
            get
            {
                if (_RemoveMeCommand == null)
                {
                    _RemoveMeCommand = new RelayCommand(RemoveMeCommand_Executed, RemoveMeCommand_CanExecute);
                }
                return _RemoveMeCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewTaskCommand_Executed(object prm)
        {
            MiniTask t = new MiniTask();
            t.Title = "New task";
            MiniTaskViewModel tvm = new MiniTaskViewModel(t, OrganizerVM, this);
            AllTasks.Add(tvm);
        }

        void AddNewProjectCommand_Executed(object prm)
        {
            MiniTask t = new MiniTask();
            t.Type = MiniTaskType.Project;
            t.Title = "New project";
            MiniTaskViewModel tvm = new MiniTaskViewModel(t, OrganizerVM, this);
            AllTasks.Add(tvm);
        }

        bool AddNewProjectCommand_CanExecute(object prm)
        {
            return true;
        }

        bool AddNewTaskCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveMeCommand_Executed(object prm)
        {
            if (ParentTaskVM != null)
                ParentTaskVM.AllTasks.Remove(this);
            else
                OrganizerVM.AllTasks.Remove(this);
        }

        bool RemoveMeCommand_CanExecute(object prm)
        {
            return true;
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Overrides

        public override string ToString()
        {
            return Title;
        }

        #endregion // Overrides

        #region Collection Changed

        private void Notes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Propagate to Model
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTaskNoteViewModel tvm in e.NewItems)
                {
                    MiniTaskNote n = new MiniTaskNote();
                    tvm.Note = n;
                    _Task.Notes.Add(n);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskNoteViewModel tvm in e.OldItems)
                {
                    _Task.Notes.Remove(tvm.Note);
                }
            }
        }

        private void AllTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.NewItems)
                {
                    _Task.AllTasks.Add(tvm.Task);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    _Task.AllTasks.Remove(tvm.Task);
                }
            }
        }

        #endregion

        #region Constructor

        public MiniTaskViewModel(MiniTask task, MiniOrganizerViewModel _organizervm, MiniTaskViewModel _taskvm)
        {
            _Task = task;
            _OrganizerVM = _organizervm;
            _ParentTaskVM = _taskvm;

            foreach (MiniTaskNote n in _Task.Notes)
            {
                MiniTaskNoteViewModel tnvn = new MiniTaskNoteViewModel(n);
                Notes.Add(tnvn);
            }

            foreach (MiniTask t in _Task.AllTasks)
            {
                MiniTaskViewModel tvn = new MiniTaskViewModel(t, _OrganizerVM, this);
                AllTasks.Add(tvn);
            }

            AllTasks.CollectionChanged += AllTasks_CollectionChanged;
            Notes.CollectionChanged += Notes_CollectionChanged;
        }
        
        #endregion // Constructor
    }
}
