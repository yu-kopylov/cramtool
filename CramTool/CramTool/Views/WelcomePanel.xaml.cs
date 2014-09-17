using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for WelcomePanel.xaml
    /// </summary>
    public partial class WelcomePanel : UserControl
    {
        public WelcomePanel()
        {
            InitializeComponent();
        }

        private void OpenRecentFile(object sender, ExecutedRoutedEventArgs e)
        {
            string filename = e.Parameter as string;
            if (filename == null)
            {
                return;
            }
            CramToolModel.Instance.OpenDictionary(filename);
        }

        private void DeleteRecentFile(object sender, ExecutedRoutedEventArgs e)
        {
            string filename = e.Parameter as string;
            if (filename == null)
            {
                return;
            }

            if (!ConfirmationWindow.Confirm(Window.GetWindow(this), string.Format("Remove file '{0}' from the list of recent files?", filename)))
            {
                return;
            }

            CramToolModel.Instance.DeleteRecentFile(filename);
        }
    }
}
