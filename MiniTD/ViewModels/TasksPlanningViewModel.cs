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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;

namespace MiniTD.ViewModels
{
    public class TasksPlanningViewModel : ViewModelBase
    {
        #region Fields
         
        private MiniOrganizerViewModel _OrganizerVM;
        private ObservableCollection<MiniTaskViewModel> _CurrentTasks;
        private MiniTaskViewModel _SelectedTask;
        Timer _UpdateClockTimer;

        #endregion // Fields

        #region Properties

        ListCollectionView _CurrentTasksGrouped;
        public ListCollectionView CurrentTasksGrouped
        {
            get
            {
                if(_CurrentTasksGrouped == null)
                {
                    _CurrentTasksGrouped = new ListCollectionView(CurrentTasks);
                }
                return _CurrentTasksGrouped;
            }
        }

        public ObservableCollection<MiniTaskViewModel> CurrentTasks
        {
            get
            {
                if (_CurrentTasks == null)
                {
                    _CurrentTasks = new ObservableCollection<MiniTaskViewModel>();
                    OnPropertyChanged("CurrentTasks");
                }
                return _CurrentTasks;
            }
        }

        public DateTime CurrentTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public MiniTaskViewModel SelectedTask
        {
            get { return _SelectedTask; }
            set
            {
                _SelectedTask = value;
                _SelectedTask.IsSelected = true;
                _SelectedTask.IsExpanded = true;
                OnPropertyChanged("SelectedTask");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Tasks Changed

        private void _OrganizerVM_TasksChanged(EventArgs e)
        {
            CurrentTasks.Clear();
            foreach(MiniTaskViewModel tvm in _OrganizerVM.AllTasks)
            {
                if (tvm.Type == DataTypes.MiniTaskType.Task && !tvm.Done && tvm.IsCurrent)
                {
                    CurrentTasks.Add(tvm);
                    tvm.DoneChanged += Tttvm_DoneChanged;
                }
                foreach (MiniTaskViewModel ttvm in GetAllCurrentProjectTasks(tvm))
                {
                    CurrentTasks.Add(ttvm);
                    ttvm.DoneChanged += Tttvm_DoneChanged;
                }
            }
        }

        List<MiniTaskViewModel> GetAllCurrentProjectTasks(MiniTaskViewModel tvm)
        {
            List<MiniTaskViewModel> currentTasks = new List<MiniTaskViewModel>();
            
            // if the project has tasks
            if (tvm.AllTasks != null && tvm.AllTasks.Count > 0)
            {
                // loop all tasks
                foreach (MiniTaskViewModel ttvm in tvm.AllTasks)
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
                        foreach (MiniTaskViewModel tttvm in GetAllCurrentProjectTasks(ttvm))
                        {
                            currentTasks.Add(tttvm);
                        }
                    }
                }
            }
            return currentTasks;
        }

        private void Tttvm_DoneChanged(object sender, MiniTaskViewModel.DoneChangedEventArgs e)
        {
            if(e.tvm.Done || !e.tvm.IsCurrent)
            {
                CurrentTasks.Remove(e.tvm);
            }
        }

        #endregion Tasks Changed

        #region Constructor

        public TasksPlanningViewModel(MiniOrganizerViewModel _organizervm)
        {
            _OrganizerVM = _organizervm;
            _OrganizerVM.TasksChanged += _OrganizerVM_TasksChanged;
            _UpdateClockTimer = new Timer();
            _UpdateClockTimer.Interval = 1000;
            _UpdateClockTimer.AutoReset = true;
            _UpdateClockTimer.Elapsed += _UpdateClockTimer_Elapsed;
            _UpdateClockTimer.Start();

            CurrentTasksGrouped.GroupDescriptions.Add(new PropertyGroupDescription("DateDueGroup"));
            CurrentTasksGrouped.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateDueSort", System.ComponentModel.ListSortDirection.Ascending));

        }

        private void _UpdateClockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("CurrentTime");
        }

        #endregion // Constructor
    }
}
