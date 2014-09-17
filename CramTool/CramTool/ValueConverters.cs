using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CramTool
{
    public class VisibleIfConverter : IValueConverter
    {
        public static VisibleIfConverter Instance = new VisibleIfConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                throw new NotSupportedException("VisibleIfConverter does not support null parameter");
            }

            if (value == null)
            {
                return Visibility.Collapsed;
            }

            if (value is bool)
            {
                bool value1 = (bool) value;
                bool value2 = bool.Parse((string)parameter);
                return GetVisibility(value1 == value2);
            }

            if (value is Enum)
            {
                object value2 = Enum.Parse(value.GetType(), (string)parameter);
                return GetVisibility(value.Equals(value2));
            }

            throw new NotSupportedException("VisibleIfConverter does not support this types");
        }

        private Visibility GetVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class VisibleIfNull : IValueConverter
    {
        public static VisibleIfNull Instance = new VisibleIfNull();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool shouldBeNull = parameter == null || IsTrue(parameter);

            if ((value == null) == shouldBeNull)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        private bool IsTrue(object parameter)
        {
            string str = parameter as string;
            if (str != null)
            {
                if (string.Equals(str, "True", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                if (string.Equals(str, "False", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            throw new NotSupportedException("Cannot convert parameter to bool.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CommandToolTipConverter : IValueConverter
    {
        public static CommandToolTipConverter Instance = new CommandToolTipConverter();

        private static readonly Regex removeUnderScoresPattern = new Regex(@"__?");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICommandSource commandSource = value as ICommandSource;

            if (commandSource == null)
            {
                return null;
            }

            RoutedUICommand uiCommand = commandSource.Command as RoutedUICommand;

            if (uiCommand == null)
            {
                return null;
            }

            string text = uiCommand.Text;

            text = removeUnderScoresPattern.Replace(text, Evaluator);

            foreach (var inputGesture in uiCommand.InputGestures)
            {
                KeyGesture keyGesture = inputGesture as KeyGesture;
                if (keyGesture != null)
                {
                    text = text + " (" + keyGesture.GetDisplayStringForCulture(CultureInfo.InvariantCulture) + ")";
                    break;
                }
            }

            return text;
        }

        private string Evaluator(Match match)
        {
            if (match.Value.Length == 2)
            {
                return "_";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}