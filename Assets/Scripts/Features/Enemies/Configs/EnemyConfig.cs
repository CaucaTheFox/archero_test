using Newtonsoft.Json;
using System.Collections.Generic;

namespace Features.Enemies
{
    public class EnemyConfig
    {
        public Dictionary<string, EnemySettings> Enemies;

        [JsonConstructor]
        public EnemyConfig(Dictionary<string, EnemySettings> enemies)
        {
            Enemies = enemies;
        }

        public EnemyConfig(List<EnemySettings> enemies)
        {
            Enemies = new Dictionary<string, EnemySettings>();
            foreach (var enemy in enemies)
            {
                Enemies.Add(enemy.Id, enemy);
            }
        }
    }

    public class EnemySettings
    {
        public string Id;
        public string Name;
        public int Health;
        public int MeleeDamage;
        public int ParticleDamage;
        public int RangedDamage;
    }
}
