using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestCramToolSettings
    {
        [Explicit]
        [Test]
        public void Test()
        {
            CramToolSettings originalSettings = new CramToolSettings();
            originalSettings.Load();

            try
            {
                const string filename1 = @"C:\dict.dictx";
                const string filename2 = @"C:\dict2.dictx";

                CramToolSettings settings1 = new CramToolSettings();
                for (int i = CramToolSettings.MaxRecentFiles * 2; i >= 0; i--)
                {
                    settings1.AddRecentFile(filename1 + "-" + i);
                }
                settings1.AddRecentFile(filename1);
                settings1.AddRecentFile(filename2);
                settings1.AddRecentFile(filename1);
                settings1.AddRecentFile(filename2);
                settings1.Save();

                CramToolSettings settings2 = new CramToolSettings();
                settings2.Load();
                Assert.That(settings2.RecentFiles.Count, Is.EqualTo(CramToolSettings.MaxRecentFiles));
                Assert.That(settings2.RecentFiles[0], Is.EqualTo(filename2));
                Assert.That(settings2.RecentFiles[1], Is.EqualTo(filename1));
                for (int i = 2; i < CramToolSettings.MaxRecentFiles; i++)
                {
                    Assert.That(settings2.RecentFiles[i], Is.EqualTo(filename1 + "-" + (i - 2)));
                }
            }
            finally
            {
                originalSettings.Save();
            }
        }
    }
}