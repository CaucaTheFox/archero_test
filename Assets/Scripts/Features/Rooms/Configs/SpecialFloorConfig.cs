using Newtonsoft.Json;
using System.Collections.Generic;

namespace Features.Rooms
{
    public class SpecialFloorConfig
    {
        public Dictionary<int, SpecialFloor> SpecialFloors; 

        [JsonConstructor]
        public SpecialFloorConfig(Dictionary<int, SpecialFloor> rooms)
        {
            SpecialFloors = rooms;
        }

        public SpecialFloorConfig(List<SpecialFloor> roomConfigs)
        {
            SpecialFloors = new Dictionary<int, SpecialFloor>();
            foreach (var room in roomConfigs)
            {
                SpecialFloors.Add(room.Id, room);
            }
        }
    }

    public class SpecialFloor
    {
        public int Id;
        public List<SpecialRow> Rows;
    }

    public class SpecialRow
    {
        public SpecialFloorTile[] Tiles;

        [JsonConstructor]
        public SpecialRow(SpecialFloorTile[] tiles)
        {
            Tiles = tiles;
        }

        public SpecialRow(string tiles)
        {
            var tileArray = tiles.Split(':');
            Tiles = new SpecialFloorTile[tileArray.Length];
            for (int i = 0; i < tileArray.Length; i++)
            {
                var tile = tileArray[i];
                switch (tile)
                {
                    case "U":
                        Tiles[i] = SpecialFloorTile.Undefined;
                        break;
                    case "W":
                        Tiles[i] = SpecialFloorTile.Wall;
                        break;
                    case "T":
                        Tiles[i] = SpecialFloorTile.Trap;
                        break;
                }
            }
        }
    }

    public enum SpecialFloorTile
    {
        Undefined,
        Wall,
        Trap,
    }
}
