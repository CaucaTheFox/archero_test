using Core;
using Core.IoC;

namespace Features.Rooms
{

    public class RoomsFeature : IFeatureInitialization, IServiceRegistration
    {
        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IRoomsModel, RoomsModel>();
        }
    }
}
