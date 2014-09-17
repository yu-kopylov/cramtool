using System.Collections.Generic;

namespace CramTool.Models
{
    public class WordTranslation
    {
        public string Name { get; set; }

        public List<WordInfo> Words { get; set; }

        private WordTranslation()
        {
        }

        public WordTranslation(string name, WordInfo word)
        {
            Name = name;
            Words = new List<WordInfo> { word };
        }

        public WordTranslation Copy()
        {
            WordTranslation res = new WordTranslation();
            res.Name = Name;
            res.Words = new List<WordInfo>(Words);
            return res;
        }
    }
}