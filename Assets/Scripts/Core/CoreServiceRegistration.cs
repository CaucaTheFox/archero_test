using Core.IoC;
using Core.ResourceManagement;

namespace Core
{
    public class CoreServiceRegistration: IServiceRegistration
    {
        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IResourceManager, ResourceManager>();
        }
    }
}