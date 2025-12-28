using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "HeightMapSettings", menuName = "TerrainGenerationSettings/HeightMapSettings")]
    public class HeightMapSettings : UpdatableData
    {
        public NoiseSettings noiseSettings;
        public float heightMultiplier;
        public AnimationCurve heightCurve;
        public float MinHeight =>
            heightMultiplier * heightCurve.Evaluate(0);
        public float MaxHeight =>
            heightMultiplier * heightCurve.Evaluate(1);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            noiseSettings.ValidateValues();
            base.OnValidate();
        }
#endif
    }
}