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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class TopicsListViewModel : ViewModelBase
    {

        #region Fields

        private MiniOrganizerViewModel _OrganizerVM;
        private string _NewTopicTitle;

        #endregion // Fields

        #region Properties

        public string NewTopicTitle
        {
            get { return _NewTopicTitle; }
            set
            {
                _NewTopicTitle = value;
                OnPropertyChanged("NewTopicTitle");
            }
        }


        public ObservableCollection<MiniTopicViewModel> Topics
        {
            get
            {
                return _OrganizerVM.Topics;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddNewTopicCommand;
        public ICommand AddNewTopicCommand
        {
            get
            {
                if (_AddNewTopicCommand == null)
                {
                    _AddNewTopicCommand = new RelayCommand(AddNewTopicCommand_Executed, AddNewTopicCommand_CanExecute);
                }
                return _AddNewTopicCommand;
            }
        }
        
        #endregion // Commands

        #region Command functionality

        void AddNewTopicCommand_Executed(object prm)
        {
            MiniTopic t = new MiniTopic();
            Random r = new Random(DateTime.Now.GetHashCode());
            t.Color = System.Windows.Media.Color.FromRgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
            t.Title = NewTopicTitle;
            MiniTopicViewModel tvm = new MiniTopicViewModel(t);
            Topics.Add(tvm);

            NewTopicTitle = "";
        }

        bool AddNewTopicCommand_CanExecute(object prm)
        {
            return !string.IsNullOrWhiteSpace(NewTopicTitle);
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public TopicsListViewModel(MiniOrganizerViewModel _origanizervm)
        {
            _OrganizerVM = _origanizervm;
        }

        #endregion // Constructor
    }
}
