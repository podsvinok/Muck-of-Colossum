using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Ritual Grounds/Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public EnemyType enemyType;
    public float health;
    public float moveSpeed;
    public float detectionRadius;
    public float attackRange;
    public float attackDamage;
}
