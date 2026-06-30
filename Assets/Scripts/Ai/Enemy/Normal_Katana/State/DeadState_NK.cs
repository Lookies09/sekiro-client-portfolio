using UnityEngine;

public class DeadState_NK : EnemyState
{
    [SerializeField] private CapsuleCollider colider;    

    public override void EnterState(int state)
    {
        //enemyHealth.GetComponent<IHealth>().Die();

        navMeshAgent.isStopped = true;
        Debug.Log("»ç¸Á");
        enemyHealth.IsDead();       


        colider.enabled = false;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }    
}
