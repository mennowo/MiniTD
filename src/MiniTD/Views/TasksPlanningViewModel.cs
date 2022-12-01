using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using MiniTD.DataTypes;
using MiniTD.Helpers;

namespace MiniTD.ViewModels
{
    public class TasksPlanningViewModel : ViewModelBase
    {
        #region Fields
         
        private readonly MiniOrganizerViewModel _organizerVM;
        private ObservableCollection<MiniTaskViewModel> _currentTasks;
        private MiniTaskViewModel _selectedTask;
        private ListCollectionView _currentTasksGrouped;
	    private TaskPlanningDropTarget _dropTarget;
        private RelayCommand _moveTaskOneWeekFormardCommand;
        private bool _displayWeekends;

        #endregion // Fields

        #region Properties

        public ListCollectionView CurrentTasksGrouped => _currentTasksGrouped ??= new ListCollectionView(CurrentTasks);

        public ObservableCollection<MiniTaskViewModel> CurrentTasks
        {
            get
            {
                if (_currentTasks != null) return _currentTasks;
                _currentTasks = new ObservableCollection<MiniTaskViewModel>();
                OnPropertyChanged("CurrentTasks");
                return _currentTasks;
            }
        }

        public DateTime CurrentTime => DateTime.Now;

        public MiniTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                if (value != null)
                {
                    _selectedTask.IsSelected = true;
                    _selectedTask.IsExpanded = true;
                }
                OnPropertyChanged("SelectedTask");
            }
        }

	    public IDropTarget DropHandler => _dropTarget ??= new TaskPlanningDropTarget();

        public bool DisplayWeekends
        {
            get => _displayWeekends;
            set
            {
                _displayWeekends = value; 
                OnPropertyChanged(nameof(DisplayWeekends));
                OnPropertyChanged(nameof(DayColumns));
            }
        }

        public int DayColumns => DisplayWeekends ? 7 : 5;

        public List<DisplayWeek> DisplayWeeks { get; set; }
        
	    #endregion // Properties

        #region Commands

        public ICommand MoveTaskOneWeekFormardCommand => _moveTaskOneWeekFormardCommand ??= new RelayCommand(_ =>
        {
            if (SelectedTask.DateDue < DateTime.Now)
            {
                SelectedTask.DateDue = DateTime.Now;
            }
            SelectedTask.DateDue = SelectedTask.DateDue.AddDays(7);
            if (SelectedTask.Status == MiniTaskStatus.Inactive ||
                SelectedTask.Status == MiniTaskStatus.ASAP)
            {
                SelectedTask.Status = MiniTaskStatus.Scheduled;
            }
        }, _ => SelectedTask != null);
        
        #endregion // Commands

        #region Private methods

        #endregion // Private methods

        #region Public methods

        private void UpdateWeeklySchedule()
        {
            var tasks = new List<MiniTaskViewModel>();
            
            foreach(var tvm in _organizerVM.AllTasks
                .Where(x => !x.Done && x.Status != MiniTaskStatus.Inactive))
            {
                if (tvm.Type == MiniTaskType.Task && !tvm.Done && tvm.IsCurrent)
                {
                    tasks.Add(tvm);
                }
                foreach (var ttvm in GetAllCurrentProjectTasks(tvm)
                    .Where(x => !x.Done && x.Status != MiniTaskStatus.Inactive && x.Type == MiniTaskType.Task))
                {
                    tasks.Add(ttvm);
                }
            }
            
            tasks.Sort((x, y) => x.DateDue.CompareTo(y.DateDue));

            var dayNo = (int) DateTime.Now.DayOfWeek;
            dayNo = dayNo == 0 ? 6 : --dayNo;
            var firstWeekDate = DateTime.Now.Date.AddDays(-1 * dayNo);
            var weeks = new List<DisplayWeek>();
            var lastWeekDate = tasks.Max(x => x.DateDue);
            dayNo = (int) lastWeekDate.DayOfWeek;
            dayNo = dayNo == 0 ? 6 : --dayNo;
            lastWeekDate = lastWeekDate.AddDays(-1 * dayNo).Date;

            var cTask = 0;

            while (firstWeekDate <= lastWeekDate)
            {
                var week = new DisplayWeek
                {
                    FirstDay = firstWeekDate
                };
                for (int day = 0; day < 7; day++)
                {
                    var date = firstWeekDate.AddDays(day).Date;
                    var nDay = new DisplayDay(_organizerVM)
                    {
                        Date = date,
                        IsFree = _organizerVM.Organizer.FreeDays.Any(x => x.Date.Date == date)
                    };
                    nDay.IsFreeChanged += DisplayDay_IsFreeChanged;
                    week.Days.Add(nDay);
                    while (tasks.Count > cTask && tasks[cTask].DateDue.Date <= nDay.Date)
                    {
                        nDay.Tasks.Add(tasks[cTask]);
                        ++cTask;
                    }
                }

                weeks.Add(week);
                firstWeekDate = firstWeekDate.AddDays(7);
            }

            if (DisplayWeeks != null && DisplayWeeks.Count > 0)
            {
                foreach (var week in DisplayWeeks)
                {
                    foreach (var day in week.Days)
                    {
                        day.IsFreeChanged -= DisplayDay_IsFreeChanged;
                    }
                }
            }

            DisplayWeeks = weeks;
            OnPropertyChanged(nameof(DisplayWeeks));
        }

        private void DisplayDay_IsFreeChanged(object sender, EventArgs e)
        {
            if (sender is DisplayDay day)
            {
                if (day.IsFree)
                {
                    _organizerVM.Organizer.FreeDays.Add(new MiniFreeDay { Date = day.Date });
                }
                else
                {
                    var r = _organizerVM.Organizer.FreeDays.FirstOrDefault(x => x.Date.Date == day.Date.Date);
                    if (r != null)
                    {
                        _organizerVM.Organizer.FreeDays.Remove(r);
                    }
                }
            }
        }

        #endregion // Public methods

        #region Tasks Changed

        private void _OrganizerVM_TasksChanged(object sender, EventArgs e)
        {
            UpdateWeeklySchedule();
            
            var sel = SelectedTask; 
            CurrentTasks.Clear();
            foreach(var tvm in _organizerVM.AllTasks)
            {
                if (tvm.Type == MiniTaskType.Task && !tvm.Done && tvm.IsCurrent)
                {
                    CurrentTasks.Add(tvm);
                }
                foreach (var ttvm in GetAllCurrentProjectTasks(tvm))
                {
                    CurrentTasks.Add(ttvm);
                }
            }
            SelectedTask = CurrentTasks.Contains(sel) ? sel : null;
        }

        List<MiniTaskViewModel> GetAllCurrentProjectTasks(MiniTaskViewModel tvm)
        {
            var currentTasks = new List<MiniTaskViewModel>();
            
            // if the project has tasks
            if (tvm.AllTasks is { Count: > 0 })
            {
                // loop all tasks
                foreach (var ttvm in tvm.AllTasks)
                {
                    // if a task is not done or inactive
                    if (!ttvm.Done && !ttvm.IsInactive)
                    {
                        // if type is task, add it
                        if (ttvm.Type == MiniTaskType.Task)
                        {
                            currentTasks.Add(ttvm);
                        }
                    }
                    // if it has tasks, add them all
                    if (ttvm.AllTasks is { Count: > 0 })
                    {
                        foreach (var tttvm in GetAllCurrentProjectTasks(ttvm))
                        {
                            currentTasks.Add(tttvm);
                        }
                    }
                }
            }
            return currentTasks;
        }

        #endregion Tasks Changed

        #region Constructor

        public TasksPlanningViewModel(MiniOrganizerViewModel organizervm)
        {
            _organizerVM = organizervm;
            _organizerVM.TasksChanged += _OrganizerVM_TasksChanged;
            var updateClockTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };
            updateClockTimer.Elapsed += _UpdateClockTimer_Elapsed;
            updateClockTimer.Start();

            CurrentTasksGrouped.GroupDescriptions?.Add(new PropertyGroupDescription("DateDueGroup"));
            CurrentTasksGrouped.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateDueSort", System.ComponentModel.ListSortDirection.Ascending));
            CurrentTasksGrouped.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));

        }

        private void _UpdateClockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("CurrentTime");
        }

        #endregion // Constructor
    }
    
    public class DisplayWeek
    {
        public DateTime FirstDay { get; set; }

        public string FriendlyFirstDay => FirstDay.ToLongDateString();

        public string WeekNumber => "Week " + GetIso8601WeekOfYear(FirstDay); 

        public List<DisplayDay> Days { get; } = new();
        
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat. If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day is >= DayOfWeek.Monday and <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

    public class DisplayDay : ViewModelBase
    {
        private string _timeNeededDescription;
        private TimeSpan _timeNeeded;
        private CalendarPlanningDropTarget _dropTarget;
        private MiniTaskViewModel _selectedTask;
        private bool _isFree;
        private readonly MiniOrganizerViewModel _organizerVM;

        public event EventHandler IsFreeChanged;

        public DateTime Date { get; set; }

        public IDropTarget DropHandler => _dropTarget ??= new CalendarPlanningDropTarget();

        public string FriendlyDate => Date.ToShortDateString();

        public bool IsFree
        {
            get => _isFree; 
            set
            {
                _isFree = value;
                IsFreeChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(nameof(IsFree));
            }
        }

        public TimeSpan TimeNeeded
        {
            get => _timeNeeded;
            private set
            {
                _timeNeeded = value;
                OnPropertyChanged(nameof(TimeNeeded));
            }
        }

        public string TimeNeededDescription => _timeNeededDescription;

        private void UpdateTimeNeeded()
        {
            var total = TimeSpan.Zero;
            foreach (var task in Tasks)
            {
                total = total.Add(task.Duration);
            }

            TimeNeeded = total;
            _timeNeededDescription = "Time: " + total.ToString(@"hh\:mm");
        }

        public ObservableCollection<MiniTaskViewModel> Tasks { get; }

        public MiniTaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                _organizerVM.CurrentTasksVM.SelectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public DisplayDay(MiniOrganizerViewModel organizerVM)
        {
            _organizerVM = organizerVM;
            Tasks = new ObservableCollection<MiniTaskViewModel>();
            Tasks.CollectionChanged += TasksOnCollectionChanged;
        }

        private void TasksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTimeNeeded();
        }
    }
}
