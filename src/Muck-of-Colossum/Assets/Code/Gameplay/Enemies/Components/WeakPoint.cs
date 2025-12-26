using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 2f;
    private IHealth enemyHealth;

    public void Initialize(IHealth health)
    {
        enemyHealth = health;
    }

    public void OnHit(float baseDamage)
    {
        enemyHealth?.TakeDamage(baseDamage * damageMultiplier, transform.position);
    }
}