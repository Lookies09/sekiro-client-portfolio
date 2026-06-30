using UnityEngine;

public class Health_LB : EnemyHealth, IHealth, IPosture
{
    private AttackState_LB attackState;

    protected override void Awake()
    {
        base.Awake();
        attackState = GetComponent<AttackState_LB>();
    }

    private void LateUpdate()
    {
        HealPosture(2f);
    }

    public void Die()
    {
        InvokeHpChanged(currentHealth, maxHealth);
        enemyController.TransactionToState((int)EnemyController_LB.STATE.BROKEN);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        InvokeHpChanged(currentHealth, maxHealth);
    }

    public void HealPosture(float amount)
    {
        currentPosture -= amount * Time.deltaTime;
        InvokePostureChanged(currentPosture, maxPosture);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void OnFullPosture()
    {
        InvokePostureChanged(currentPosture, maxPosture);
        enemyController.TransactionToState((int)EnemyController_LB.STATE.BROKEN);
    }

    public void TakeDamage(int amount, bool isHitFront, bool isDefenseable)
    {
        if (isDead) return;

        // 앞에서 맞고 방어 중인 경우
        if (isDefense)
        {
            if (TakePosture(amount)) return;
            enemyController.TransactionToState((int)EnemyController_LB.STATE.PARRY);
            return;
        }

        // 체력 감소
        currentHealth -= amount;
        InvokeHpChanged(currentHealth,maxHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }
        // 자세 무너짐 체크
        if (TakePosture(amount)) return;

        if (!attackState.isAttacking)
        {
            // 상태 전이
            enemyController.TransactionToState((int)EnemyController_LB.STATE.HIT);
        }
    }


    public bool TakePosture(int amount)
    {
        if (currentPosture == maxPosture) return true;

        currentPosture += amount;
        InvokePostureChanged(currentPosture, maxPosture);
        if (currentPosture >= maxPosture)
        {
            currentPosture = maxPosture;
            OnFullPosture();
            return true;
        }
        return false;

    }
}
