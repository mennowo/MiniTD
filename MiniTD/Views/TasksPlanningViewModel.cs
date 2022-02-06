/*
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
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

        #endregion // Fields

        #region Properties

        [UsedImplicitly]
        public ListCollectionView CurrentTasksGrouped => _currentTasksGrouped ?? (_currentTasksGrouped = new ListCollectionView(CurrentTasks));

        [UsedImplicitly]
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

        [UsedImplicitly]
        public DateTime CurrentTime => DateTime.Now;

        [UsedImplicitly]
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

        #endregion // Public methods

        #region Tasks Changed

        private void _OrganizerVM_TasksChanged(object sender, EventArgs e)
        {
            var sel = SelectedTask; 
            CurrentTasks.Clear();
            foreach(var tvm in _organizerVM.AllTasks)
            {
                if (tvm.Type == DataTypes.MiniTaskType.Task && !tvm.Done && tvm.IsCurrent)
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
            if (tvm.AllTasks != null && tvm.AllTasks.Count > 0)
            {
                // loop all tasks
                foreach (var ttvm in tvm.AllTasks)
                {
                    // if a task is not done or inactive
                    if (!ttvm.Done && !ttvm.IsInactive)
                    {
                        // if type is task, add it
                        if (ttvm.Type == DataTypes.MiniTaskType.Task)
                        {
                            currentTasks.Add(ttvm);
                        }
                    }
                    // if it has tasks, add them all
                    if (ttvm.AllTasks != null && ttvm.AllTasks.Count > 0)
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
}
