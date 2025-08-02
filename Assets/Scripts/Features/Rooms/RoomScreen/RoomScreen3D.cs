using System;
using System.Collections.Generic;
using System.Linq;
using Core.CameraScripts;
using Core.IoC;
using UnityEngine;
using UnityEngine.AI;
using Utility.Utility;

namespace Features.Rooms.Screens
{
    public class RoomScreen3D : InjectableBehaviour
    {
        [Serializable]
        private struct BaseTileToTemplate
        {
            public Tile Template;
            public BaseFloorTile Tile;
        }

       [Serializable]
        private struct SpecialTileToTemplate
        {
            public Tile Template;
            public SpecialFloorTile Tile;
        }

        #region Events
        public Action<int> OnPlayerHit;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private TopDownCamera topDownCamera;
        [SerializeField] private NavMeshSurface baseFloorSurface;
        [SerializeField] private Transform heroContainer, enemyContainer, specialFloorContainer;
        [SerializeField] private BaseTileToTemplate[] baseTileTemplates;
        [SerializeField] private SpecialTileToTemplate[] specialTileTemplates;
        #endregion

        #region Dependencies
        [Inject] private IRoomsModel roomModel;
        #endregion

        #region Properties
        public TopDownCamera TopDownCamera => topDownCamera;
        public Transform HeroContainer => heroContainer;
        public Transform EnemyContainer => enemyContainer;
        #endregion

        #region State
        private Dictionary<BaseFloorTile, Tile> baseTileToPrefabMap;
        private Dictionary<SpecialFloorTile, Tile> specialTileToPrefabMap;
        #endregion

        #region Lifecycle
        private void Start()
        {
            baseTileToPrefabMap = baseTileTemplates.ToDictionary(x => x.Tile, y => y.Template);
            specialTileToPrefabMap = specialTileTemplates.ToDictionary(x => x.Tile, y => y.Template);
            InstantiateBaseFloorTiles();
            InstantiateSpecialFloorTiles();
            baseFloorSurface.BuildNavMesh();
        }

        #endregion

        #region Private
        private void InstantiateBaseFloorTiles()
        {
            var floorConfig = roomModel.GetRandomBaseFloorConfig();
            baseFloorSurface.transform.DestroyChildren();
            for (int i = 0; i < floorConfig.Rows.Count; i++)
            {
                var row = floorConfig.Rows[i];
                for (int j = 0; j < row.Tiles.Length; j++)
                {
                    var tile = row.Tiles[j];
                    if (!baseTileToPrefabMap.TryGetValue(tile, out var template))
                    {
                        throw new Exception($"[RoomScreen3D] No Template found for tile type {tile}");
                    }

                    var tileInstance = GameObject.Instantiate(template, baseFloorSurface.transform);
                    tileInstance.transform.position = new Vector3(j, 0, i);
                }
            }
        }

        private void InstantiateSpecialFloorTiles()
        {
            var floorConfig = roomModel.GetRandomSpecialFloorConfig();
            specialFloorContainer.DestroyChildren();
            for (int i = 0; i < floorConfig.Rows.Count; i++)
            {
                var row = floorConfig.Rows[i];
                for (int j = 0; j < row.Tiles.Length; j++)
                {
                    var tile = row.Tiles[j];
                    if (tile == SpecialFloorTile.Undefined)
                    {
                        continue;
                    }

                    if (!specialTileToPrefabMap.TryGetValue(tile, out var template))
                    {
                        throw new Exception($"[RoomView] No Template found for tile type {tile}");
                    }

                    var tileInstance = Instantiate(template, specialFloorContainer);
                    tileInstance.transform.position = new Vector3(j, tileInstance.transform.position.y, i);
                    if (tileInstance is TrapTile trap)
                    {
                        trap.OnPlayerHit += HandlePlayerHit;
                    }
                }
            }
        }

        private void HandlePlayerHit(int damage)
        {
            OnPlayerHit?.Invoke(damage);
        }
        #endregion

    }
}
