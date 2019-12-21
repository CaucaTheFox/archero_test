using Newtonsoft.Json;
using System.Collections.Generic;

namespace Features.Rooms
{
    public class RoomsConfig
    {
        public Dictionary<int, RoomConfig> Rooms; 

        [JsonConstructor]
        public RoomsConfig(Dictionary<int, RoomConfig> rooms)
        {
            Rooms = rooms;
        }

        public RoomsConfig(List<RoomConfig> roomConfigs)
        {
            Rooms = new Dictionary<int, RoomConfig>();
            foreach (var room in roomConfigs)
            {
                Rooms.Add(room.Id, room);
            }
        }
    }

    public class RoomConfig
    {
        public int Id; 
        public List<RoomRowConfig> Rows;
    }
    public class RoomRowConfig
    {
        public RoomTile[] Tiles;

        [JsonConstructor]
        public RoomRowConfig(RoomTile[] tiles)
        {
            Tiles = tiles;
        }

        public RoomRowConfig(string tiles)
        {
            var tileArray = tiles.Split(':');
            Tiles = new RoomTile[tileArray.Length];
            for (int i = 0; i < tileArray.Length; i++)
            {
                var tile = tileArray[i];
                switch (tile)
                {
                    case "G":
                        Tiles[i] = RoomTile.Grass;
                        break;
                    case "W":
                        Tiles[i] = RoomTile.Water;
                        break;
                    case "T":
                        Tiles[i] = RoomTile.Trap;
                        break;
                    case "B":
                        Tiles[i] = RoomTile.Bridge;
                        break;
                }
            }
        }
    }

    public enum RoomTile
    {
        Grass,
        Water,
        Trap,
        Bridge
    }
}
