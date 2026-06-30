using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.AI;


// 적군 컨트롤러 부모
public abstract class EnemyController : MonoBehaviour
{
    // 적의 현재 동작중인 상태 컴포넌트
    protected EnemyState currentState;

    [SerializeField] protected GameObject targetPos;

    // 적의 모든 상태 컴포넌트들
    [SerializeField] protected EnemyState[] enemyStates;

    protected NavMeshAgent agent;

    // 플레이어 참조
    public GameObject player { get; private set; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        currentState = enemyStates[0];

        // 대기 상태로 시작
        TransactionToState(0);
    }

    // Update is called once per frame
    protected void Update()
    {
        // * 현재 설정된 상태의 기능을 동작!
        currentState?.UpdateState();
    }

    // * 상태 전환 메소드
    public void TransactionToState(int state)
    {
        Debug.Log($"적군 상태 전환 : {state}");

        currentState?.ExitState(); // 이전 상태 정리
        currentState = enemyStates[state]; // 상태 전환 처리
        currentState.EnterState(state); // 새로운 상태 전이
    }

    // 플레이어와 적군 간의 거리 측정
    public float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public GameObject GetLockonPos()
    {
        return targetPos;
    }

    public abstract bool IsExecuteAbleState();
}
