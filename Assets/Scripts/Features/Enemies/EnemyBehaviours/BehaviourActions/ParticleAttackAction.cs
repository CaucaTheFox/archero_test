using UnityEngine;

namespace Features.Enemies
{
    public class ParticleAttackAction : BehaviourAction
    {
        public override void Execute()
        {
            base.Execute();
            enemyActor.SetSpeed(0f);
            enemyActor.ParticleAttack();
        }

        public override void Exit()
        {
            enemyActor.EndParticleAttack();
            base.Exit();
        }
    }
}