using Core.IoC;

namespace Features.Heroes
{
    public class Hero : InjectableBehaviour
    {
        #region Properties
        public HeroSettings Settings {get;set;}
        #endregion
    }
}
