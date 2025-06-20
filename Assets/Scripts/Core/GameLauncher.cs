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

        #region Lifecycle
        private void Start()
        {
            GameContext.Init();
            screenManager.Push<RoomScreenController>(RoomScreenController.ScreenName);
        }
        #endregion
    }
}