using Code.Infrastructure.States.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.UI.HUD
{
    public class ExitGameState : IState
    {
        public UniTask Exit()
        {
            Application.Quit();
            return default;
        }

        public UniTask Enter()
        {
            Debug.Log(":(");
            return default;
        }
    }
}