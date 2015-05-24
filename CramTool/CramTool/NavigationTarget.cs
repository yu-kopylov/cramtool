namespace CramTool
{
    public class NavigationTarget
    {
        private readonly string wordName;

        private NavigationTarget(string wordName)
        {
            this.wordName = wordName;
        }

        public static NavigationTarget ToWord(string wordName)
        {
            return new NavigationTarget(wordName);
        }

        public string WordName
        {
            get { return wordName; }
        }
    }
}