using UnityEngine;

public interface IHealth
{

    // 체력 감소
    void TakeDamage(int amount, bool isHitFront, bool isDefenseable = true);

    // 체력 회복
    void Heal(int amount);

    // 생존 여부 확인
    bool IsAlive();

    void Die();
}
