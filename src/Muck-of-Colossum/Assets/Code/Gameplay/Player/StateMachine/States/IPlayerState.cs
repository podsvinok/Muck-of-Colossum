
namespace Code.Gameplay.Player.StateMachine.States
{
   public interface IPlayerState
   {
      void Enter();
      void Exit();
      void HandleInput();
      void Update();
   }
}
