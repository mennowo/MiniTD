﻿using MiniTD.DataTypes;
using System;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniTD.ViewModels
{
    public class MiniTopicViewModel : ViewModelBase
    {
        #region Fields

        private MiniTopic _Topic;
        private readonly MiniOrganizerViewModel _OrganizerVM;

        #endregion // Fields

        #region Properties

        public MiniOrganizerViewModel OrganizerVM
        {
            get { return _OrganizerVM; }
        }

        //public static class Colors
        //{
        //    private static readonly Dictionary<string, Color> dictionary =
        //        typeof(Color).GetProperties(BindingFlags.Public |
        //                                    BindingFlags.Static)
        //                     .Where(prop => prop.PropertyType == typeof(Color))
        //                     .ToDictionary(prop => prop.Name,
        //                                   prop => (Color)prop.GetValue(null, null));

        //    public static Color FromName(string name)
        //    {
        //        // Adjust behaviour for lookup failure etc
        //        return dictionary[name];
        //    }
        //}

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
                OnMonitoredPropertyChanged("Title", OrganizerVM);
            }
        }

        public Color TopicColor
        {
            get { return _Topic.Color; }
            set
            {
                _Topic.Color = value;
                OnMonitoredPropertyChanged("TopicColor", OrganizerVM);
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

        public MiniTopicViewModel(MiniTopic _topic, MiniOrganizerViewModel organizervm)
        {
            _OrganizerVM = organizervm;
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
                var val = value as string;
                Color c;
                if (val.StartsWith("#"))
                {
                    val = val.Replace("#", "");
                    var a = System.Convert.ToByte("ff", 16);
                    byte pos = 0;
                    if (val.Length == 8)
                    {
                        a = System.Convert.ToByte(val.Substring(pos, 2), 16);
                        pos = 2;
                    }
                    var r = System.Convert.ToByte(val.Substring(pos, 2), 16);
                    pos += 2;
                    var g = System.Convert.ToByte(val.Substring(pos, 2), 16);
                    pos += 2;
                    var b = System.Convert.ToByte(val.Substring(pos, 2), 16);
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
            var colorType = (typeof(System.Windows.Media.Colors));
            if (colorType.GetProperty(colorString) != null)
            {
                var color = colorType.InvokeMember(colorString, BindingFlags.GetProperty, null, null, null);
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
            var val = (Color)value;
            return val.ToString();
            //if (typeof(Colors).GetProperty(val.Color.ToString()) != null)
            //    return typeof(Colors).GetProperty(val.Color.ToString()).GetValue(val, null);
            //else
            //    return "#" + val.Color.A.ToString() + val.Color.R.ToString() + val.Color.G.ToString() + val.Color.B.ToString();
        }
    }
}
