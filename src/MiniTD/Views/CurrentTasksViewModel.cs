﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Data;
using MiniTD.Helpers;
using Xceed.Wpf.Toolkit.Primitives;

namespace MiniTD.ViewModels
{
    public class CurrentTasksViewModel : ViewModelBase
    {
        #region Fields
         
        private readonly MiniOrganizerViewModel _organizerVM;
        private ObservableCollection<MiniTaskViewModel> _currentTasks;
        private MiniTaskViewModel _selectedTask;
        private ListCollectionView _currentTasksGrouped;

        #endregion // Fields

        #region Properties

        public ListCollectionView CurrentTasksGrouped => _currentTasksGrouped ?? (_currentTasksGrouped = new ListCollectionView(CurrentTasks));

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
                if (CurrentTasks.Contains(value))
                {
                    _selectedTask = value;
                    if (value != null)
                    {
                        if ((_selectedTask.Done || _selectedTask.AnyParentDone) && !_organizerVM.ProjectManagerVM.ShowDone)
                        {
                            _organizerVM.ProjectManagerVM.ShowDone = true;
                        }
                        _selectedTask.IsSelected = true;
                        _selectedTask.IsExpanded = true;
                    }
                }
                else
                {
                    _selectedTask = null;
                }
                OnPropertyChanged("SelectedTask");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        private void _UpdateClockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("CurrentTime");
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Tasks Changed

        private void _OrganizerVM_TasksChanged(object sender, EventArgs e)
        {
            var sel = SelectedTask;
            SelectedTask = null;
            CurrentTasks.Clear();
            foreach (var tvm in _organizerVM.AllTasks)
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
            SelectedTask = sel;
            OnPropertyChanged("SelectedTask");
        }

        IEnumerable<MiniTaskViewModel> GetAllCurrentProjectTasks(MiniTaskViewModel tvm)
        {
            var currentTasks = new List<MiniTaskViewModel>();
            
            // return if the project no tasks
            if (tvm.AllTasks == null || tvm.AllTasks.Count <= 0) return currentTasks;

            // loop all tasks
            foreach (var ttvm in tvm.AllTasks)
            {
                // if a task is not done or inactive
                if (!ttvm.Done && ttvm.IsCurrent)
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
                    currentTasks.AddRange(GetAllCurrentProjectTasks(ttvm));
                }
            }
            return currentTasks;
        }

        internal void SetSelectedTask(object sender, MiniTaskViewModel task)
        {
            if (ReferenceEquals(this, sender)) return;
            SelectedTask = task == null ? null : CurrentTasks?.FirstOrDefault(x => x.ID == task.ID);
        }

        #endregion Tasks Changed

        #region Constructor

        public CurrentTasksViewModel(MiniOrganizerViewModel organizervm)
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
            CurrentTasksGrouped.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));

	        CurrentTasks.CollectionChanged += (o, e) =>
	        {
		        if (e.OldItems != null && e.OldItems.Count > 0)
		        {
			        _organizerVM.TasksChanged -= _OrganizerVM_TasksChanged;
					foreach (MiniTaskViewModel mtvm in e.OldItems)
			        {
						if (mtvm.ParentTaskVM != null)
							mtvm.ParentTaskVM.AllTasks.Remove(mtvm);
						else
							_organizerVM.AllTasks.Remove(mtvm);
					}
			        _organizerVM.TasksChanged += _OrganizerVM_TasksChanged;
		        }
	        };

        }

        #endregion // Constructor
    }
}
