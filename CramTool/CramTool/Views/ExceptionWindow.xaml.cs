using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        public static readonly DependencyProperty ExceptionProperty = DependencyProperty.Register("Exception", typeof(Exception), typeof(ExceptionWindow), new PropertyMetadata(default(Exception), OnExceptionChanged));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ExceptionWindow), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty StackTraceProperty = DependencyProperty.Register("StackTrace", typeof(string), typeof(ExceptionWindow), new PropertyMetadata(default(string)));

        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public Exception Exception
        {
            get { return (Exception) GetValue(ExceptionProperty); }
            set { SetValue(ExceptionProperty, value); }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            private set { SetValue(MessageProperty, value); }
        }

        public string StackTrace
        {
            get { return (string) GetValue(StackTraceProperty); }
            private set { SetValue(StackTraceProperty, value); }
        }

        private static void OnExceptionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ExceptionWindow win = (ExceptionWindow) obj;
            win.UpdateExceptionData();
        }

        private void UpdateExceptionData()
        {
            Exception ex = Exception;

            if (ex == null)
            {
                Message = null;
                StackTrace = null;
                return;
            }

            Message = ex.Message;
            StackTrace = FormatStackTrace(ex);
        }

        private static string FormatStackTrace(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            for(;ex != null; ex = ex.InnerException)
            {
                sb.AppendFormat("{0}: {1}", ex.GetType().FullName, ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);
            }

            return sb.ToString();
        }
    }
}
