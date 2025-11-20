using Code.Gameplay.Levels;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Network;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    { 
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform terrainParent;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameplayLoadingStateRPCs loadingStateRPCs;
        
        private ILevelDataProvider levelData;
        
        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }
        
        public override void InstallBindings()
        {
            levelData.GameplayLoadingStateRPCs = loadingStateRPCs;
            levelData.StartPoint = startPoint.position;
            levelData.TerrainParent = terrainParent;
            levelData.Camera = cameraTransform;
        }
    }
}