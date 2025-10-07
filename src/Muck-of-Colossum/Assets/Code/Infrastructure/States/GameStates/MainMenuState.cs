using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class MainMenuState : IState
    {
        public UniTask Exit() => 
            default;

        public UniTask Enter() => 
            default;
    }
}