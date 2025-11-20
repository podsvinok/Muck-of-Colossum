using Code.Gameplay.TerrainGeneration.Generators;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Infrastructure.StaticData;
using Code.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class TerrainGeneratorDebug : MonoBehaviour
{
    public bool autoUpdate;
    public float timeToDraw = 1f;

    public MeshSettings meshSettings;
    public NoiseSettings noiseSetting;
    public TextureSettings textureSettings;
    public HeightMapSettings heightMapSettings;

    private float lastDrawTime;
    private bool hasPendingChanges;

    private TerrainGenerator terrainGenerator;
    private IStaticDataService staticData;

    [Inject]
    public void Construct(TerrainGenerator terrainGenerator, IStaticDataService staticData)
    {
        this.terrainGenerator = terrainGenerator;
        this.staticData = staticData;
    }

    private void Start()
    {
        lastDrawTime = Time.timeSinceLevelLoad + 1;
        
        meshSettings = staticData.MeshSettings;
        noiseSetting = staticData.NoiseSettings;
        textureSettings = staticData.TextureSettings;
        heightMapSettings = staticData.HeightMapSettings;

        meshSettings.OnValuesUpdated += MarkAutoChanged;
        noiseSetting.OnValuesUpdated += MarkAutoChanged;
        textureSettings.OnValuesUpdated += MarkAutoChanged;
        heightMapSettings.OnValuesUpdated += MarkAutoChanged;
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

    private void Update()
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
            terrainGenerator.RegenerateTerrain();
        }
    }
}