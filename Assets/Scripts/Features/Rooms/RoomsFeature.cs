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
            container.RegisterSingleton<IJsonConfig<RoomsConfig>, RoomsConfigContainer>();
        }

        private class RoomsConfigContainer : JsonConfig<RoomsConfig>
        {
            protected override string ConfigPath => "Configs/Rooms/RoomsConfig";
        }
    }
}
