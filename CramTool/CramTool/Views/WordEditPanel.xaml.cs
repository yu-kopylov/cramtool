using System;
using System.Windows;
using System.Windows.Controls;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for WordEditPanel.xaml
    /// </summary>
    public partial class WordEditPanel : UserControl
    {
        public static readonly DependencyProperty Word2Property = DependencyProperty.Register("Word", typeof(Word), typeof(WordEditPanel), new PropertyMetadata(default(Word)));

        public WordEditPanel()
        {
            InitializeComponent();
        }

        public Word Word
        {
            get { return (Word)GetValue(Word2Property); }
            set { SetValue(Word2Property, value); }
        }
    }
}