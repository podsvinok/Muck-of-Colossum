using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class MeleeGoblinFactory: IEnemyFactory
{
    private NetworkManager networkManager;
    
    public MeleeGoblinFactory(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }
    public Enemy CreateEnemy(Transform position)
    {
        var prefab = Resources.Load<GameObject>("Characters/Enemies/MeleeGoblinEnemy");
        var gameObject = Object.Instantiate(prefab, position.position, Quaternion.identity);
        
        var enemyComponent = gameObject.GetComponent<Enemy>();
        

        var networkObject = gameObject.GetComponent<NetworkObject>();
        networkManager.ServerManager.Spawn(networkObject);
        enemyComponent.Init();
        return enemyComponent;
    }
}