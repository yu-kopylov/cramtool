using System.Windows.Input;

namespace CramTool
{
    public static class Commands
    {
        #region Menu Commands

        public static readonly RoutedUICommand HelpAbout = new RoutedUICommand {Text = "_About CramTool"};
        public static readonly RoutedUICommand CreateNewDictionary = new RoutedUICommand {Text = "_New Dictionary"};
        public static readonly RoutedUICommand OpenDictionary = new RoutedUICommand {Text = "_Open Dictionary"};
        public static readonly RoutedUICommand SaveDictionary = new RoutedUICommand {Text = "_Save Dictionary"};
        public static readonly RoutedUICommand SaveDictionaryAs = new RoutedUICommand {Text = "Save Dictionary _As..."};
        public static readonly RoutedUICommand ExportDictionary = new RoutedUICommand { Text = "Export Dictionary..." };
        public static readonly RoutedUICommand ResetHistory = new RoutedUICommand { Text = "_Reset history" };

        #endregion

        public static readonly RoutedCommand OpenRecentFile = new RoutedCommand();
        public static readonly RoutedCommand DeleteRecentFile = new RoutedCommand();
        
        public static readonly RoutedCommand CloseWindow = new RoutedCommand();
        public static readonly RoutedCommand Search = new RoutedCommand();
        public static readonly RoutedCommand AddWord = new RoutedCommand();
        public static readonly RoutedCommand EditWord = new RoutedCommand();
        public static readonly RoutedCommand SaveWord = new RoutedCommand();
        public static readonly RoutedCommand CancelEditWord = new RoutedCommand();
        public static readonly RoutedCommand MarkWordRemembered = new RoutedCommand();
        public static readonly RoutedCommand MarkWordForgotten = new RoutedCommand();
        public static readonly RoutedCommand MarkWordAdded = new RoutedCommand();
        public static readonly RoutedCommand ResetWordHistory = new RoutedCommand();
        public static readonly RoutedCommand ShowAnswer = new RoutedCommand();
        public static readonly RoutedCommand MoveToNextWord = new RoutedCommand();
        public static readonly RoutedCommand StartQuiz = new RoutedCommand();
        public static readonly RoutedCommand EndQuiz = new RoutedCommand();

        static Commands()
        {
            OpenDictionary.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            SaveDictionary.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
        }
    }
}