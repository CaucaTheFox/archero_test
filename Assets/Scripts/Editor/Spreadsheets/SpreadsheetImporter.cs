using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Networking;
using CsvParser = Utility.CsvParser;

namespace Spreadsheets
{
    public static class SpreadsheetImporter
    {
        private static void ImportGeneric<A>(
            string url, Func<string, List<A>> parser, Action<List<A>> onComplete
        )
        {
            var request = UnityWebRequest.Get(url);
            request.timeout = 20;
            var op = request.SendWebRequest();
            op.completed += _ => {
                var result = parser(request.downloadHandler.text);
                onComplete?.Invoke(result);
                EditorUtility.DisplayDialog(
                    "Great Success",
                    $"{typeof(A).Name} updated successfully",
                    "OK"
                );
            };
        }

        public static void ImportUsingMapper<A, B>(string url, Action<List<A>> onComplete) where B : ClassMap =>
            ImportGeneric(url, ParseCsv<A, B>, onComplete);

        public static void ImportSimple<A>(string uri, bool includesHeader, char separator, Action<List<A>> onComplete) where A : new() =>
            ImportGeneric(uri, s => CsvParser.Parse<A>(s, includesHeader, separator), onComplete);

        private static List<A> ParseCsv<A, B>(string rawText) where B : ClassMap
        {
            using var stringReader = new StringReader(rawText);
            using var reader = new CsvReader(stringReader);
            reader.Configuration.Delimiter = ",";
            reader.Configuration.MissingFieldFound = null;
            reader.Configuration.CultureInfo = new CultureInfo("en-US");
            reader.Configuration.RegisterClassMap<B>();

            return reader.GetRecords<A>().ToList();
        }
    }
}