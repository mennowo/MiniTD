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
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniTD.ViewModels
{
    public class MiniOrganizerViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizer _organizer;
        private TaskGatherViewModel _tasksGatherVM;
        private TaskProcessViewModel _tasksProcessVM;
        private TopicsListViewModel _topicsListVM;
        private ProjectManagerViewModel _projectManagerVM;
        private CurrentTasksViewModel _currentTasksVM;
        private TasksPlanningViewModel _tasksPlanningVM;

        private ObservableCollection<MiniTaskViewModel> _gatheredTasks;
        private ObservableCollection<MiniTaskViewModel> _allTasks;
        private ObservableCollection<MiniTopicViewModel> _topics;

        private bool _hasChanged;

        #endregion // Fields

        #region Properties

        public MiniOrganizer Organizer => _organizer;

        public bool HasChanged
        {
            get => _hasChanged;
            set
            {
                _hasChanged = value;
                OnPropertyChanged("HasChanged");
            }
        }

        public ObservableCollection<MiniTaskViewModel> GatheredTasks
        {
            get
            {
                if (_gatheredTasks != null) return _gatheredTasks;
                _gatheredTasks = new ObservableCollection<MiniTaskViewModel>();
                OnPropertyChanged("GatheredTasks");
                return _gatheredTasks;
            }
        }

        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get
            {
                if (_topics != null) return _topics;
                _topics = new ObservableCollection<MiniTopicViewModel>();
                OnPropertyChanged("Topics");
                return _topics;
            }
            set
            {
                _topics = value;
                OnPropertyChanged("Topics");
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

        public TaskGatherViewModel TasksGatherVM
        {
            get => _tasksGatherVM;
            set
            {
                _tasksGatherVM = value;
                OnPropertyChanged("TasksGatherVM");
            }
        }

        public TaskProcessViewModel TasksProcessVM
        {
            get => _tasksProcessVM;
            set
            {
                _tasksProcessVM = value;
                OnPropertyChanged("TasksProcessVM");
            }
        }

        public TopicsListViewModel TopicsListVM
        {
            get => _topicsListVM;
            set
            {
                _topicsListVM = value;
                OnPropertyChanged("TopicsListVM");
            }
        }

        public ProjectManagerViewModel ProjectManagerVM
        {
            get => _projectManagerVM;
            set
            {
                _projectManagerVM = value;
                OnPropertyChanged("ProjectManagerVM");
            }
        }

        public CurrentTasksViewModel CurrentTasksVM
        {
            get => _currentTasksVM;
            set
            {
                _currentTasksVM = value;
                OnPropertyChanged("CurrentTasksVM");
            }
        }

        public TasksPlanningViewModel TasksPlanningVM
        {
            get => _tasksPlanningVM;
            set
            {
                _tasksPlanningVM = value;
                OnPropertyChanged("TasksPlanningVM");
            }
        }

        #endregion // Properties

        #region Events
        
        public event EventHandler TasksChanged;

        public void OnTasksChanged()
        {
            TasksChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnTaskDoneChanged(object sender, MiniTaskViewModel e)
        {
            
        }

        #endregion // Events

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        public MiniTopicViewModel GetTopicVMFromID(long topicid)
        {
            return Topics.FirstOrDefault(tvm => tvm.ID == topicid);
        }

        #endregion // Public methods

        #region Collection Changed

        private void Topics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTopicViewModel tvm in e.NewItems)
                {
                    _organizer.Topics.Add(tvm.Topic);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTopicViewModel tvm in e.OldItems)
                {
                    _organizer.Topics.Remove(tvm.Topic);
                }
            }
            HasChanged = true;
        }

        private void GatheredTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.NewItems)
                {
                    // this is always a task, never a project
                    tvm.Task.Type = MiniTaskType.Task;
                    _organizer.TaskInbox.Add(tvm.Task);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    _organizer.TaskInbox.Remove(tvm.Task);
                }
            }
            HasChanged = true;
        }

        private void AllTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.NewItems)
                {
                    _organizer.AllTasks.Add(tvm.Task);
                    tvm.DoneChanged += OnTaskDoneChanged;
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    _organizer.AllTasks.Remove(tvm.Task);
                    tvm.DoneChanged -= OnTaskDoneChanged;
                }
            }
            OnTasksChanged();
            HasChanged = true;
        }

        #endregion // Collection Changed

        #region Constructor

        public MiniOrganizerViewModel(MiniDataProvider provider)
        {
            _organizer = provider.Organizer;
            
            foreach (var t in Organizer.Topics)
            {
                var tvm = new MiniTopicViewModel(t, this);
                Topics.Add(tvm);
            }
            foreach (var t in Organizer.TaskInbox)
            {
                var tvm = new MiniTaskViewModel(t, this, null);
                GatheredTasks.Add(tvm);
            }
            foreach (var t in Organizer.AllTasks)
            {
                var tvm = new MiniTaskViewModel(t, this, null);
                AllTasks.Add(tvm);
            }

            TopicsListVM = new TopicsListViewModel(this);
            TasksProcessVM = new TaskProcessViewModel(this);
            TasksGatherVM = new TaskGatherViewModel(this);
            ProjectManagerVM = new ProjectManagerViewModel(this);
            CurrentTasksVM = new CurrentTasksViewModel(this);
            TasksPlanningVM = new TasksPlanningViewModel(this);

            GatheredTasks.CollectionChanged += GatheredTasks_CollectionChanged;
            Topics.CollectionChanged += Topics_CollectionChanged;
            AllTasks.CollectionChanged += AllTasks_CollectionChanged;

            foreach (var t in AllTasks)
            {
                t.DoneChanged += OnTaskDoneChanged;
            }

            OnTasksChanged();
        }

        #endregion // Constructor
    }
}
