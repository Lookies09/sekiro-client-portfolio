using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAnimationController : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent agent;
    protected EnemyHealth enemyHealth;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    protected void LateUpdate()
    {
        UpdateState();
    }

    protected abstract void UpdateState();

    public abstract void CrossFadeAnimation(string name);
    public abstract bool CheckAnimationEnd(string name, float time, bool isTag);

}
