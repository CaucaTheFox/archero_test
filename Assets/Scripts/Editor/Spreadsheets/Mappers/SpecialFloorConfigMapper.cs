using CsvHelper.Configuration;
using Features.Rooms;
using System.Collections.Generic;

namespace Spreadsheets.Mappers
{
    public sealed class SpecialFloorConfigMapper : ClassMap<SpecialFloor>
    {
        public SpecialFloorConfigMapper()
        {
            Map(m => m.Id).ConvertUsing(row => row.GetField<int>(0));
            Map(m => m.Rows).ConvertUsing(row => {
                var list = new List<SpecialRow>();
                for (var i = 1; i < 26; i++)
                {
                    var field = row.GetField<string>(i);
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    var rowConfig = new SpecialRow(field);
                    list.Add(rowConfig);
                }

                return list;
            });
        }
    }
}