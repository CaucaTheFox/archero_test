using UnityEngine;

namespace Features.Rooms.Screens
{
    public class RoomScreen2D : MonoBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private Joystick joystick;
        #endregion

        #region Public
        public Joystick Joystick => joystick;
        #endregion
    }
}