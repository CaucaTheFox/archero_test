using Features.Screens;

namespace Features.Rooms.Screens
{
    public class RoomScreenController: DualScreenController<RoomScreen2D, RoomScreen3D>
    {
        #region - Constants
        public const string ScreenName = "RoomScreen";
        #endregion
        
        #region - Properties
        public override string Name => ScreenName;
        #endregion

        #region - Dependencies
        #endregion
        
        #region - State
        #endregion
        
        #region - Lifecycle
        public override void Init()
        {

        }
        #endregion

        #region - Private
      
        #endregion
    }
}
