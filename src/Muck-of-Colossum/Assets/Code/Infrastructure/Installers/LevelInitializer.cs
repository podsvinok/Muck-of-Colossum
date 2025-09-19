using Code.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class LevelInitializer : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform startPoint;

        private ILevelDataProvider levelData;
        
        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }

        public void Initialize()
        {
            levelData.SetStartPoint(startPoint.position);
        }
    }
}