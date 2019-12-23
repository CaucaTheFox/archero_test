using Core.IoC;
using Core.ResourceManagement;
using Core.Time;

namespace Core
{
    public class CoreServiceRegistration: IServiceRegistration
    {
        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IResourceManager, ResourceManager>();
            container.RegisterSingleton<ITimeProvider, TimeProvider>();
        }
    }
}