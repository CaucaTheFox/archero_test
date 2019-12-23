using Core;
using Core.IoC;

namespace Features.Rooms
{

    public class RoomsFeature : IFeatureInitialization, IServiceRegistration
    {
        [Inject] private IRoomsModel roomsModel;

        public void Init()
        {

        }

        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IRoomsModel, RoomsModel>();
            container.RegisterSingleton<IJsonConfig<BaseFloorConfig>, BaseFloorConfigContainer>();
            container.RegisterSingleton<IJsonConfig<SpecialFloorConfig>, SpecialFloorConfigContainer>();
        }

        private class BaseFloorConfigContainer : JsonConfig<BaseFloorConfig>
        {
            protected override string ConfigPath => "Configs/Rooms/BaseFloorConfig";
        }

        private class SpecialFloorConfigContainer : JsonConfig<SpecialFloorConfig>
        {
            protected override string ConfigPath => "Configs/Rooms/SpecialFloorConfig";
        }
    }
}
