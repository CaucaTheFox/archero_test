using System;
using UnityEngine;

namespace Utility
{
    public class AnimationEventProvider : MonoBehaviour
    {
        public event Action OnAttack; 

        public void AttackEvent()
        {
            OnAttack?.Invoke();
        }
    }
}