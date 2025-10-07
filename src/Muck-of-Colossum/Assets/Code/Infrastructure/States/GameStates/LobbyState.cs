using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class LobbyState : IState
    {
        public UniTask Exit() => 
            default;

        public UniTask Enter() => 
            default;
    }
}