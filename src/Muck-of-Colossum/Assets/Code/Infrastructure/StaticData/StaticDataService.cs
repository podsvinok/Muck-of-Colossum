using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Infrastructure.AssetManagement;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Code.UI.Windows.StaticData;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        public HeightMapSettings HeightMapSettings { get; set; }
        public MeshSettings MeshSettings { get; set; }
        public TextureSettings TextureSettings { get; set; }
        public NoiseSettings NoiseSettings { get; set; }

        private readonly IAssetProvider assetProvider;
        private Dictionary<WindowId, WindowConfig> windowConfigs;

        public StaticDataService(IAssetProvider assetProvider)
        {
            this.assetProvider = assetProvider;
        }

        public async UniTask LoadAllAsync()
        {
            await LoadTerrainGenerationSettings();
            await LoadWindowConfigs();
        }

        public WindowConfig ForWindow(WindowId windowId)
            => windowConfigs.GetValueOrDefault(windowId);

        private async UniTask LoadTerrainGenerationSettings()
        {
            await UniTask.WhenAll(
                LoadHeightMapSettings(),
                LoadMeshSettings(),
                LoadTextureSettings(),
                LoadNoiseSettings());
        }

        private async UniTask LoadWindowConfigs()
        {
            var windowsStaticData = await assetProvider
                .LoadAsync<WindowStaticData>(AssetPath.WindowStaticData);
            
            windowConfigs = windowsStaticData
                .Configs
                .ToDictionary(x => x.WindowId, x => x);
        }

        private async UniTask LoadHeightMapSettings() => 
            HeightMapSettings = await assetProvider.LoadAsync<HeightMapSettings>(AssetPath.HeightMapSettings);

        private async UniTask LoadMeshSettings() => 
            MeshSettings = await assetProvider.LoadAsync<MeshSettings>(AssetPath.MeshSettings);

        private async UniTask LoadTextureSettings() => 
            TextureSettings = await assetProvider.LoadAsync<TextureSettings>(AssetPath.TextureSettings);
        
        private async UniTask LoadNoiseSettings() => 
            NoiseSettings = await assetProvider.LoadAsync<NoiseSettings>(AssetPath.NoiseSettings);
    }
}