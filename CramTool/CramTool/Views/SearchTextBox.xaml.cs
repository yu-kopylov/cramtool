using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof (string),
            typeof (SearchTextBox),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SearchTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void OnClearButtonPressed(object sender, RoutedEventArgs e)
        {
            UxTextBox.BeginChange();
            try
            {
                UxTextBox.SelectAll();
                UxTextBox.SelectedText = "";
            }
            finally
            {
                UxTextBox.EndChange();
            }

            UxTextBox.Focus();
        }
    }
}