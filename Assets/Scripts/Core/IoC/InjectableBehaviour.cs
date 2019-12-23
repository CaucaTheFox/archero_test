using UnityEngine;

namespace Core.IoC
{
    public class InjectableBehaviour: MonoBehaviour
    {
        public virtual void Awake()
        {
            GameContext.Container.ResolveAll(this);
        }
    }
}