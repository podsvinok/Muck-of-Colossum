using Code.Gameplay.TerrainGeneration.Generators;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code.Gameplay.TerrainGeneration.Debug
{
    public class TerrainGeneratorDebug : MonoBehaviour
    {
        public bool autoUpdate;
        public float timeToDraw = 1f;

        public MeshSettings meshSettings;
        public NoiseSettings noiseSetting;
        public TextureSettings textureSettings;
        public HeightMapSettings heightMapSettings;
        public ResourceSettings resourceSettings;

        private float lastDrawTime;
        private bool hasPendingChanges;

        private TerrainGenerator terrainGenerator;

        [Inject]
        public void Construct(TerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        private void Start()
        {
            lastDrawTime = Time.timeSinceLevelLoad + timeToDraw;
        
            meshSettings.OnValuesUpdated += MarkAutoChanged;
            noiseSetting.OnValuesUpdated += MarkAutoChanged;
            textureSettings.OnValuesUpdated += MarkAutoChanged;
            heightMapSettings.OnValuesUpdated += MarkAutoChanged;
            resourceSettings.OnValuesUpdated += MarkAutoChanged;
        }

        private void MarkAutoChanged()
        {
            if (autoUpdate)
                MarkChanged();
        }
    
        private void MarkChanged() => 
            hasPendingChanges = true;

        public void Generate() => 
            MarkChanged();

        private async void Update()
        {
            if (!Application.isPlaying)
                return;

            if (SceneManager.GetActiveScene().name != Scenes.GameScene)
                return;

            if (!hasPendingChanges)
                return;

            if (Time.timeSinceLevelLoad > lastDrawTime + timeToDraw)
            {
                lastDrawTime = Time.timeSinceLevelLoad;
                hasPendingChanges = false;
                await terrainGenerator.RegenerateTerrain();
            }
        }
    }
}