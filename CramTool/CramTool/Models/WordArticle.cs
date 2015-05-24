using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CramTool.Models
{
    public class WordArticle
    {
        public List<WordFormGroup> FormGroups { get; private set; }

        public WordArticle()
        {
            FormGroups = new List<WordFormGroup>();
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();
            bool firstFormGroup = true;
            foreach (WordFormGroup formGroup in FormGroups)
            {
                if (!firstFormGroup)
                {
                    sb.Append("\n");
                }
                firstFormGroup = false;
                foreach (string form in formGroup.Forms)
                {
                    sb.AppendFormat("#{0}\n", form);
                }
                bool firstTranslationGroup = true;
                foreach (WordTranslationGroup translationGroup in formGroup.TranslationGroups)
                {
                    if (!firstTranslationGroup || (formGroup.Forms.Count > 1 && FormGroups.Count == 1))
                    {
                        sb.Append("\n");
                    }
                    firstTranslationGroup = false;
                    foreach (string translation in translationGroup.Translations)
                    {
                        sb.AppendFormat("{0}\n", translation);
                    }
                    bool firstExample = true;
                    foreach (string example in translationGroup.Examples)
                    {
                        if (firstExample && translationGroup.Translations.Count > 1 && formGroup.TranslationGroups.Count == 1 && FormGroups.Count == 1)
                        {
                            sb.Append("\n");
                        }
                        firstExample = false;
                        sb.AppendFormat("//{0}\n", example);
                    }
                }
            }
            return sb.ToString();
        }

        public List<string> GetAllTranslations()
        {
            return FormGroups.SelectMany(fg => fg.TranslationGroups).SelectMany(tg => tg.Translations).Distinct().ToList();
        }
    }

    public class WordFormGroup
    {
        public List<string> Forms { get; private set; }
        public List<WordTranslationGroup> TranslationGroups { get; private set; }

        public WordFormGroup()
        {
            Forms = new List<string>();
            TranslationGroups = new List<WordTranslationGroup>();
        }
    }

    public class WordTranslationGroup
    {
        public List<string> Translations { get; set; }
        public List<string> Examples { get; set; }

        public WordTranslationGroup()
        {
            Translations = new List<string>();
            Examples = new List<string>();
        }
    }
}