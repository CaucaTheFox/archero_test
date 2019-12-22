using Features.Heroes;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

namespace Spreadsheets.Importers
{

    public static class HeroConfigImporter
    {
        private const string Uri = "https://docs.google.com/spreadsheets/d/e/" +
            "2PACX-1vRWPrm3oa7WjeuxoZxISDgFkG87Y2TY0Oa5PsLvCddi6Jq3rEXFoIkXe7U1jwK9BTc75k2qHzJNJt4Y/" +
            "pub?gid=2145931350&single=true&output=csv";


        [MenuItem("Import Spreadsheets/Hero Config")]
        public static void Import() =>
                SpreadsheetImporter.ImportSimple<HeroSettings>(
                    Uri,
                    true, 
                    ',',
                    importedData =>
                        {
                            var newConfig = new HeroConfig(importedData);
                            var json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                            var path = Path.Combine("Assets/Resources/Configs/Hero", "HeroConfig.json");
                            File.WriteAllText(path, json);
                        }
                );
    }
}