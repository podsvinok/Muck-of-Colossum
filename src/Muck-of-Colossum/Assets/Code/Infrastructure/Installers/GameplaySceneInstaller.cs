using Code.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    { 
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform terrainParent;
        
        private ILevelDataProvider levelData;

        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }
        
        public override void InstallBindings()
        {
            levelData.StartPoint = startPoint.position;
            levelData.TerrainParent = terrainParent;
        }
    }
}