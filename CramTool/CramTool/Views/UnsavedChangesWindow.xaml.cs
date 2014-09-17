using System.Windows;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for UnsavedChangesWindow.xaml
    /// </summary>
    public partial class UnsavedChangesWindow : Window
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof (string), typeof (UnsavedChangesWindow), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof (UnsavedChangesHandling), typeof (UnsavedChangesWindow),
                                                                                               new PropertyMetadata(UnsavedChangesHandling.Cancel));

        public UnsavedChangesWindow()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public UnsavedChangesHandling Result
        {
            get { return (UnsavedChangesHandling) GetValue(ResultProperty); }
            private set { SetValue(ResultProperty, value); }
        }

        private void ReturnSave(object sender, RoutedEventArgs e)
        {
            Result = UnsavedChangesHandling.Save;
            DialogResult = true;
        }

        private void ReturnIgnore(object sender, RoutedEventArgs e)
        {
            Result = UnsavedChangesHandling.Ignore;
            DialogResult = true;
        }
    }

    public enum UnsavedChangesHandling
    {
        Save,
        Ignore,
        Cancel
    }
}