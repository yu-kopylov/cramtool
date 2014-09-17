using System.Windows;
using System.Windows.Controls;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for QuizPreparePanel.xaml
    /// </summary>
    public partial class QuizPreparePanel : UserControl
    {
        public static readonly DependencyProperty QuizProperty =
            DependencyProperty.Register("Quiz", typeof (Quiz), typeof (QuizPreparePanel), new PropertyMetadata(default(Quiz)));

        public QuizPreparePanel()
        {
            InitializeComponent();
        }

        public Quiz Quiz
        {
            get { return (Quiz)GetValue(QuizProperty); }
            set { SetValue(QuizProperty, value); }
        }
    }
}
