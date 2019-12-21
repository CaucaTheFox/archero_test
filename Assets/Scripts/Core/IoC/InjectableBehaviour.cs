using UnityEngine;

namespace Core.IoC
{
    public class InjectableBehaviour: MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameContext.Container.ResolveAll(this);
        }
    }
}