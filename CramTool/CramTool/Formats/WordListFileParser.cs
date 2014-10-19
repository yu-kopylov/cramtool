using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CramTool.Formats
{
    public class WordListFileParser
    {
        private const string ZipWordListEntryName = "words.xml";

        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(WordList.WordList));

        public Models.WordList ParseXml(Stream inputStream)
        {
            WordList.WordList wordsXml;
            try
            {
                wordsXml = (WordList.WordList) serializer.Deserialize(inputStream);
            }
            catch (InvalidOperationException e)
            {
                throw new ParserException("Unknown file format.", e);
            }
            return WordListXmlConverter.ConvertToObject(wordsXml);
        }

        public Models.WordList ParseZip(Stream inputStream)
        {
            try
            {
                using (ZipArchive zip = new ZipArchive(inputStream, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry wordsEntry = zip.GetEntry(ZipWordListEntryName);
                    if (wordsEntry == null)
                    {
                        throw new ParserException(string.Format("File '{0}' is missing in the archive.", ZipWordListEntryName));
                    }
                    return ParseXml(wordsEntry.Open());
                }
            }
            catch (InvalidDataException e)
            {
                throw new ParserException("Corrupted archive.", e);
            }
        }

        public void GenerateXml(Models.WordList wordList, Stream outputStream)
        {
            WordList.WordList wordsXml = WordListXmlConverter.ConvertToXml(wordList);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(outputStream, xmlWriterSettings))
            {
                serializer.Serialize(writer, wordsXml);
                writer.Flush();
            }
        }

        public void GenerateZip(Models.WordList wordList, Stream outputStream)
        {
            using (ZipArchive zip = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
            {
                ZipArchiveEntry wordsEntry = zip.CreateEntry(ZipWordListEntryName);
                GenerateXml(wordList, wordsEntry.Open());
            }
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}