using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class MiniTaskViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizerViewModel _organizerVM;
        private MiniTaskViewModel _parentTaskVM;
        private ObservableCollection<MiniTaskViewModel> _allTasks;
        private ObservableCollection<MiniTaskNoteViewModel> _notes;

        private bool _isExpanded;
        private bool _isSelected;
        
        private RelayCommand _addNewProjectCommand;
        private RelayCommand _addNewTaskCommand;
        private RelayCommand _removeMeCommand;
        private RelayCommand _postponeCommand;

        #endregion // Fields

        #region Properties

        public MiniTaskViewModel ParentTaskVM
        {
            get => _parentTaskVM;
            set
            {
                _parentTaskVM = value;
                OnMonitoredPropertyChanged("ParentTaskVM", OrganizerVM);
            }
        }

        public MiniTask Task { get; }

        public string Title
        {
            get => Task.Title;
            set
            {
                Task.Title = value;
                OnMonitoredPropertyChanged("Title", OrganizerVM);
                foreach(var mtvm in AllTasks)
                {
                    mtvm.OnPropertyChanged("ProjectTitle");
                }
            }
        }

        public string Outcome
        {
            get => Task.Outcome;
            set
            {
                Task.Outcome = value;
                OnMonitoredPropertyChanged("Outcome", OrganizerVM);
            }
        }

        public bool AnyParentDone
        {
            get
            {
                var done = false;
                if (ParentTaskVM != null)
                    done = ParentTaskVM.Done || ParentTaskVM.AnyParentDone;
                return done;
            }
        }

        public bool Done
        {
            get => Task.Done;
            set
            {
                Task.Done = value;
                DateDone = DateTime.Now;
                DoneChanged?.Invoke(this, this);
                OnMonitoredPropertyChanged("Done", OrganizerVM);
                OnPropertyChanged(null);

                // if the Task becomes undone, we have recreate the current list
                // by calling OnTaskChanged, which will propagate to the current 
                // tasks viewmodel
                OrganizerVM.OnTasksChanged();
            }
        }

        public DateTime DateDone
        {
            get => Task.DateDone;
            set
            {
                Task.DateDone = value;
                OnMonitoredPropertyChanged("DateDone", OrganizerVM);
                DoneChanged?.Invoke(this, this);
            }
        }

        public DateTime DateDue
        {
            get => Task.DateDue;
            set
            {
                Task.DateDue = value;
                OnMonitoredPropertyChanged("DateDue", OrganizerVM);

                OrganizerVM.OnTasksChanged();
                SetIsCurrent();
            }
        }

        private static void GetWeek(DateTime now, System.Globalization.CultureInfo cultureInfo, out DateTime begining, out DateTime end)
        {
            if (cultureInfo == null)
                throw new ArgumentNullException(nameof(cultureInfo));

            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            var offset = firstDayOfWeek - now.DayOfWeek;
            if (offset != 1)
            {
                var weekStart = now.AddDays(offset);
                var endOfWeek = weekStart.AddDays(6);
                begining = weekStart;
                end = endOfWeek;
            }
            else
            {
                begining = now.AddDays(-6);
                end = now;
            }
        }

        public DateTime DateDueSort => DateTime.Now.Date.CompareTo(DateDue.Date) >= 0 ? DateTime.Now : DateDue;

        private double GetTotTaskTime(MiniTaskViewModel tvm, DateTime d)
        {
            var dd = 0.0;
            dd += AllTasks.Where(x => x.DateDue.Date <= d.Date).Select(x => x.Duration).Sum(x => x.TotalMinutes);
            if (tvm.AllTasks == null || tvm.AllTasks.Count <= 0)
                return dd;

            dd += tvm.AllTasks.Sum(tvm2 => GetTotTaskTime(tvm2, d));
            return dd;
        }

        private double GetTotalTime(DateTime d)
        {
            var dd = 0.0;
            dd += OrganizerVM.AllTasks.Where(x => x.DateDue.Date <= d.Date).Select(x => x.Duration).Sum(x => x.TotalMinutes);

            dd += OrganizerVM.AllTasks.Sum(tvm2 => GetTotTaskTime(tvm2, d));

            return dd;
        }

        public string DateDueGroup
        {
            get
            {
                if (DateTime.Now.Date.CompareTo(DateDue.Date) >= 0)
                {
                    return "Today (" + DateTime.Now.DayOfWeek + ")";
                }
                if (DateTime.Now.Date.AddDays(1).CompareTo(DateDue.Date) == 0)
                {
                    return "Tomorrow (" + DateDue.DayOfWeek + ")";
                }
                return DateDue.DayOfWeek + " " + DateDue.ToString("dd-MM-yyyy");
            }
        }

        public TimeSpan Duration
        {
            get => Task.Duration;
            set
            {
                Task.Duration = value;
                OrganizerVM.OnTasksChanged();
                OnMonitoredPropertyChanged("Duration", OrganizerVM);
            }
        }

        public TimeSpan TotalDuration
        {
            get
            {
                var t = new TimeSpan();
                t += Duration;
                return AllTasks?.Aggregate(t, (current, tvm) => current + tvm.TotalDuration) ?? t;
            }
        }

        public long TopicID
        {
            get => Task.TopicID;
            set
            {
                Task.TopicID = value;
                OnPropertyChanged("Topic");
                OnMonitoredPropertyChanged("TopicID", OrganizerVM);
            }
        }
        
        public MiniTopicViewModel Topic
        {
            get => _organizerVM.GetTopicVMFromID(TopicID);
            set
            {
                TopicID = value.ID;
                OnPropertyChanged("Topic");
                OnPropertyChanged("TopicID");
            }
        }

        public ObservableCollection<MiniTopicViewModel> Topics => _organizerVM.Topics;

        public MiniTaskStatus Status
        {
            get => Task.Status;
            set
            {
                Task.Status = value;
                OnMonitoredPropertyChanged(nameof(Status), OrganizerVM);
                SetIsCurrent();
                RaiseStatusChanged();
            }
        }

        public MiniTaskType Type
        {
            get => Task.Type;
            set
            {
                Task.Type = value;
                OnMonitoredPropertyChanged("Type", OrganizerVM);
                OnPropertyChanged(null);
            }
        }

        public long ProjectID
        {
            get => Task.ProjectID;
            set
            {
                Task.ProjectID = value;
                OnPropertyChanged(null);
            }
        }

        public string ProjectTitle => ParentTaskVM?.Title;

        public bool IsProject
        {
            get => Type == MiniTaskType.Project;
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
            get => Type == MiniTaskType.Task;
            set
            {
                if (value)
                {
                    Type = MiniTaskType.Task;
                }
            }
        }

        public long ID => Task.ID;

        public bool StatusHasDelegatedTo => Task.Status == MiniTaskStatus.Delegated;

        public bool StatusHasDueDate => Task.Status == MiniTaskStatus.Delegated || Task.Status == MiniTaskStatus.Scheduled;

        public string DelegatedTo
        {
            get => Task.DelegatedTo;
            set
            {
                Task.DelegatedTo = value;
                OnMonitoredPropertyChanged("DelegatedTo", OrganizerVM);
            }
        }
        
        public bool IsCurrent => Status == MiniTaskStatus.ASAP ||
                                 Status == MiniTaskStatus.Delegated && DateDue.Date <= DateTime.Today.Date ||
                                 Status == MiniTaskStatus.Scheduled && DateDue.Date <= DateTime.Today.Date;

        public bool IsASAP => Status == MiniTaskStatus.ASAP;

        public bool IsScheduled => Status == MiniTaskStatus.Scheduled;

        public bool IsDelegated => Status == MiniTaskStatus.Delegated;

        public bool IsInactive => Status == MiniTaskStatus.Inactive;

        public ObservableCollection<MiniTaskNoteViewModel> Notes
        {
            get
            {
                if (_notes != null) return _notes;
                _notes = new ObservableCollection<MiniTaskNoteViewModel>();
                OnPropertyChanged("Notes");
                return _notes;
            }
        }

        public ObservableCollection<MiniTaskViewModel> AllTasks
        {
            get
            {
                if (_allTasks != null) return _allTasks;
                _allTasks = new ObservableCollection<MiniTaskViewModel>();
                OnPropertyChanged("AllTasks");
                return _allTasks;
            }
            set
            {
                _allTasks = value;
                OnPropertyChanged("AllTasks");
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                if(_isExpanded && ParentTaskVM != null)
                {
                    ParentTaskVM.IsExpanded = value;
                }
                OnPropertyChanged("IsExpanded");
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public MiniOrganizerViewModel OrganizerVM => _organizerVM;

        #endregion // Properties

        #region Events

        public event EventHandler<MiniTaskViewModel> DoneChanged;

        #endregion // Events

        #region Commands

        public ICommand AddNewProjectCommand => _addNewProjectCommand ??= new RelayCommand(AddNewProjectCommand_Executed, AddNewProjectCommand_CanExecute);

        public ICommand AddNewTaskCommand => _addNewTaskCommand ??= new RelayCommand(AddNewTaskCommand_Executed, AddNewTaskCommand_CanExecute);

        public ICommand RemoveMeCommand => _removeMeCommand ??= new RelayCommand(RemoveMeCommand_Executed, RemoveMeCommand_CanExecute);
        
        public ICommand PostponeCommand => _postponeCommand ??= new RelayCommand(PostponeCommand_Executed, PostponeCommand_CanExecute);

        #endregion // Commands

        #region Command functionality

        private void AddNewTaskCommand_Executed(object prm)
        {
            var t = new MiniTask
            {
                Title = "New task",
                TopicID = TopicID
            };
            var tvm = new MiniTaskViewModel(t, OrganizerVM, this);
            AllTasks.Add(tvm);
            _organizerVM.CurrentTasksVM.SelectedTask = tvm;
        }

        private void AddNewProjectCommand_Executed(object prm)
        {
            var t = new MiniTask
            {
                Type = MiniTaskType.Project,
                Title = "New project",
                TopicID = TopicID
            };
            var tvm = new MiniTaskViewModel(t, OrganizerVM, this);
            AllTasks.Add(tvm);
            _organizerVM.CurrentTasksVM.SelectedTask = tvm;
        }

        private bool AddNewProjectCommand_CanExecute(object prm)
        {
            return AllTasks != null;
        }

        private bool AddNewTaskCommand_CanExecute(object prm)
        {
            return AllTasks != null;
        }

        private void RemoveMeCommand_Executed(object prm)
        {
            if (ParentTaskVM != null)
                ParentTaskVM.AllTasks.Remove(this);
            else
                OrganizerVM.AllTasks.Remove(this);
        }

        private bool RemoveMeCommand_CanExecute(object prm)
        {
            return true;
        }

        private void PostponeCommand_Executed(object prm)
        {
            var s = (string) prm;
            switch (s)
            {
                case "day":
                    DateDue = DateDue.AddDays(1);
                    break;
                case "week":
                    DateDue = DateDue.AddDays(7);
                    break;
            }
        }

        private bool PostponeCommand_CanExecute(object prm)
        {
            return true;
        }

        #endregion // Command functionality

        #region Private methods

        public void SetFilterDone(Predicate<object> predicate)
        {
            var iv = CollectionViewSource.GetDefaultView(AllTasks);
            iv.Filter = predicate;
            foreach(var t in AllTasks)
            {
                t.SetFilterDone(predicate);
            }
        }

        private void RaiseStatusChanged()
        {
            OnPropertyChanged(nameof(IsASAP));
            OnPropertyChanged(nameof(IsDelegated));
            OnPropertyChanged(nameof(IsInactive));
            OnPropertyChanged(nameof(IsScheduled));
            OnPropertyChanged(nameof(StatusHasDelegatedTo));
            OnPropertyChanged(nameof(StatusHasDueDate));
        }

        private void SetIsCurrent()
        {
            // if the Task becomes current, we have recreate the current list
            // by calling OnTaskChanged, which will propagate to the current 
            // tasks viewmodel
            OrganizerVM.OnTasksChanged();
            OnPropertyChanged(nameof(IsCurrent));
        }

        private void OnTaskDoneChanged(object sender, MiniTaskViewModel e)
        {
            DoneChanged?.Invoke(this, e);
        }

        #endregion // Private methods

        #region Public methods

        public IEnumerable<MiniTaskViewModel> GetAllProjects()
        {
            yield return this;
            foreach (var item in AllTasks.Where(x => x.Type == MiniTaskType.Project && x.Done == false).SelectMany(x => x.GetAllProjects()))
            {
                yield return item;
            }
        }

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
            if (e.NewItems is { Count: > 0 })
            {
                foreach (MiniTaskNoteViewModel tvm in e.NewItems)
                {
                    var n = new MiniTaskNote();
                    tvm.Note = n;
                    Task.Notes.Add(n);
                }
            }
            if (e.OldItems is { Count: > 0 })
            {
                foreach (MiniTaskNoteViewModel tvm in e.OldItems)
                {
                    Task.Notes.Remove(tvm.Note);
                }
            }
            OrganizerVM.HasChanged = true;
        }

        private void AllTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is { Count: > 0 })
            {
                foreach (MiniTaskViewModel tvm in e.NewItems)
                {
                    Task.AllTasks.Add(tvm.Task);
                    tvm.DoneChanged += OnTaskDoneChanged;
                }
            }
            if (e.OldItems is { Count: > 0 })
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    Task.AllTasks.Remove(tvm.Task);
                    tvm.DoneChanged -= OnTaskDoneChanged;
                }
            }
            OrganizerVM.HasChanged = true;
            OrganizerVM.OnTasksChanged();
        }

        #endregion

        #region Constructor

        public MiniTaskViewModel(MiniTask task, MiniOrganizerViewModel organizervm, MiniTaskViewModel taskvm)
        {
            Task = task;
            _organizerVM = organizervm;
            _parentTaskVM = taskvm;

            foreach (var n in Task.Notes)
            {
                var tnvn = new MiniTaskNoteViewModel(n);
                Notes.Add(tnvn);
            }

            foreach (var t in Task.AllTasks)
            {
                var tvn = new MiniTaskViewModel(t, _organizerVM, this);
                AllTasks.Add(tvn);
            }

            AllTasks.CollectionChanged += AllTasks_CollectionChanged;
            Notes.CollectionChanged += Notes_CollectionChanged;

            foreach (var t in AllTasks)
            {
                t.DoneChanged += OnTaskDoneChanged;
            }
        }
        
        #endregion // Constructor
    }
}
