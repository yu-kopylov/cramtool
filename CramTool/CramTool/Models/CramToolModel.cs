using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CramTool.Formats;
using CramTool.Models.Quizzes;

namespace CramTool.Models
{
    public class CramToolModel : INotifyPropertyChanged
    {
        private static readonly CramToolModel instance = new CramToolModel();

        private readonly CramToolSettings settings = new CramToolSettings();
        private readonly Quiz quiz = new Quiz();

        private string mainWindowTitle;
        private string dictPath;
        private string shortDictPath;
        private WordList wordList;

        private CramToolModel()
        {
            settings.Load();
        }

        public static CramToolModel Instance
        {
            get { return instance; }
        }

        public string MainWindowTitle
        {
            get { return mainWindowTitle; }
            set
            {
                mainWindowTitle = value;
                OnPropertyChanged();
            }
        }

        private void UpdateMainWindowTitle()
        {
            MainWindowTitle = string.Format("CramTool: {0}{1}", ShortDictPath, (WordList != null && WordList.Modified) ? " (Modified)" : "");
        }

        public string DictPath
        {
            get { return dictPath; }
            private set
            {
                dictPath = value;
                OnPropertyChanged();
                ShortDictPath = dictPath == null ? "Untitled" : Path.GetFileName(DictPath);
            }
        }

        public string ShortDictPath
        {
            get { return shortDictPath; }
            set
            {
                shortDictPath = value;
                OnPropertyChanged();
                UpdateMainWindowTitle();
            }
        }

        public WordList WordList
        {
            get { return wordList; }
            private set
            {
                wordList = value;
                if (wordList != null)
                {
                    wordList.PropertyChanged += (sender, args) => UpdateMainWindowTitle();
                }
                OnPropertyChanged();
            }
        }

        public CramToolSettings Settings
        {
            get { return settings; }
        }

        public Quiz Quiz
        {
            get { return quiz; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenDictionary(string filename)
        {
            bool oldFormat = false;
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    WordListFileParser parser = new WordListFileParser();
                    WordList = parser.ParseXml(stream);
                    oldFormat = true;
                }
            }
            catch (ParserException)
            {
                //ignore an attempt to read file in old format if it fails
            }

            if (!oldFormat)
            {
                using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    WordListFileParser parser = new WordListFileParser();
                    WordList = parser.ParseZip(stream);
                }
            }

            DictPath = oldFormat ? null : filename;

            if (!oldFormat)
            {
                Settings.Load();
                Settings.AddRecentFile(filename);
                Settings.Save();
            }
        }

        public void SaveDictionary(string filename)
        {
            if (File.Exists(filename))
            {
                string backupFilename = filename + ".bak";
                if (File.Exists(backupFilename))
                {
                    File.Delete(backupFilename);
                }
                File.Copy(filename, filename + ".bak");
            }
            using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                WordListFileParser parser = new WordListFileParser();
                parser.GenerateZip(WordList, stream);
            }
            DictPath = filename;
            WordList.ResetModified();

            Settings.Load();
            Settings.AddRecentFile(filename);
            Settings.Save();
        }

        public void DeleteRecentFile(string filename)
        {
            Settings.Load();
            Settings.DeleteRecentFile(filename);
            Settings.Save();
        }
    
        public void CreateNewDictionary()
        {
            WordList = new WordList();
            DictPath = null;
        }
    }
}