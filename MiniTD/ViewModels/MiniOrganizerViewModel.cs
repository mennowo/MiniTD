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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTD.ViewModels
{
    public class MiniOrganizerViewModel : ViewModelBase
    {
        #region Fields

        private MiniDataProvider _DataProvider;
        private MiniOrganizer _Organizer;
        private TaskGatherViewModel _TasksGatherVM;
        private TaskProcessViewModel _TasksProcessVM;
        private TopicsListViewModel _TopicsListVM;
        private ProjectManagerViewModel _ProjectManagerVM;
        private CurrentTasksViewModel _CurrentTasksVM;

        private ObservableCollection<MiniTaskViewModel> _GatheredTasks;
        private ObservableCollection<MiniTaskViewModel> _AllTasks;
        private ObservableCollection<MiniTopicViewModel> _Topics;

        private bool _HasChanged;

        #endregion // Fields

        #region Properties

        public MiniOrganizer Organizer
        {
            get { return _Organizer; }
        }

        public bool HasChanged
        {
            get { return _HasChanged; }
            set
            {
                _HasChanged = value;
                OnPropertyChanged("HasChanged");
            }
        }

        public ObservableCollection<MiniTaskViewModel> GatheredTasks
        {
            get
            {
                if (_GatheredTasks == null)
                {
                    _GatheredTasks = new ObservableCollection<MiniTaskViewModel>();
                    OnPropertyChanged("GatheredTasks");
                }
                return _GatheredTasks;
            }
        }

        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get
            {
                if (_Topics == null)
                {
                    _Topics = new ObservableCollection<MiniTopicViewModel>();
                    OnPropertyChanged("Topics");
                }
                return _Topics;
            }
            set
            {
                _Topics = value;
                OnPropertyChanged("Topics");
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

        public TaskGatherViewModel TasksGatherVM
        {
            get { return _TasksGatherVM; }
            set
            {
                _TasksGatherVM = value;
                OnPropertyChanged("TasksGatherVM");
            }
        }

        public TaskProcessViewModel TasksProcessVM
        {
            get { return _TasksProcessVM; }
            set
            {
                _TasksProcessVM = value;
                OnPropertyChanged("TasksProcessVM");
            }
        }

        public TopicsListViewModel TopicsListVM
        {
            get { return _TopicsListVM; }
            set
            {
                _TopicsListVM = value;
                OnPropertyChanged("TopicsListVM");
            }
        }

        public ProjectManagerViewModel ProjectManagerVM
        {
            get { return _ProjectManagerVM; }
            set
            {
                _ProjectManagerVM = value;
                OnPropertyChanged("ProjectManagerVM");
            }
        }

        public CurrentTasksViewModel CurrentTasksVM
        {
            get { return _CurrentTasksVM; }
            set
            {
                _CurrentTasksVM = value;
                OnPropertyChanged("CurrentTasksVM");
            }
        }

        #endregion // Properties

        #region Events

        public delegate void TasksChangedHandler(EventArgs e);

        public event TasksChangedHandler TasksChanged;

        public void OnTasksChanged()
        {
            if (TasksChanged != null)
            {
                TasksChanged(EventArgs.Empty);
            }
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
            foreach(MiniTopicViewModel tvm in Topics)
            {
                if (tvm.ID == topicid)
                    return tvm;
            }
            return null;
        }

        #endregion // Public methods

        #region Collection Changed

        private void Topics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MiniTopicViewModel tvm in e.NewItems)
                {
                    _Organizer.Topics.Add(tvm.Topic);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTopicViewModel tvm in e.OldItems)
                {
                    _Organizer.Topics.Remove(tvm.Topic);
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
                    _Organizer.TaskInbox.Add(tvm.Task);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    _Organizer.TaskInbox.Remove(tvm.Task);
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
                    _Organizer.AllTasks.Add(tvm.Task);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MiniTaskViewModel tvm in e.OldItems)
                {
                    _Organizer.AllTasks.Remove(tvm.Task);
                }
            }
            OnTasksChanged();
            HasChanged = true;
        }

        #endregion // Collection Changed

        #region Constructor

        public MiniOrganizerViewModel(MiniDataProvider _provider)
        {
            _DataProvider = _provider;
            _Organizer = _DataProvider.Organizer;
            
            foreach (MiniTopic t in Organizer.Topics)
            {
                MiniTopicViewModel tvm = new MiniTopicViewModel(t, this);
                Topics.Add(tvm);
            }
            foreach (MiniTask t in Organizer.TaskInbox)
            {
                MiniTaskViewModel tvm = new MiniTaskViewModel(t, this, null);
                GatheredTasks.Add(tvm);
            }
            foreach (MiniTask t in Organizer.AllTasks)
            {
                MiniTaskViewModel tvm = new MiniTaskViewModel(t, this, null);
                AllTasks.Add(tvm);
            }

            TopicsListVM = new TopicsListViewModel(this);
            TasksProcessVM = new TaskProcessViewModel(this);
            TasksGatherVM = new TaskGatherViewModel(this);
            ProjectManagerVM = new ProjectManagerViewModel(this);
            CurrentTasksVM = new CurrentTasksViewModel(this);

            GatheredTasks.CollectionChanged += GatheredTasks_CollectionChanged;
            Topics.CollectionChanged += Topics_CollectionChanged;
            AllTasks.CollectionChanged += AllTasks_CollectionChanged;

            this.OnTasksChanged();
        }

        #endregion // Constructor
    }
}
