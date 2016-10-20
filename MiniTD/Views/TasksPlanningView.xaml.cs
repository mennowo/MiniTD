using MiniTD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniTD.Views
{
    /// <summary>
    /// Interaction logic for CurrentTasksView.xaml
    /// </summary>
    public partial class TasksPlanningView : UserControl
    {

        public TasksPlanningView()
        {
            InitializeComponent();
        }
    }

    // below from: http://stackoverflow.com/questions/24618966/wpf-datagrid-grouping-with-sums-and-other-fields
    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                TimeSpan total = new TimeSpan();
                foreach (MiniTaskViewModel tvm in items)
                {
                    total += tvm.Duration;
                }
                return total.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class GroupsToTotalConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                TimeSpan total = new TimeSpan();
                foreach (MiniTaskViewModel tvm in items)
                {
                    total += tvm.Duration;
                }
                if(total.TotalHours > 8)
                    return Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
