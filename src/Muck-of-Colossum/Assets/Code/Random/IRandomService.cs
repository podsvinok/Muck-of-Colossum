namespace Code.Random
{
    public interface IRandomService
    {
        public int GetRandomSeed();
        float GetRandomFloatInRange(float minValue, float maxValue);
    }
}