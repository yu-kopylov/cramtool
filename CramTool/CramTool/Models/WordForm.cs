namespace CramTool.Models
{
    public class WordForm
    {
        public WordInfo WordInfo { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }

        public WordForm(WordInfo wordInfo, string name)
        {
            WordInfo = wordInfo;
            Name = name;
            Title = (wordInfo.Word.Name == name) ? name : string.Format("{0} (form of {1})", name, wordInfo.Word.Name);
        }
    }
}