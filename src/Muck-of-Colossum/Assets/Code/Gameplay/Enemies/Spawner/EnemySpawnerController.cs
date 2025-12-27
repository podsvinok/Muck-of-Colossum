using FishNet.Object;
using UnityEngine;
using Zenject;

public class EnemySpawnerController : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private IEnemySpawner spawner;

    [Inject]
    public void Construct(IEnemySpawner spawner)
    {
        this.spawner = spawner;
    }

    [ContextMenu("Spawn")]
    public void Spawn() => spawner.StartSpawning(spawnPoint);
}
