using UnityEngine;

public class PointEnemySpawner : IEnemySpawner
{
    private IEnemyFactory factory;

    public PointEnemySpawner(IEnemyFactory factory)
    {
        this.factory = factory;
        
    }
    
    public void StartSpawning(Transform spawnPostion)
    {
        factory.CreateEnemy(spawnPostion);
    }

    public void StopSpawning()
    {
        
    }
}