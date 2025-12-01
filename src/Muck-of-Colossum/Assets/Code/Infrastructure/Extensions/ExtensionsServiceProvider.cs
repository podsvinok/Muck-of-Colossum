using Code.Gameplay.Item;
using Code.Utils;
using Zenject;

namespace Code.Infrastructure.Extensions
{
    public class ExtensionsServiceProvider : IExtensionsServiceProvider, IInitializable
    {
        private readonly ItemDatabase database;

        public ExtensionsServiceProvider(ItemDatabase database)
        {
            this.database = database;
        }

        public void Initialize()
        {
            ProvideServices();
        }

        public void ProvideServices()
        {
            InventoryDataUtils.Initialize(database);
        }
    }
}