using System;
using Zenject;
using Random = UnityEngine.Random;

public class RandomService : IRandomService
{
    public int GetRandomSeed() => 
        (int)DateTime.Now.Ticks;

    public float GetRandomFloatInRange(float minValue, float maxValue) => 
        UnityEngine.Random.Range(minValue, maxValue);
}
