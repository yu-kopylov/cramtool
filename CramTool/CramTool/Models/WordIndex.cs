﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models
{
    public class WordIndex
    {
        private readonly SortedList<string, SortedSet<string>> attributesIndex = new SortedList<string, SortedSet<string>>();

        public void Add(string wordName, string attribute)
        {
            SortedSet<string> wordNames;
            if (!attributesIndex.TryGetValue(attribute, out wordNames))
            {
                wordNames = new SortedSet<string>();
                attributesIndex.Add(attribute, wordNames);
            }
            if (!wordNames.Add(wordName))
            {
                throw new ArgumentException(string.Format("Word '{0}' already has attribute '{1}'.", wordName, attribute));
            }
        }

        public void Remove(string wordName, string attribute)
        {
            SortedSet<string> wordNames;
            if (!attributesIndex.TryGetValue(attribute, out wordNames))
            {
                throw new KeyNotFoundException(string.Format("Attribute '{0}' does not exits.", attribute));
            }
            if (!wordNames.Remove(wordName))
            {
                throw new KeyNotFoundException(string.Format("Word '{0}' does not have attribute '{1}'.", wordName, attribute));
            }
            if (wordNames.Count == 0)
            {
                attributesIndex.Remove(attribute);
            }
        }

        public List<string> GetAttributes()
        {
            return attributesIndex.Keys.ToList();
        }

        public List<string> GetWordNames(string attribute)
        {
            return attributesIndex[attribute].ToList();
        }

        public void Update(string oldWordName, List<string> oldAttributes, string newWordName, List<string> newAttributes)
        {
            List<Tuple<string, string>> oldPairs = GetWordAttributePairs(oldWordName, oldAttributes);
            List<Tuple<string, string>> newPairs = GetWordAttributePairs(newWordName, newAttributes);

            List<Tuple<string, string>> removedPairs = oldPairs.Except(newPairs).ToList();
            List<Tuple<string, string>> addedPairs = newPairs.Except(oldPairs).ToList();

            foreach (Tuple<string, string> pair in removedPairs)
            {
                Remove(pair.Item1, pair.Item2);
            }
            foreach (Tuple<string, string> pair in addedPairs)
            {
                Add(pair.Item1, pair.Item2);
            }
        }

        private static List<Tuple<string, string>> GetWordAttributePairs(string wordName, List<string> attributes)
        {
            return attributes.Select(attribute => Tuple.Create(wordName, attribute)).ToList();
        }
    }
}