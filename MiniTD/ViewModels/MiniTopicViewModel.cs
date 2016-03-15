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

using MiniTD.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniTD.ViewModels
{
    public class MiniTopicViewModel : ViewModelBase
    {
        #region Fields

        private MiniTopic _Topic;

        #endregion // Fields

        #region Properties

        public static class Colors
        {
            private static readonly Dictionary<string, Color> dictionary =
                typeof(Color).GetProperties(BindingFlags.Public |
                                            BindingFlags.Static)
                             .Where(prop => prop.PropertyType == typeof(Color))
                             .ToDictionary(prop => prop.Name,
                                           prop => (Color)prop.GetValue(null, null));

            public static Color FromName(string name)
            {
                // Adjust behaviour for lookup failure etc
                return dictionary[name];
            }
        }

        public MiniTopic Topic
        {
            get { return _Topic; }
            set
            {
                _Topic = value;
                OnPropertyChanged("Topic");
            }
        }

        public long ID
        {
            get { return _Topic.ID; }
        }

        public string Title
        {
            get { return _Topic.Title; }
            set
            {
                _Topic.Title = value;
                OnPropertyChanged("Title");
            }
        }

        public Color TopicColor
        {
            get { return _Topic.Color; }
            set
            {
                _Topic.Color = value;
                OnPropertyChanged("TopicColor");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public MiniTopicViewModel(MiniTopic _topic)
        {
            _Topic = _topic;
        }

        public MiniTopicViewModel()
        {

        }

        #endregion // Constructor
    }

    // from: http://www.java2s.com/Code/CSharp/Windows-Presentation-Foundation/ColorConverter.htm
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(String))
            {
                string val = value as string;
                Color c;
                if (val.StartsWith("#"))
                {
                    val = val.Replace("#", "");
                    byte a = System.Convert.ToByte("ff", 16);
                    byte pos = 0;
                    if (val.Length == 8)
                    {
                        a = System.Convert.ToByte(val.Substring(pos, 2), 16);
                        pos = 2;
                    }
                    byte r = System.Convert.ToByte(val.Substring(pos, 2), 16);
                    pos += 2;
                    byte g = System.Convert.ToByte(val.Substring(pos, 2), 16);
                    pos += 2;
                    byte b = System.Convert.ToByte(val.Substring(pos, 2), 16);
                    c = Color.FromArgb(a, r, g, b);
                    return new SolidColorBrush(c);
                }
                else
                {
                    try
                    {
                        c = GetColorFromString(value as string);
                        return new SolidColorBrush(c);
                    }
                    catch (InvalidCastException ex)
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public static Color GetColorFromString(string colorString)
        {
            Type colorType = (typeof(System.Windows.Media.Colors));
            if (colorType.GetProperty(colorString) != null)
            {
                object color = colorType.InvokeMember(colorString, BindingFlags.GetProperty, null, null, null);
                try
                {
                    return (Color)color;
                }
                catch
                {
                    throw new InvalidCastException("Color not defined");
                }
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color val = (Color)value;
            return val.ToString();
            //if (typeof(Colors).GetProperty(val.Color.ToString()) != null)
            //    return typeof(Colors).GetProperty(val.Color.ToString()).GetValue(val, null);
            //else
            //    return "#" + val.Color.A.ToString() + val.Color.R.ToString() + val.Color.G.ToString() + val.Color.B.ToString();
        }
    }
}
