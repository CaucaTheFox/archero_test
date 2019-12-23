using Core.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.Utility;

namespace Features.Rooms.Screens
{
    public class RoomScreen3D : InjectableBehaviour
    {
        [Serializable]
        private struct TileToTemplate
        {
            public GameObject Template;
            public RoomTile Tile;
        }

        #region Unity Serialized Fields
        [SerializeField] private Transform tilesContainer, heroContainer;
        [SerializeField] private TileToTemplate[] tileTemplates;
        #endregion

        #region Dependencies
        [Inject] IRoomsModel roomModel;
        #endregion

        #region Properties
        public Transform HeroContainer => heroContainer;
        #endregion

        #region State
        private Dictionary<RoomTile, GameObject> tileToPrefabMap;
        #endregion

        #region Lifecycle
        private void Start()
        {
            tileToPrefabMap = tileTemplates.ToDictionary(x => x.Tile, y => y.Template);
            //SpawnTiles();
        }

        #endregion

        #region Private
        private void SpawnTiles()
        {
            var roomConfig = roomModel.GetRandomRoomConfig();
            tilesContainer.DestroyChildren();
            for (int i = 0; i < roomConfig.Rows.Count; i++)
            {
                var row = roomConfig.Rows[i];
                for (int j = 0; j < row.Tiles.Length; j++)
                {
                    var tile = row.Tiles[j];
                    if (!tileToPrefabMap.TryGetValue(tile, out var template))
                    {
                        throw new Exception($"[RoomView] No Template found for tile type {tile}");
                    }

                    var tileInstance = GameObject.Instantiate(template, tilesContainer);
                    tileInstance.transform.position = new Vector3(j, 0, i);
                }
            }
        }
        #endregion

    }
}
