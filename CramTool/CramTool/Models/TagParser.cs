using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models
{
    public class TagParser
    {
        private static readonly char[] tagSeparators = new[] {',', ';'};

        public static List<string> ParseTags(string tagsString)
        {
            if (string.IsNullOrWhiteSpace(tagsString))
            {
                return new List<string>();
            }
            string[] tags = tagsString.Split(tagSeparators);
            var sortedTags = new SortedSet<string>();
            foreach (string tag in tags)
            {
                string trimmedTag = tag.Trim();
                if (!string.IsNullOrEmpty(trimmedTag))
                {
                    sortedTags.Add(trimmedTag);
                }
            }
            return sortedTags.ToList();
        }

        public static string FormatTags(List<string> tags)
        {
            return string.Join(", ", tags);
        }

        public static string ReformatTags(string tags)
        {
            List<string> parsedTags = ParseTags(tags);
            return FormatTags(parsedTags);
        }
    }
}