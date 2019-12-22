using Features.Rooms;
using Newtonsoft.Json;
using Spreadsheets.Mappers;
using System.IO;
using UnityEditor;

namespace Spreadsheets.Importers
{

    public static class RoomConfigImporter
    {
        private const string Uri = "https://docs.google.com/spreadsheets" +
            "/d/e/2PACX-1vRWPrm3oa7WjeuxoZxISDgFkG87Y2TY0Oa5PsLvCddi6Jq3rEXFoIkXe7U1jwK9BTc75k2qHzJNJt4Y/" +
            "pub?gid=0&single=true&output=csv";


        [MenuItem("Import Spreadsheets/Rooms Config")]
        public static void Import() =>
                SpreadsheetImporter.ImportUsingMapper<RoomConfig, RoomConfigMapper>(
                    Uri,
                    importedData =>
                        {
                            var newConfig = new RoomsConfig(importedData);
                            var json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                            var path = Path.Combine("Assets/Resources/Configs/Rooms", "RoomsConfig.json");
                            File.WriteAllText(path, json);
                        }
                );
    }
}