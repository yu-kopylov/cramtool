using System.Windows.Controls;
using System.Windows.Input;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for MainTabSet.xaml
    /// </summary>
    public partial class MainTabSet : UserControl
    {
        public MainTabSet()
        {
            InitializeComponent();
        }

        private void NavigateTo(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationTarget target = e.Parameter as NavigationTarget;
            if (target == null)
            {
                return;
            }
            UxTabSet.SelectedItem = UxLookupTab;
            UxLookupPanel.NavigateTo(target.WordName);
        }
    }
}
