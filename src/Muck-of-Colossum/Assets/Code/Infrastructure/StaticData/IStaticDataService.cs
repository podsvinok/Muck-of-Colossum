using Code.Gameplay.TerrainGeneration.StaticData;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StaticData
{
    public interface IStaticDataService
    {
        public HeightMapSettings HeightMapSettings { get; set; }
        public MeshSettings MeshSettings { get; set; }
        public TextureSettings TextureSettings { get; set; }
        public NoiseSettings NoiseSettings { get; set; }
        
        public UniTask LoadAllAsync();
        public WindowConfig ForWindow(WindowId windowId);
    }
}