using System.Windows;

namespace MiniTD.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InfoHyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:menno@codingconnected.eu");
        }

        private void InfoHyperlink2_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.codingconnected.eu");
        }
    }
}
