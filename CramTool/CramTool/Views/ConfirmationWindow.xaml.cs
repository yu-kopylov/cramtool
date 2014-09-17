using System.Windows;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof (string), typeof (ConfirmationWindow), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof (bool), typeof (ConfirmationWindow), new PropertyMetadata(false));

        public ConfirmationWindow()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public bool Result
        {
            get { return (bool) GetValue(ResultProperty); }
            private set { SetValue(ResultProperty, value); }
        }

        public static bool Confirm(Window owner, string message)
        {
            ConfirmationWindow win = new ConfirmationWindow();

            win.Owner = owner;
            win.Message = message;

            win.ShowDialog();

            return win.Result;
        }

        private void ReturnYes(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
        }

        private void ReturnNo(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = true;
        }
    }
}