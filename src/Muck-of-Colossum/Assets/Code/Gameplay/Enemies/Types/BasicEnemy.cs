using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void InitializeComponents()
    {
        ((EnemyHealth)Health).Init(config.health);
    }
}
