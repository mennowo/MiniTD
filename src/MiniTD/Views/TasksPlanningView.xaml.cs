using MiniTD.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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

    public class TimeSpanToColorConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                return ts.TotalHours switch
                {
                    var i when i <= 8.0 => new SolidColorBrush(Color.FromRgb(120, (byte)(255 - i / 8.0 * 68), 150)),
                    _ => Brushes.Pink
                };
            }

            return Brushes.DarkSeaGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class TimeSpanScaleTransformer : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (double)values[1] / 8.0 * ((TimeSpan)values[0]).TotalHours;
            return v > 0 ? v : 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // below from: http://stackoverflow.com/questions/24618966/wpf-datagrid-grouping-with-sums-and-other-fields
    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<object> items)
            {
                var total = new TimeSpan();
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
                var total = new TimeSpan();
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
