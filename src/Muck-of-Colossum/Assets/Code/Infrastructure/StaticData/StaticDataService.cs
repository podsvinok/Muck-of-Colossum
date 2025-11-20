using System;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Infrastructure.AssetManagement;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        public HeightMapSettings HeightMapSettings { get; set; }
        public MeshSettings MeshSettings { get; set; }
        public TextureSettings TextureSettings { get; set; }
        public NoiseSettings NoiseSettings { get; set; }

        private readonly IAssetProvider assetProvider;

        public StaticDataService(IAssetProvider assetProvider)
        {
            this.assetProvider = assetProvider;
        }

        public async UniTask LoadAllAsync()
        {
            await LoadTerrainGenerationSettings();
        }

        public async UniTask LoadTerrainGenerationSettings()
        {
            await UniTask.WhenAll(
                LoadHeightMapSettings(),
                LoadMeshSettings(),
                LoadTextureSettings(),
                LoadNoiseSettings());
        }

        private async UniTask LoadHeightMapSettings() => 
            HeightMapSettings = await assetProvider.Load<HeightMapSettings>(AssetPath.HeightMapSettings);

        private async UniTask LoadMeshSettings() => 
            MeshSettings = await assetProvider.Load<MeshSettings>(AssetPath.MeshSettings);

        private async UniTask LoadTextureSettings() => 
            TextureSettings = await assetProvider.Load<TextureSettings>(AssetPath.TextureSettings);
        
        private async UniTask LoadNoiseSettings() => 
            NoiseSettings = await assetProvider.Load<NoiseSettings>(AssetPath.NoiseSettings);
    }
}