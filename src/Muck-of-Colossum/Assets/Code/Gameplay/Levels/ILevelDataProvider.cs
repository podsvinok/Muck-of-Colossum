using UnityEngine;

namespace Code.Gameplay.Levels
{
    public interface ILevelDataProvider
    {
        public Vector3 StartPoint { get; set; }
        public Transform TerrainParent { get; set; }
        public GameObject Player { get; set; }
    }
}