using Core.IoC;
using UnityEngine;

namespace Features.Rooms
{
    public interface IRoomsModel
    {
        RoomConfig GetRandomRoomConfig();
    }

    public class RoomsModel : IRoomsModel
    {
        #region Dependencies
        [Inject] private IJsonConfig<RoomsConfig> roomsConfig;
        #endregion

        #region Public
        public RoomConfig GetRandomRoomConfig()
        {
            var rooms = roomsConfig.Value.Rooms;
            var randomIndex = Random.Range(0,rooms.Count);
            return rooms[randomIndex]; 
        }
        #endregion
    }
}
