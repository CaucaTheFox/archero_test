using Features.Enemies;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

namespace Spreadsheets.Importers
{
    public static class EnemyConfigImporter
    {
        private const string Url = "https://docs.google.com/spreadsheets/d/" +
                                   "1WSJIC9YE3-Ib5XzUwBmLfsWMZd4DKYYoBgMLrtgY52U" +
                                   "/export?format=csv&id=1WSJIC9YE3-Ib5XzUwBmLfsWMZd4DKYYoBgMLrtgY52U&gid=1614437953";
        
        [MenuItem("ArcheroTest/Import Spreadsheets/Enemy Config")]
        public static void Import() =>
                SpreadsheetImporter.ImportSimple<EnemySettings>(
                    Url,
                    true, 
                    ',',
                    importedData =>
                        {
                            var newConfig = new EnemyConfig(importedData);
                            var json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                            var path = Path.Combine("Assets/Resources/Configs/Enemies", "EnemyConfig.json");
                            File.WriteAllText(path, json);
                        }
                );
    }
}