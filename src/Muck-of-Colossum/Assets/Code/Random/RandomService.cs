using System;
using Zenject;
using Random = UnityEngine.Random;

public class RandomService : IRandomService
{
    public int GetRandomSeed()
    {
        return (int)DateTime.Now.Ticks;
    }
}
