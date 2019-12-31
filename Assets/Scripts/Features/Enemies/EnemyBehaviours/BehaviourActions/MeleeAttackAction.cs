using UnityEngine;

namespace Features.Enemies
{
    public class MeleeAttackAction : BehaviourAction
    {
        [SerializeField] private string[] attackNames;

        public override void Execute()
        {
            base.Execute();
            enemyActor.SetSpeed(0f);
            enemyActor.MeleeAttack(GetRandomAttackName());
        }

        private string GetRandomAttackName()
        {
            var randomIndex = Random.Range(0, attackNames.Length);
            return attackNames[randomIndex];
        }
    }
}