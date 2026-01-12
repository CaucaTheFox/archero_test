using Core;
using Core.IoC;
using Features.Rooms.Screens;

namespace Features.Screens
{
    public class ScreensInitialization: IFeatureInitialization, IServiceRegistration
    {
        [Inject] private IScreenManager screenManager;
        
        public void Init()
        {
            screenManager.RegisterScreen(
                typeof(RoomScreenController),
                "Screens/RoomScreen/RoomScreen2D",
                "Screens/RoomScreen/RoomScreen3D"
            );
        }

        public void RegisterServices(IIoC container)
        {
        }
    }
}