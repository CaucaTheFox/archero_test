using Core.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
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
            public GameObject Template;
            public BaseFloorTile Tile;
        }

       [Serializable]
        private struct SpecialTileToTemplate
        {
            public GameObject Template;
            public SpecialFloorTile Tile;
        }

        #region Unity Serialized Fields
        [SerializeField] private NavMeshSurface baseFloorSurface;
        [SerializeField] private Transform heroContainer, specialFloorContainer;
        [SerializeField] private BaseTileToTemplate[] baseTileTemplates;
        [SerializeField] private SpecialTileToTemplate[] specialTileTemplates;
        #endregion

        #region Dependencies
        [Inject] IRoomsModel roomModel;
        #endregion

        #region Properties
        public Transform HeroContainer => heroContainer;
        #endregion

        #region State
        private Dictionary<BaseFloorTile, GameObject> baseTileToPrefabMap;
        private Dictionary<SpecialFloorTile, GameObject> specialTileToPrefabMap;
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

                    var tileInstance = GameObject.Instantiate(template, specialFloorContainer);
                    tileInstance.transform.position = new Vector3(j, specialFloorContainer.position.y, i);
                }
            }
        }

        #endregion

    }
}
