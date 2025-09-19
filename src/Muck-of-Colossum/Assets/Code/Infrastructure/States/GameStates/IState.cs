using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public interface IState : IExitableState
    {
        UniTask Enter();
    }
}