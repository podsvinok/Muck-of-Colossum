using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public interface IPayloadState<TPayload> : IExitableState
    {
        UniTask Enter(TPayload payload);
    }
}