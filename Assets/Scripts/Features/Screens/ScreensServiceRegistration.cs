using Core;
using Core.IoC;

namespace Features.Screens
{
    public class ScreensServiceRegistration: IServiceRegistration
    {
        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<ISceneLayers, SceneLayers>();
            container.RegisterSingleton<IScreenManager, ScreenManager>();
            container.RegisterSingleton<IControlsManager,ControlsManager>();

        }
    }
}