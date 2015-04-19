using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CramTool.Formats.Html;
using CramTool.Models;
using Microsoft.Win32;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanCreateNewDictionary(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreateNewDictionary(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmChangesSaved())
            {
                return;
            }
            CramToolModel.Instance.CreateNewDictionary();
        }

        private void CanOpenDictionary(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenDictionary(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmChangesSaved())
            {
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dictz";
            dlg.Filter = "All Supported Files|*.dictz;*.dictx|Cram Tool Compressed Dictionaries (.dictz)|*.dictz|Cram Tool Dictionaries (.dictx)|*.dictx";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dlg.ShowDialog(this) == true)
            {
                CramToolModel.Instance.OpenDictionary(dlg.FileName);
            }
        }

        private void CanSaveDictionary(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CramToolModel.Instance.WordList != null;
        }

        private void SaveDictionary(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDictionary();
        }

        private bool SaveDictionary()
        {
            if (string.IsNullOrEmpty(CramToolModel.Instance.DictPath))
            {
                return SaveDictionaryAs();
            }
            CramToolModel.Instance.SaveDictionary(CramToolModel.Instance.DictPath);
            return true;
        }

        private void CanSaveDictionaryAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CramToolModel.Instance.WordList != null;
        }

        private void SaveDictionaryAs(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDictionaryAs();
        }

        private bool SaveDictionaryAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".dictz";
            dlg.Filter = "Cram Tool Compressed Dictionaries|*.dictz";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dlg.ShowDialog(this) == true)
            {
                CramToolModel.Instance.SaveDictionary(dlg.FileName);
                return true;
            }
            return false;
        }

        private void CanExportDictionary(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CramToolModel.Instance.WordList != null;
        }

        private void ExportDictionary(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".html";
            dlg.Filter = "HTML|*.htm;*.html;*.xhtml";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dlg.ShowDialog(this) != true)
            {
                return;
            }

            HtmlGenerator htmlGenerator = new HtmlGenerator();

            File.WriteAllBytes(dlg.FileName, htmlGenerator.Generate(CramToolModel.Instance.WordList));
        }

        private void CanResetHistory(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CramToolModel.Instance.WordList != null;
        }

        private void ResetHistory(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmationWindow.Confirm(this, "Reset history for all words?"))
            {
                return;
            }
            CramToolModel.Instance.WordList.ResetHistory();
        }

        private void HelpAbout(object sender, ExecutedRoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ConfirmChangesSaved();
        }

        private bool ConfirmChangesSaved()
        {
            WordList wordList = CramToolModel.Instance.WordList;
            if (wordList == null || !wordList.Modified)
            {
                return true;
            }
            UnsavedChangesWindow win = new UnsavedChangesWindow();
            win.Owner = this;
            win.Message = string.Format("Do you want to save changes to {0}?", CramToolModel.Instance.ShortDictPath);
            win.ShowDialog();

            UnsavedChangesHandling result = win.Result;
            if (result == UnsavedChangesHandling.Cancel)
            {
                return false;
            }
            if (result == UnsavedChangesHandling.Ignore)
            {
                return true;
            }
            try
            {
                return SaveDictionary();
            }
            catch (Exception e)
            {
                App.HandleException(this, e);
                return false;
            }
        }
    }
}