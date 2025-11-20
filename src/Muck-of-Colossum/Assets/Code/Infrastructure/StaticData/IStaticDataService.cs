using Code.Gameplay.TerrainGeneration.StaticData;
using Cysharp.Threading.Tasks;
using NoiseSettings = Unity.Cinemachine.NoiseSettings;

namespace Code.Infrastructure.StaticData
{
    public interface IStaticDataService
    {
        public HeightMapSettings HeightMapSettings { get; set; }
        public MeshSettings MeshSettings { get; set; }
        public TextureSettings TextureSettings { get; set; }
        public Gameplay.TerrainGeneration.StaticData.NoiseSettings NoiseSettings { get; set; }
        
        public UniTask LoadAllAsync();
        public UniTask LoadTerrainGenerationSettings();
    }
}