using System.Windows;
using System.Windows.Controls;

namespace MiniTD.Views
{
    /// <summary>
    /// Interaction logic for CurrentTasksView.xaml
    /// </summary>
    public partial class CurrentTasksView : UserControl
    {

        public bool ShowNotes
        {
            get { return (bool)GetValue(ShowNotesProperty); }
            set { SetValue(ShowNotesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNotes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNotesProperty =
            DependencyProperty.Register("ShowNotes", typeof(bool), typeof(CurrentTasksView), new PropertyMetadata(true));

        public CurrentTasksView()
        {
            InitializeComponent();
        }
    }
}
