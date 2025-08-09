using UnityEngine;

namespace Features.Rooms
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;

        public MeshRenderer MeshRenderer => meshRenderer;
    }
}
