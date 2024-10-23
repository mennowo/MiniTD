using MiniTD.DataTypes;
using MiniTD.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using System.Threading.Tasks;
using System.Linq;

namespace MiniTD.ViewModels
{
    public class MiniTaskDropHandler : DefaultDropHandler
    {
        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetCollection is ObservableCollection<MiniTaskViewModel> target &&
                dropInfo.DragInfo.SourceCollection is ObservableCollection<MiniTaskViewModel> source &&
                dropInfo.Data is MiniTaskViewModel vm)
            {
                source.Remove(vm);
                var index = dropInfo.InsertIndex;
                if (ReferenceEquals(source, target) && index > dropInfo.DragInfo.SourceIndex) index--;
                var i = 0;
                var j = 0;
                for (; j < index && i < target.Count; ++i)
                {
                    if (!target[i].Done) ++j;
                }
                target.Insert(i, vm);
            }
        }
    }
    
    public class ProjectManagerViewModel : ViewModelBase
    {
        #region Fields

        private readonly MiniOrganizerViewModel _organizerVM;
        
        private object _selectedItem;
        private bool _showDone;
        private RelayCommand _addProjectCommand;
        
        #endregion // Fields

        #region Properties
        
        public MiniTaskDropHandler MiniTaskDropHandler { get; } = new();

        public CurrentTasksViewModel CurrentTasksVM => _organizerVM.CurrentTasksVM;
        
        public ObservableCollection<MiniTaskViewModel> AllTasks => _organizerVM.AllTasks;

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                _organizerVM.SetSelectedTask(this, (MiniTaskViewModel)value);
                OnPropertyChanged("SelectedItem");
            }
        }

        public object SelectedItemJustSet
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public bool ShowDone
        {
            get => _showDone;
            set
            {
                _showDone = value;
                SetAllFilterDone();
                OnPropertyChanged("ShowDone");
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddProjectCommand => _addProjectCommand ??= new RelayCommand(AddProjectCommand_Executed, AddProjectCommand_CanExecute);

        #endregion // Commands

        #region Command functionality

        void AddProjectCommand_Executed(object prm)
        {
            var t = new MiniTask
            {
                Type = MiniTaskType.Project,
                Title = "New project"
            };
            AllTasks.Add(new MiniTaskViewModel(t, _organizerVM, null));
        }
        
        bool AddProjectCommand_CanExecute(object prm)
        {
            return true;
        }

        #endregion // Command functionality

        #region Private methods

        private void SetAllFilterDone()
        {
            if (!ShowDone)
            {
                var iv = CollectionViewSource.GetDefaultView(AllTasks);
                iv.Filter = FilterDone;
                foreach (var t in AllTasks)
                {
                    t.SetFilterDone(FilterDone);
                }
            }
            else
            {
                var iv = CollectionViewSource.GetDefaultView(AllTasks);
                iv.Filter = null;
                foreach (var t in AllTasks)
                {
                    t.SetFilterDone(null);
                }
            }
        }

        private bool FilterDone(object item)
        {
            var mtv = item as MiniTaskViewModel;
            return mtv != null && mtv.Done == false;
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public ProjectManagerViewModel(MiniOrganizerViewModel organizervm)
        {
            _organizerVM = organizervm;
            AllTasks.CollectionChanged += AllTasks_CollectionChanged;
            SetAllFilterDone();
        }

        private void AllTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _organizerVM.OnTasksChanged();
        }

        internal void SetSelectedTask(object sender, MiniTaskViewModel task)
        {
            if (ReferenceEquals(this, sender)) return;
            SelectedItemJustSet = task == null ? null : AllTasks?.FirstOrDefault(x => x.ID == task.ID);
        }

        #endregion // Constructor
    }

    public class CurrentToItalicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
            {
                return FontStyles.Italic;
            }
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoneToTextDecorationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var done = value != null && (bool)value;
            return done ? TextDecorations.Strikethrough : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (TreeViewItem)value;
            var ic = ItemsControl.ItemsControlFromItemContainer(item);
            return item != null && ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
