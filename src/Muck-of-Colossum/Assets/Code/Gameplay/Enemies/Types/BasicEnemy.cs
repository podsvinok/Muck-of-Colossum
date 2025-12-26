using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void InitializeComponents()
    {
        ((EnemyHealth)Health).Initialize(config.health);
    }
}
