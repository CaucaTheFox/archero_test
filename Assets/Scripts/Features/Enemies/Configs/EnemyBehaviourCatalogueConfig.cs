using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Enemies
{
    [Serializable]
    public class EnemyBehaviourCatalogueConfig
    {
        public Dictionary<string, EnemyBehaviourConfig> Configs;
        
        [SerializeField] private List<EnemyBehaviourConfig> configList;
        
        [JsonConstructor]
        public EnemyBehaviourCatalogueConfig(Dictionary<string, EnemyBehaviourConfig> configs)
        {
            Configs = configs;
            configList = configs.Values.ToList();
        }

        public EnemyBehaviourCatalogueConfig(List<EnemyBehaviourConfig> enemyBehaviours)
        {
            Configs = new Dictionary<string, EnemyBehaviourConfig>();
            configList = new List<EnemyBehaviourConfig>();
            foreach (var enemy in enemyBehaviours)
            {
                Configs.Add(enemy.Id, enemy);
                configList.Add(enemy);
            }
        }

        public void AddEnemyBehaviourConfig(EnemyBehaviourConfig enemyBehaviourConfig)
        {
            if (Configs.TryAdd(enemyBehaviourConfig.Id, enemyBehaviourConfig))
            {
                configList.Add(enemyBehaviourConfig);
            }
        }
    }

    [Serializable]
    public class EnemyBehaviourConfig
    {
        [HideInInspector] public string Id; 
        public List<EnemyBehaviourActionData> EnemyBehaviourActionData;
    }
}
