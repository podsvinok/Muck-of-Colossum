using Code.Gameplay.Player.Crafting;

namespace Code.UI.Windows
{
    public class CraftingWindow : WindowBase
    {
        public CraftingView craftingView;

        public override void Show()
        {
            base.Show();
            craftingView.UpdateAvailability();
        }
    }
}