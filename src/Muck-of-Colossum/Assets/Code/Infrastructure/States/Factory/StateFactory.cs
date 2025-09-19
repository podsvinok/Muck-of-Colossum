using Code.Infrastructure.States.GameStates;
using Zenject;

namespace Code.Infrastructure.States.Factory
{
    public class StateFactory : IStateFactory
    {
        private readonly DiContainer container;

        public StateFactory(DiContainer container)
        {
            this.container = container;
        }

        public TState GetState<TState>() where TState : class, IExitableState
        {
            return container.Resolve<TState>();
        }
    }
}