using Code.Network;
using UnityEngine;

namespace Code.Gameplay.Levels
{
    public class LevelDataProvider : ILevelDataProvider
    {
        public Vector3 StartPoint { get; set; }
        public Transform TerrainParent { get; set; }
        public Transform Camera { get; set; }
        public Transform Player { get; set; }
        public GameplayLoadingStateRPCs GameplayLoadingStateRPCs { get; set; }
    }
}