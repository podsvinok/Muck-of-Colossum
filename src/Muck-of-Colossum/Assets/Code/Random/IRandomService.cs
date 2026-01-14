namespace Code.Random
{
    public interface IRandomService
    {
        int GetRandomSeed();
        float GetRandomFloatInRange(float minValue, float maxValue);
        int GetRandomIntInRange(int minValue, int maxValue);
    }
}