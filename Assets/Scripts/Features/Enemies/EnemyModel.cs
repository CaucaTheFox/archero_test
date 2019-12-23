using Core.IoC;

namespace Features.Enemies
{
    public interface IEnemyModel
    {
        EnemySettings GetSettings();
    }

    public class EnemyModel : IEnemyModel
    {
        #region Dependencies
        #endregion

        #region State
        private EnemySettings settings;
        #endregion

        #region Public
        public EnemyModel(EnemySettings settings)
        {
            this.settings = settings;
        }

        public EnemySettings GetSettings()
        {
            return settings;
        }
        #endregion
    }
}
