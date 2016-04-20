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
using MiniTD.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class ProjectManagerViewModel : ViewModelBase
    {
        #region Fields

        private MiniOrganizerViewModel _OrganizerVM;
        private MiniTaskViewModel _CurrentSelectedTask;
        private object _SelectedItem;
        private bool _ShowDone;

        #endregion // Fields

        #region Properties

        public CurrentTasksViewModel CurrentTasksVM
        {
            get
            {
                return _OrganizerVM.CurrentTasksVM;
            }
        }

        public MiniTaskViewModel CurrentSelectedTask
        {
            get { return _CurrentSelectedTask; }
            set
            {
                _CurrentSelectedTask = value;
                SelectedItem = value;
            }
        }

        public ObservableCollection<MiniTaskViewModel> AllTasks
        {
            get
            {
                return _OrganizerVM.AllTasks;
            }
        }

        public object SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public bool ShowDone
        {
            get { return _ShowDone; }
            set
            {
                _ShowDone = value;
                SetAllFilterDone();
                OnPropertyChanged("ShowDone");
            }
        }

        #endregion // Properties

        #region Commands


        RelayCommand _AddProjectCommand;
        public ICommand AddProjectCommand
        {
            get
            {
                if (_AddProjectCommand == null)
                {
                    _AddProjectCommand = new RelayCommand(AddProjectCommand_Executed, AddProjectCommand_CanExecute);
                }
                return _AddProjectCommand;
            }
        }
        
        #endregion // Commands

        #region Command functionality

        void AddProjectCommand_Executed(object prm)
        {
            MiniTask t = new MiniTask();
            t.Type = MiniTaskType.Project;
            t.Title = "New project";
            AllTasks.Add(new MiniTaskViewModel(t, _OrganizerVM, null));
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
                ICollectionView iv = CollectionViewSource.GetDefaultView(AllTasks);
                iv.Filter = FilterDone;
                foreach (MiniTaskViewModel t in AllTasks)
                {
                    t.SetFilterDone(FilterDone);
                }
            }
            else
            {
                ICollectionView iv = CollectionViewSource.GetDefaultView(AllTasks);
                iv.Filter = null;
                foreach (MiniTaskViewModel t in AllTasks)
                {
                    t.SetFilterDone(null);
                }
            }
        }

        private bool FilterDone(object item)
        {
            MiniTaskViewModel mtv = item as MiniTaskViewModel;
            return mtv.Done == false;
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public ProjectManagerViewModel(MiniOrganizerViewModel _organizervm)
        {
            _OrganizerVM = _organizervm;
            SetAllFilterDone();
        }

        #endregion // Constructor
    }

    public class currentToItalicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return FontStyles.Italic;
            }
            else
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
            bool done = (bool)value;
            if (done)
            {
                return TextDecorations.Strikethrough;
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
