using Core.IoC;
using Features.Rooms.Screens;
using Features.Screens;

namespace Core
{
    public class GameLauncher : InjectableBehaviour
    {
        #region Dependencies
        [Inject] private IScreenManager screenManager;
        #endregion

        private void Start()
        {
            screenManager.Push<RoomScreenController>(RoomScreenController.ScreenName);
        }
    }
}