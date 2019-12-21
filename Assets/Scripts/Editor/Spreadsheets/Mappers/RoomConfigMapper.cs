using CsvHelper.Configuration;
using Features.Rooms;
using System.Collections.Generic;

namespace Spreadsheets.Mappers
{
    public sealed class RoomConfigMapper : ClassMap<RoomConfig>
    {
        public RoomConfigMapper()
        {
            Map(m => m.Id).ConvertUsing(row => row.GetField<int>(0));
            Map(m => m.Rows).ConvertUsing(row => {
                var list = new List<RoomRowConfig>();
                for (var i = 1; i < 26; i++)
                {
                    var field = row.GetField<string>(i);
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    var roomConfig = new RoomRowConfig(field);
                    list.Add(roomConfig);
                }

                return list;
            });
        }
    }
}