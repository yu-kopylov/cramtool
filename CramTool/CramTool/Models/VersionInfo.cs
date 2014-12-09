using System.Reflection;

namespace CramTool.Models
{
    public static class VersionInfo
    {
        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
    }
}