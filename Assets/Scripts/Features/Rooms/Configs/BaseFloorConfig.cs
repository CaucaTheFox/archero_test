using Newtonsoft.Json;
using System.Collections.Generic;

namespace Features.Rooms
{
    public class BaseFloorConfig
    {
        public Dictionary<int, BaseFloor> BaseFloors; 

        [JsonConstructor]
        public BaseFloorConfig(Dictionary<int, BaseFloor> rooms)
        {
            BaseFloors = rooms;
        }

        public BaseFloorConfig(List<BaseFloor> roomConfigs)
        {
            BaseFloors = new Dictionary<int, BaseFloor>();
            foreach (var room in roomConfigs)
            {
                BaseFloors.Add(room.Id, room);
            }
        }
    }

    public class BaseFloor
    {
        public int Id; 
        public List<BasicRow> Rows;
    }
    public class BasicRow
    {
        public BaseFloorTile[] Tiles;

        [JsonConstructor]
        public BasicRow(BaseFloorTile[] tiles)
        {
            Tiles = tiles;
        }

        public BasicRow(string tiles)
        {
            var tileArray = tiles.Split(':');
            Tiles = new BaseFloorTile[tileArray.Length];
            for (int i = 0; i < tileArray.Length; i++)
            {
                var tile = tileArray[i];
                switch (tile)
                {
                    case "G":
                        Tiles[i] = BaseFloorTile.Grass;
                        break;
                    case "W":
                        Tiles[i] = BaseFloorTile.Water;
                        break;
                }
            }
        }
    }

    public enum BaseFloorTile
    {
        Grass,
        Water
    }
}
