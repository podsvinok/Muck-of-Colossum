using System.Linq;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu()]
    public class TextureSettings : UpdatableData
    {
        public Material mapMaterial;
        public Layer[] layers;
        public HeightMapSettings heightMapSettings;
        
        private const int TextureSize = 512;
        private const TextureFormat TextureFormat = UnityEngine.TextureFormat.RGB565;
        
        private readonly int layerCount = Shader.PropertyToID("layerCount");
        private readonly int baseColours = Shader.PropertyToID("baseColours");
        private readonly int baseStartHeights = Shader.PropertyToID("baseStartHeights");
        private readonly int baseBlends = Shader.PropertyToID("baseBlends");
        private readonly int baseColourStrength = Shader.PropertyToID("baseColourStrength");
        private readonly int baseTextureScales = Shader.PropertyToID("baseTextureScales");
        private readonly int baseTextures = Shader.PropertyToID("baseTextures");
        private readonly int minHeight = Shader.PropertyToID("minHeight");
        private readonly int maxHeight = Shader.PropertyToID("maxHeight");

        public void ApplyToMaterial()
        {
            mapMaterial.SetInt(layerCount, layers.Length);
            
            mapMaterial.SetColorArray(baseColours, layers.Select(x => x.tint).ToArray());
            mapMaterial.SetFloatArray(baseStartHeights, layers.Select(x => x.startHeight).ToArray());
            mapMaterial.SetFloatArray(baseBlends, layers.Select(x => x.blendStrength).ToArray());
            mapMaterial.SetFloatArray(baseColourStrength, layers.Select(x => x.tintStrength).ToArray());
            mapMaterial.SetFloatArray(baseTextureScales, layers.Select(x => x.textureScale).ToArray());
            
            mapMaterial.SetTexture(baseTextures, GenerateTextureArray(layers.Select(x => x.texture).ToArray()));

            mapMaterial.SetFloat(minHeight, heightMapSettings.MinHeight);
            mapMaterial.SetFloat(maxHeight, heightMapSettings.MaxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            var textureArray = new Texture2DArray(TextureSize, TextureSize, textures.Length, TextureFormat, true);
            for (var i = 0; i < textures.Length; i++) 
                textureArray.SetPixels(textures[i].GetPixels(), i);
            
            textureArray.Apply();
            return textureArray;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            ApplyToMaterial();
            base.OnValidate();
        }
#endif

        [System.Serializable]
        public class Layer
        {
            public Texture2D texture;
            public Color tint;
            [Range(0, 1)] public float tintStrength;
            /*[Range(0, 1)]*/ public float startHeight;
            [Range(0, 1)] public float blendStrength;
            public float textureScale;
        }
    }
}