using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Common.WPF;
using System.Windows.Controls;
using Infragistics.Controls.Editors;
using System.Windows.Media.Imaging;

namespace Cassandra.LinqPad
{
    public class NetworkMaskConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                                System.Globalization.CultureInfo culture)
        {
            if (value == null || ReferenceEquals(value, DependencyProperty.UnsetValue))
            {
                return value;
            }

            if (value is NetworkAddressType)
            {
                switch ((NetworkAddressType) value)   
                {
                    case NetworkAddressType.IPV4:
                        return "{number:0-255}\\.{number:0-255}\\.{number:0-255}\\.{number:0-255}";
                    case NetworkAddressType.HostName:
                        return "{char:253:a-zA-Z0-9_\\.\\-\\:\\%}";                                        
                    case NetworkAddressType.IPV6:
                        return "{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}\\:{char:4:0-9a-fA-F}";
                    default:
                        break;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DetermineImageStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                                System.Globalization.CultureInfo culture)
        {
            if (ReferenceEquals(value, DependencyProperty.UnsetValue))
            {
                return value;
            }

            var status = (NodeState.Status?) value;          

            if (status.HasValue)
            {
                switch (status.Value)
                {                   
                    case NodeState.Status.Error:
                        return new BitmapImage(new Uri(Properties.Settings.Default.ErrorURIIcon16));                       
                    case NodeState.Status.Running:
                        return new BitmapImage(new Uri(Properties.Settings.Default.QeuryURI16));
                    case NodeState.Status.Up:
                        return new BitmapImage(new Uri(Properties.Settings.Default.UPURI16));
                    case NodeState.Status.Down:
                        return new BitmapImage(new Uri(Properties.Settings.Default.DownURI16));
                    case NodeState.Status.Sucessful:
                        return new BitmapImage(new Uri(Properties.Settings.Default.OKURI16));                        
                    default:
                        return new BitmapImage(new Uri(Properties.Settings.Default.AlertURI16));
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DetermineToolTipStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
                                System.Globalization.CultureInfo culture)
        {
            if (ReferenceEquals(values[0], DependencyProperty.UnsetValue))
            {
                return values[0];
            }

            var status = (NodeState.Status?)values[0];
            var exception = values[1] as Exception;            

            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case NodeState.Status.Error:                        
                        return exception == null ? "An unhandled exception occurred" : string.Format("{0}: {1}", exception.GetType().Name, exception.Message);
                    case NodeState.Status.Running:
                        return "Please wait, determining status...";
                    case NodeState.Status.Up:
                        return "Server is Up";
                    case NodeState.Status.Down:
                        return "Server is Down";
                    case NodeState.Status.Sucessful:
                        return "Status Query was Successful";
                    default:
                        return exception == null ? "Cannot determine status" : "Cannot determine status due to Exception";
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypse, object parameter,
           System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
