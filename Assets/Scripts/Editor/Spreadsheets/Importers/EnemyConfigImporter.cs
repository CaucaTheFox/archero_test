using Features.Enemies;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

namespace Spreadsheets.Importers
{
    public static class EnemyConfigImporter
    {
        private const string Uri = "https://docs.google.com/spreadsheets/d/e/" +
            "2PACX-1vRWPrm3oa7WjeuxoZxISDgFkG87Y2TY0Oa5PsLvCddi6Jq3rEXFoIkXe7U1jwK9BTc75k2qHzJNJt4Y/" +
            "pub?gid=1407994961&single=true&output=csv";


        [MenuItem("Import Spreadsheets/Enemy Config")]
        public static void Import() =>
                SpreadsheetImporter.ImportSimple<EnemySettings>(
                    Uri,
                    true, 
                    ',',
                    importedData =>
                        {
                            var newConfig = new EnemyConfig(importedData);
                            var json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                            var path = Path.Combine("Assets/Resources/Configs/Enemy", "EnemyConfig.json");
                            File.WriteAllText(path, json);
                        }
                );
    }
}