using System;

namespace Code.Random
{
    public class RandomService : IRandomService
    {
        public int GetRandomSeed() => 
            (int)DateTime.Now.Ticks;

        public float GetRandomFloatInRange(float minValue, float maxValue) => 
            UnityEngine.Random.Range(minValue, maxValue);

        public int GetRandomIntInRange(int minValue, int maxValue) =>
            UnityEngine.Random.Range(minValue, maxValue);
    }
}
