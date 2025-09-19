using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public interface IExitableState
    {
        UniTask Exit();
    }
}