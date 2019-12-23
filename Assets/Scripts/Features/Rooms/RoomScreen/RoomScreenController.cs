using Core.CameraScripts;
using Core.IoC;
using Core.ResourceManagement;
using Features.Heroes;
using Features.Screens;
using System;
using UnityEngine;

namespace Features.Rooms.Screens
{
    public class RoomScreenController: DualScreenController<RoomScreen2D, RoomScreen3D>
    {
        #region - Constants
        public const string ScreenName = "RoomScreen";
        private const string DefaultHeroId = "archer_green";
        private const string DefaultHeroPath = "HeroPrefabs/archer_green";
        #endregion

        #region - Properties
        public override string Name => ScreenName;
        public TopDownCamera TopDownCamera { get; private set; }
        #endregion

        #region - Dependencies
        [Inject] private IHeroModel heroModel;
        [Inject] private IResourceManager resourceManager;
        #endregion

        #region - State
        private Hero hero;
        #endregion

        #region - Lifecycle
        public override void Init()
        {
            TopDownCamera = Camera.main.GetComponent<TopDownCamera>();
            if (TopDownCamera == null)
            {
                throw new Exception("[RoomScreenController] No TopDownCamera component found on main camera.");
            }

            var heroTemplate = resourceManager.LoadResource<Hero>(DefaultHeroPath);
            var heroSettings = heroModel.GetHeroBaseSettings(DefaultHeroId);
        
            hero = UnityEngine.Object.Instantiate(heroTemplate, Screen3D.HeroContainer);
            hero.Settings = heroSettings;
            TopDownCamera.CameraTarget = hero.transform;
            Screen2D.Joystick.OnUpdate += HandlePlayerInput;
        }

        #endregion

        #region - Private
        private void HandlePlayerInput(bool isPointerDown)
        {
            if (isPointerDown)
            {
                var input = new Vector3(Screen2D.Joystick.Horizontal, 0, Screen2D.Joystick.Vertical);
                hero.MoveCharacter(input);
            }
            else
            {
                hero.Shoot();
            }
        }
        #endregion
    }
}
