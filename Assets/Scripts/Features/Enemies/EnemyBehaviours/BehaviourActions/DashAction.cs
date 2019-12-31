using UnityEngine;

namespace Features.Enemies
{
    public class DashAction : BehaviourAction
    {
        #region Unity Serialized Field
        [SerializeField] private float dashSpeed;
        #endregion

        public override void Execute()
        {
            base.Execute();
            enemyActor.SetSpeed(dashSpeed);
            enemyActor.MoveTorwardsTarget(heroModel.HeroPosition);
        }
    }
}