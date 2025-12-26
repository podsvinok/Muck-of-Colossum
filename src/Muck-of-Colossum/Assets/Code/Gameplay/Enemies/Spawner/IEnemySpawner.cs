using UnityEngine;

public interface IEnemySpawner
{
    public void StartSpawning(Transform spawnPosition);
    public void StopSpawning();
}