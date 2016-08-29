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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
                OnMonitoredPropertyChanged("ParentTaskVM", OrganizerVM);
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
                OnMonitoredPropertyChanged("Title", OrganizerVM);
                foreach(MiniTaskViewModel mtvm in AllTasks)
                {
                    mtvm.OnPropertyChanged("ProjectTitle");
                }
            }
        }

        public string Outcome
        {
            get { return _Task.Outcome; }
            set
            {
                _Task.Outcome = value;
                OnMonitoredPropertyChanged("Outcome", OrganizerVM);
            }
        }

        public bool Done
        {
            get { return _Task.Done; }
            set
            {
                _Task.Done = value;
                DateDone = DateTime.Now;
                OnDoneChanged(new DoneChangedEventArgs() { tvm = this });
                OnMonitoredPropertyChanged("Done", OrganizerVM);
                OnPropertyChanged("IsCurrent");

                // if the Task becomes undone, we have recreate the current list
                // by calling OnTaskChanged, which will propagate to the current 
                // tasks viewmodel
                if(!_Task.Done)
                    OrganizerVM.OnTasksChanged();
            }
        }

        public DateTime DateDone
        {
            get { return _Task.DateDone; }
            set
            {
                _Task.DateDone = value;
                OnMonitoredPropertyChanged("DateDone", OrganizerVM);
                OnDoneChanged(new DoneChangedEventArgs() { tvm = this });
            }
        }

        public DateTime DateDue
        {
            get { return _Task.DateDue; }
            set
            {
                _Task.DateDue = value;
                OnMonitoredPropertyChanged("DateDue", OrganizerVM);

                SetIsCurrent();
            }
        }

        private static void GetWeek(DateTime now, System.Globalization.CultureInfo cultureInfo, out DateTime begining, out DateTime end)
        {
            if (now == null)
                throw new ArgumentNullException("Now");
            if (cultureInfo == null)
                throw new ArgumentNullException("CultureInfo");

            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            int offset = firstDayOfWeek - now.DayOfWeek;
            if (offset != 1)
            {
                DateTime weekStart = now.AddDays(offset);
                DateTime endOfWeek = weekStart.AddDays(6);
                begining = weekStart;
                end = endOfWeek;
            }
            else
            {
                begining = now.AddDays(-6);
                end = now;
            }
        }

        public DateTime DateDueSort
        {
            get
            {
                if (DateTime.Now.Date.CompareTo(DateDue.Date) >= 0)
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateDue;
                }
            }
        }

        public string DateDueGroup
        {
            get
            {

                DateTime first;
                DateTime last;
                GetWeek(DateTime.Now, System.Globalization.CultureInfo.CurrentCulture, out first, out last);
                DateTime nextweek = last.AddDays(7);

                if(DateTime.Now.Date.CompareTo(DateDue.Date) >= 0)
                {
                    return "Today (" + DateTime.Now.DayOfWeek.ToString() + ")";
                }
                else if (DateTime.Now.Date.AddDays(1).CompareTo(DateDue.Date) == 0)
                {
                    return "Tomorrow (" + DateDue.DayOfWeek.ToString() + ")";
                }
                else
                {
                    return DateDue.DayOfWeek.ToString() + " " + DateDue.ToString("dd-MM-yyyy");
                }
            }
        }

        public TimeSpan Duration
        {
            get { return _Task.Duration; }
            set
            {
                _Task.Duration = value;
                OnMonitoredPropertyChanged("Duration", OrganizerVM);
            }
        }

        public long TopicID
        {
            get { return _Task.TopicID; }
            set
            {
                _Task.TopicID = value;
                OnPropertyChanged("Topic");
                OnMonitoredPropertyChanged("TopicID", OrganizerVM);
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

        public IEnumerable<ValueDescription> StatusOptions
        {
            get
            {
                return EnumHelper.GetAllValuesAndDescriptions<MiniTaskStatus>();
            }
        }

        public MiniTaskStatus Status
        {
            get { return _Task.Status; }
            set
            {
                _Task.Status = value;
                OnMonitoredPropertyChanged("Status", OrganizerVM);
                OnPropertyChanged("StatusHasDelegatedTo");
                OnPropertyChanged("StatusHasDueDate");
                OnPropertyChanged("IsASAP");
                OnPropertyChanged("IsDelegated");
                OnPropertyChanged("IsInactive");
                OnPropertyChanged("IsScheduled");

                SetIsCurrent();
            }
        }

        public MiniTaskType Type
        {
            get { return _Task.Type; }
            set
            {
                _Task.Type = value;
                OnMonitoredPropertyChanged("Type", OrganizerVM);
                OnPropertyChanged("IsProject");
                OnPropertyChanged("IsTask");
            }
        }

        public long ProjectID
        {
            get { return _Task.ProjectID; }
            set
            {
                _Task.ProjectID = value;
                OnPropertyChanged("ProjectID");
                OnPropertyChanged("ProjectTitle");
            }
        }

        public string ProjectTitle
        {
            get
            {
                if (ParentTaskVM != null) return ParentTaskVM.Title;
                else return null;
            }
        }

        public bool IsProject
        {
            get { return Type == MiniTaskType.Project; }
            set
            {
                if (value)
                {
                    Type = MiniTaskType.Project;
                }
            }
        }

        public bool IsTask
        {
            get { return Type == MiniTaskType.Task; }
            set
            {
                if (value)
                {
                    Type = MiniTaskType.Task;
                }
            }
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
                OnMonitoredPropertyChanged("DelegatedTo", OrganizerVM);
            }
        }
        
        public bool IsCurrent
        {
            get
            {
                return this.Status == MiniTaskStatus.ASAP ||
                    (this.Status == MiniTaskStatus.Delegated && this.DateDue.Date <= DateTime.Today.Date) ||
                    (this.Status == MiniTaskStatus.Scheduled && this.DateDue.Date <= DateTime.Today.Date);
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
                if(_IsExpanded && ParentTaskVM != null)
                {
                    ParentTaskVM.IsExpanded = value;
                }
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

        #region Events


        public delegate void OnDoneChangedEventHandler(object sender, DoneChangedEventArgs e);
        public event OnDoneChangedEventHandler DoneChanged;

        public class DoneChangedEventArgs : EventArgs
        {
            public MiniTaskViewModel tvm { get; set; }
        }

        public void OnDoneChanged(DoneChangedEventArgs e)
        {
            // raise event
            OnDoneChangedEventHandler handler = DoneChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion // Events

        #region Commands

        RelayCommand _AddNewProjectCommand;
        public ICommand AddNewProjectCommand
        {
            get
            {
                if (_AddNewProjectCommand == null)
                {
                    _AddNewProjectCommand = new RelayCommand(AddNewProjectCommand_Executed, AddNewProjectCommand_CanExecute);
                }
                return _AddNewProjectCommand;
            }
        }

        RelayCommand _AddNewTaskCommand;
        public ICommand AddNewTaskCommand
        {
            get
            {
                if (_AddNewTaskCommand == null)
                {
                    _AddNewTaskCommand = new RelayCommand(AddNewTaskCommand_Executed, AddNewTaskCommand_CanExecute);
                }
                return _AddNewTaskCommand;
            }
        }

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
            return AllTasks != null;
        }

        bool AddNewTaskCommand_CanExecute(object prm)
        {
            return AllTasks != null;
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

        public void SetFilterDone(Predicate<object> predicate)
        {
            ICollectionView iv = CollectionViewSource.GetDefaultView(AllTasks);
            iv.Filter = predicate;
            foreach(MiniTaskViewModel t in AllTasks)
            {
                t.SetFilterDone(predicate);
            }
        }

        private void SetIsCurrent()
        {
            // if the Task becomes current, we have recreate the current list
            // by calling OnTaskChanged, which will propagate to the current 
            // tasks viewmodel
            OrganizerVM.OnTasksChanged();
            OnPropertyChanged("IsCurrent");
        }
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
            OrganizerVM.HasChanged = true;
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
            OrganizerVM.HasChanged = true;
            OrganizerVM.OnTasksChanged();
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
