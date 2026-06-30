using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState : MonoBehaviour
{
    protected IDetector detector;
    protected EnemyController controller;
    protected NavMeshAgent navMeshAgent;
    protected EnemyAnimationController animationController;
    protected EnemyEffectController effectController;
    protected EnemyHealth enemyHealth;

    public virtual void Awake()
    {
        effectController = FindFirstObjectByType<EnemyEffectController>();
        controller = GetComponent<EnemyController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        detector = GetComponent<IDetector>();
        animationController= GetComponent<EnemyAnimationController>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public abstract void EnterState(int state);

    public abstract void UpdateState();

    public abstract void ExitState();
}
