using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CramTool.Views;

namespace CramTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var binding = new CommandBinding(Commands.CloseWindow, CloseWindow);
            CommandManager.RegisterClassCommandBinding(typeof(Window), binding);
        }

        private void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Window owner = MainWindow;
            if (owner != null && owner.IsVisible)
            {
                HandleException(owner, args.Exception);
                args.Handled = true;
            }
        }

        public static void HandleException(Window owner, Exception ex)
        {
            ExceptionWindow win = new ExceptionWindow();
            win.Exception = ex;
            win.Owner = owner;
            win.ShowDialog();
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window win = sender as Window;
            if (win != null)
            {
                win.Close();
                e.Handled = true;
            }
        }
    }
}