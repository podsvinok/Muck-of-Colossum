using Code.Gameplay.Levels;
using FishNet.Object;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerNetworkInjecter : NetworkBehaviour
    {
        private ILevelDataProvider levelData;

        [Inject] 
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
                return;
            
            levelData.Player = gameObject;
        }
    }
}