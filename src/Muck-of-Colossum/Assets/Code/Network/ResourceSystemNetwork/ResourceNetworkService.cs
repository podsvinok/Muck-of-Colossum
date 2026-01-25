using Code.Gameplay.Items.Factory;
using Code.Gameplay.ResourceSystem;
using FishNet.Object;
using Code.Gameplay.ResourceSystem.Factory;
using Code.Random;
using UnityEngine;
using Zenject;

public class ResourceNetworkService : NetworkBehaviour
{
    private ResourcePool resourcePool;
    private IItemFactory itemFactory;
    private IRandomService randomService;

    [Inject]
    public void Construct(
        ResourcePool resourcePool,
        IItemFactory itemFactory,
        IRandomService randomService)
    {
        this.resourcePool = resourcePool;
        this.itemFactory = itemFactory;
        this.randomService = randomService;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HitResource(int id) => 
        HitResourceRPC(id);

    [ServerRpc(RequireOwnership = false)]
    public void DestroyResource(int id) => 
        DestroyResourceRPC(id);

    [ObserversRpc]
    private void HitResourceRPC(int id) => 
        resourcePool.Get(id).GetHitNetwork();

    [ObserversRpc]
    private void DestroyResourceRPC(int id)
    {
        var resource = resourcePool.Get(id);
        resource.BeDestroyedNetwork();
        SpawnLoot(resource);
    }

    private void SpawnLoot(Resource resource)
    {
        foreach (var drop in resource.resourcePreset.drop)
        {
            if (randomService.GetRandomFloatInRange(0, 0.99f) < drop.dropChance)
            {
                var amount = randomService.GetRandomIntInRange(drop.minAmount, drop.maxAmount);
                var position = resource.transform.position;
                var newPosition = new Vector3(position.x, position.y + 2, position.z);
                itemFactory.SpawnItem(drop.item.prefab, newPosition, amount);
            }
        }
    }
}