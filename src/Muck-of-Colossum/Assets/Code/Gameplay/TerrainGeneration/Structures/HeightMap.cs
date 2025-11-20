namespace Code.Gameplay.TerrainGeneration.Structures
{
    public struct HeightMap
    {
        public readonly float[,] Values;
        public readonly float MinValue;
        public readonly float MaxValue;

        public HeightMap(float[,] values, float minValue, float maxValue)
        {
            this.Values = values;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }
}