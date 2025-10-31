using Code.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    { 
        [SerializeField] private Transform startPoint;
        
        private ILevelDataProvider levelData;
        
        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }
        
        public override void InstallBindings()
        {
            levelData.SetStartPoint(startPoint.position);
        }
    }
}