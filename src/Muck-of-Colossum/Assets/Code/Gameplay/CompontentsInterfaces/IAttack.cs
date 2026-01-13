
using UnityEngine;

public interface IAttack
{
    void Init(float baseDamage);
    void Attack();
    float CalculateDamage();

    void ApplyDamage(Vector3 hitPoint, IHealth health, float damage);

}
