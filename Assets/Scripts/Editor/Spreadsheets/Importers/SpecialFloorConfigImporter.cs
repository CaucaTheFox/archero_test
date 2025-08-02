using Features.Rooms;
using Newtonsoft.Json;
using Spreadsheets.Mappers;
using System.IO;
using UnityEditor;

namespace Spreadsheets.Importers
{

    public static class SpecialFloorConfigImporter
    {
        private const string Uri = "https://docs.google.com/spreadsheets/" +
            "d/e/2PACX-1vRWPrm3oa7WjeuxoZxISDgFkG87Y2TY0Oa5PsLvCddi6Jq3rEXFoIkXe7U1jwK9BTc75k2qHzJNJt4Y/" +
            "pub?gid=465461440&single=true&output=csv";


        [MenuItem("ArcheroTest/Import Spreadsheets/SpecialFloor Config")]
        public static void Import() =>
                SpreadsheetImporter.ImportUsingMapper<SpecialFloor, SpecialFloorConfigMapper>(
                    Uri,
                    importedData =>
                        {
                            var newConfig = new SpecialFloorConfig(importedData);
                            var json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                            var path = Path.Combine("Assets/Resources/Configs/Rooms", "SpecialFloorConfig.json");
                            File.WriteAllText(path, json);
                        }
                );
    }
}