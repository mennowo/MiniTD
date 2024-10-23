using Microsoft.Win32;
using MiniTD.DataAccess;
using MiniTD.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MiniTD.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private MiniOrganizerViewModel _OrganizerVM;
        private readonly MiniDataProvider _DataProvider;

        #endregion // Fields

        #region Properties

        public MiniOrganizerViewModel OrganizerVM
        {
            get { return _OrganizerVM; }
            set
            {
                _OrganizerVM = value;
                OnPropertyChanged("OrganizerVM");
                OnPropertyChanged("HasOrganizer");
            }
        }

        public bool HasOrganizer
        {
            get { return OrganizerVM != null; }
        }

        public MiniDataProvider DataProvider
        {
            get { return _DataProvider; }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _NewFileCommand;
        public ICommand NewFileCommand
        {
            get
            {
                if (_NewFileCommand == null)
                {
                    _NewFileCommand = new RelayCommand(NewFileCommand_Executed, NewFileCommand_CanExecute);
                }
                return _NewFileCommand;
            }
        }

        RelayCommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                if (_OpenFileCommand == null)
                {
                    _OpenFileCommand = new RelayCommand(OpenFileCommand_Executed, OpenFileCommand_CanExecute);
                }
                return _OpenFileCommand;
            }
        }

        RelayCommand _SaveFileCommand;
        public ICommand SaveFileCommand
        {
            get
            {
                if (_SaveFileCommand == null)
                {
                    _SaveFileCommand = new RelayCommand(SaveFileCommand_Executed, SaveFileCommand_CanExecute);
                }
                return _SaveFileCommand;
            }
        }

        RelayCommand _SaveAsFileCommand;
        public ICommand SaveAsFileCommand
        {
            get
            {
                if (_SaveAsFileCommand == null)
                {
                    _SaveAsFileCommand = new RelayCommand(SaveAsFileCommand_Executed, SaveAsFileCommand_CanExecute);
                }
                return _SaveAsFileCommand;
            }
        }


        RelayCommand _CloseFileCommand;
        public ICommand CloseFileCommand
        {
            get
            {
                if (_CloseFileCommand == null)
                {
                    _CloseFileCommand = new RelayCommand(CloseFileCommand_Executed, CloseFileCommand_CanExecute);
                }
                return _CloseFileCommand;
            }
        }

        RelayCommand _ExitApplicationCommand;
        public ICommand ExitApplicationCommand
        {
            get
            {
                if (_ExitApplicationCommand == null)
                {
                    _ExitApplicationCommand = new RelayCommand(ExitApplicationCommand_Executed, ExitApplicationCommand_CanExecute);
                }
                return _ExitApplicationCommand;
            }
        }

        RelayCommand _ShowAboutDialogCommand;
        public ICommand ShowAboutDialogCommand
        {
            get
            {
                if (_ShowAboutDialogCommand == null)
                {
                    _ShowAboutDialogCommand = new RelayCommand(ShowAboutDialogCommand_Executed, ShowAboutDialogCommand_CanExecute);
                }
                return _ShowAboutDialogCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void NewFileCommand_Executed(object prm)
        {
            if (!OrganizerHasChanged())
            {
                DataProvider.NewOrganizer();
                OrganizerVM = new MiniOrganizerViewModel(DataProvider);
            }
        }
        
        bool NewFileCommand_CanExecute(object prm)
        {
            return true;
        }

        void OpenFileCommand_Executed(object prm)
        {
            if (!OrganizerHasChanged())
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = "MiniTD files|*.mtd";
                if (openFileDialog.ShowDialog() == true)
                {
                    DataProvider.FileName = openFileDialog.FileName;
                    DataProvider.LoadOrganizer();
                    OrganizerVM = new MiniOrganizerViewModel(DataProvider);
                }
            }
        }

        bool OpenFileCommand_CanExecute(object prm)
        {
            return true;
        }

        void SaveFileCommand_Executed(object prm)
        {
            if (string.IsNullOrWhiteSpace(DataProvider.FileName))
                SaveAsFileCommand.Execute(null);
            else
            {
                DataProvider.SaveOrganizer();
                OrganizerVM.HasChanged = false;
            }
        }

        bool SaveFileCommand_CanExecute(object prm)
        {
            return OrganizerVM != null;
        }
        
        void SaveAsFileCommand_Executed(object prm)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "MiniTD files|*.mtd";
            if (!string.IsNullOrWhiteSpace(DataProvider.FileName))
                saveFileDialog.FileName = DataProvider.FileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                DataProvider.FileName = saveFileDialog.FileName;
                DataProvider.SaveOrganizer();
                OrganizerVM.HasChanged = false;
            }
        }

        bool SaveAsFileCommand_CanExecute(object prm)
        {
            return OrganizerVM != null;
        }
        
        void CloseFileCommand_Executed(object prm)
        {
            if (!OrganizerHasChanged())
            {
                DataProvider.CloseOrganizer();
                OrganizerVM = null;
            }
        }
        
        bool CloseFileCommand_CanExecute(object prm)
        {
            return OrganizerVM != null;
        }
        
        void ExitApplicationCommand_Executed(object prm)
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        bool ExitApplicationCommand_CanExecute(object prm)
        {
            return true;
        }

        private bool ShowAboutDialogCommand_CanExecute(object obj)
        {
            return true;
        }

        private void ShowAboutDialogCommand_Executed(object obj)
        {
            var d = new Views.Dialogs.AboutDialog();
            d.Owner = Application.Current.MainWindow;
            d.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            d.ShowDialog();
        }

        #endregion // Command functionality

        #region Private methods

        bool OrganizerHasChanged()
        {
            if (OrganizerVM != null && OrganizerVM.HasChanged)
            {
                var r = System.Windows.MessageBox.Show("Save changes?", "There are unsaved changes. Save first?", System.Windows.MessageBoxButton.YesNoCancel);
                if (r == System.Windows.MessageBoxResult.Yes)
                {
                    SaveFileCommand.Execute(null);
                    if (OrganizerVM.HasChanged)
                        return true;
                }
                else if (r == System.Windows.MessageBoxResult.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public MainWindowViewModel()
        {
            _DataProvider = new MiniDataProvider();

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                _DataProvider.FileName = args[1];
                _DataProvider.LoadOrganizer();
                OrganizerVM = new MiniOrganizerViewModel(DataProvider);
            }
            else if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.LastOpenFile) &&
                     File.Exists(Properties.Settings.Default.LastOpenFile))
            {
                DataProvider.FileName = Properties.Settings.Default.LastOpenFile;
                DataProvider.LoadOrganizer();
                OrganizerVM = new MiniOrganizerViewModel(DataProvider);
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DataProvider.FileName))
            {
                Properties.Settings.Default.LastOpenFile = DataProvider.FileName;
                Properties.Settings.Default.Save();
            }
            if(OrganizerHasChanged())
            {
                e.Cancel = true;
            }
        }

        #endregion // Constructor
    }
}