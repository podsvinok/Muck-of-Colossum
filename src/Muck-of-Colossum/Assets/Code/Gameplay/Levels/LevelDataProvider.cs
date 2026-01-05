using Code.Gameplay.InventorySystem;
using UnityEngine;

namespace Code.Gameplay.Levels
{
    public class LevelDataProvider : ILevelDataProvider
    {
        public Vector3 StartPoint { get; set; }
        public Transform TerrainParent { get; set; }
        public GameObject Player { get; set; }
        public InventoryView InventoryView { get; set; }
    }
}