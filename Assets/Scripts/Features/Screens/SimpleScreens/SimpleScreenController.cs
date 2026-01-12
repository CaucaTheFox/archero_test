using Core.IoC;

namespace Features.Screens
{
    public abstract class SimpleScreenController: InjectableBehaviour, IScreenController
    {
        public abstract string Name { get; }
        public bool Visible {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
        
        public void DestroyScreen()
        {
            Destroy(gameObject);
        }
    }
}