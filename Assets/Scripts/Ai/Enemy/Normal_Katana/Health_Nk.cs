using System;
using UnityEngine;
using UnityEngine.Events;

public class Health_Nk : EnemyHealth, IHealth, IPosture
{

    private void LateUpdate()
    {
        HealPosture(2f);
    }

    public void Die()
    {
        currentHealth = 0;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void HealPosture(float amount)
    {
        if (currentPosture <= 0 && currentHealth != 0)
        {
            currentPosture = 0;
            InvokePostureChanged(currentPosture, maxPosture);
            return;
        }

        if (currentHealth <= maxHealth / 2) amount /= 3;

        currentPosture -= amount * Time.deltaTime;
        InvokePostureChanged(currentPosture, maxPosture);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void OnFullPosture()
    {
        enemyController.TransactionToState((int)EnemyController_NK.STATE.BROKEN);
    }

    public void TakeDamage(int amount, bool isHitFront, bool isDefenseable)
    {
        if (isDead) return;

        // 앞에서 맞고 방어 중인 경우
        if (isHitFront && isDefense)
        {
            if (TakePosture(amount)) return;
            animationController.CrossFadeAnimation("DefenseL_Hit01");
            return;
        }

        // 체력 감소
        currentHealth -= amount;
        InvokeHpChanged(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnFullPosture();
            return;
        }

        // 자세 무너짐 체크
        if (TakePosture(amount)) return;

        // 맞은 방향에 따른 애니메이션 선택
        string hitAnimation = isHitFront ? "Hit_F" : "Hit_B";
        animationController.CrossFadeAnimation(hitAnimation);

        // 상태 전이
        enemyController.TransactionToState((int)EnemyController_NK.STATE.HIT);
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
