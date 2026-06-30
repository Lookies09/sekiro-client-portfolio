using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPattern_SO : ScriptableObject
{
    public string attackName;
    public int[] damages;
    public string[] animationNames;
    public float[] animationTimes;

    public bool[] isReboundableAttack; // 리바운드 가능 공격 확인
    public bool[] isDefenseableAttack; // 방어가능 공격 확인

    public bool isPatternFin { get; set; } = false;

    public abstract void StopAttack(EnemyAnimationController animationController);
    // 공격 실행 메소드, 자식 클래스에서 구현
    public abstract void ExecuteAttack(EnemyAttackAbleState attackState, EnemyAnimationController animationController);
}

