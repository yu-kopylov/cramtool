using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CramTool.Formats.Settings;

namespace CramTool.Models
{
    public class CramToolSettings
    {
        public const int MaxRecentFiles = 8;

        private readonly ObservableCollection<string> recentFiles = new ObservableCollection<string>();

        public ObservableCollection<string> RecentFiles
        {
            get { return recentFiles; }
        }

        public static string CramToolSettingsFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CramTool"); }
        }

        public static string SettingsFilename
        {
            get { return Path.Combine(CramToolSettingsFolder, "settings.xml"); }
        }

        public void Load()
        {
            Settings settings;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (Stream stream = File.OpenRead(SettingsFilename))
                {
                    settings = (Settings)ser.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return;
            }
            RecentFiles.Clear();
            foreach (string recentFile in settings.RecentFiles)
            {
                RecentFiles.Add(recentFile);
            }
        }

        public void Save()
        {
            Settings settings = new Settings();
            settings.RecentFiles = RecentFiles.ToArray();
            try
            {
                Directory.CreateDirectory(CramToolSettingsFolder);

                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                using (Stream stream = File.Create(SettingsFilename))
                {
                    ser.Serialize(stream, settings);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public void AddRecentFile(string filename)
        {
            int oldIndex = RecentFiles.IndexOf(filename);
            if (oldIndex >= 0)
            {
                RecentFiles.RemoveAt(oldIndex);
            }
            RecentFiles.Insert(0, filename);
            while (RecentFiles.Count > MaxRecentFiles)
            {
                RecentFiles.RemoveAt(RecentFiles.Count - 1);
            }
        }

        public void DeleteRecentFile(string filename)
        {
            int oldIndex = RecentFiles.IndexOf(filename);
            if (oldIndex >= 0)
            {
                RecentFiles.RemoveAt(oldIndex);
            }
        }
    }
}