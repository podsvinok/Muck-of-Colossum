using Code.Gameplay.Item;
using FishNet.Serializing;

namespace Code.Utils
{
    public static class InventoryDataUtils
    {
        private static ItemDatabase database;

        public static void Initialize(ItemDatabase _database)
        {
            database = _database;
        }
        
        public static void WriteItemPreset(this Writer writer, ItemPreset preset)
        {
            if (preset == null)
            {
                writer.WriteBoolean(false);
                return;
            } 
            writer.WriteBoolean(true);
            writer.WriteString(preset.uid);
        }

        public static ItemPreset ReadItemPreset(this Reader reader)
        {
            bool hasPreset = reader.ReadBoolean();
            if (!hasPreset)
                return null;

            var uid = reader.ReadStringAllocated();
            database.TryGetItemPreset(uid, out var preset);
            
            
            return preset;
        }
    }
}