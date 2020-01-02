using Core;
using Core.IoC;
using Features.Rooms.Screens;

namespace Features.Screens
{
    public class ScreensInitialization: IFeatureInitialization, IServiceRegistration
    {
#pragma warning disable 649
        [Inject] private IScreenManager screenManager;
#pragma warning restore 649
        
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