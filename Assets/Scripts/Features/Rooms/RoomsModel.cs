using Core.IoC;
using UnityEngine;

namespace Features.Rooms
{
    public interface IRoomsModel
    {
        BaseFloor GetRandomBaseFloorConfig();
        SpecialFloor GetRandomSpecialFloorConfig();
    }

    public class RoomsModel : IRoomsModel
    {
        #region Dependencies
        [Inject] private IJsonConfig<BaseFloorConfig> baseFloorsConfig;
        [Inject] private IJsonConfig<SpecialFloorConfig> specialFloorsConfig;
        #endregion

        #region Public
        public BaseFloor GetRandomBaseFloorConfig()
        {
            var rooms = baseFloorsConfig.Value.BaseFloors;
            var randomIndex = Random.Range(0,rooms.Count);
            return rooms[randomIndex]; 
        }

        public SpecialFloor GetRandomSpecialFloorConfig()
        {
            var rooms = specialFloorsConfig.Value.SpecialFloors;
            var randomIndex = Random.Range(0, rooms.Count);
            return rooms[randomIndex];
        }
        #endregion
    }
}
