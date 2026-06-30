using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLockOnSystem : MonoBehaviour
{
    #region 타겟 변환 옵저버 이벤트

    // 이벤트를 통해 FocusEnemy가 변경될 때 UI에 알림
    public UnityEvent<GameObject> OnFocusEnemyChanged = new UnityEvent<GameObject>();

    public GameObject FocusEnemy
    {
        get => focusEnemy;
        set
        {
            // FocusEnemy가 바뀔 때마다 이벤트 발생
            focusEnemy = value;
            OnFocusEnemyChanged.Invoke(focusEnemy);  // FocusEnemy가 변경되었음을 알림
        }
    }
    #endregion

    // 프리 카메라
    [SerializeField] private GameObject freeLookCam;

    // 락온 카메라
    [SerializeField] private GameObject lockOnCam;

    // 오버랩 생성 위치
    [SerializeField] private Transform overlapTransform;

    // 오버랩 범위
    [Range(6.0f, 10.0f)]
    [SerializeField] private float overlapRadius = 9;

    //최대 락온 유지 거리
    [Range(20f, 40f)]
    [SerializeField] private float maxDistanceToEnemy;

    // 충돌 가능 레이어 설정
    [SerializeField] private LayerMask overlapLayer;

    // 타겟과 중간지점 만들어주는 오브젝트
    [SerializeField] private MidPointUpdater pointUpdater;

    private Collider[] detectedEnemis;

    // 포커싱 적군
    public GameObject focusEnemy { get; private set; } = null;

    private void Update()
    {
        LockOnOffSystem();
    }

    public void LockOnOffSystem()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (!FocusEnemy)
            {
                LockOn();
            }
            else
            {
                FocusEnemy = null;
                detectedEnemis = null;
                pointUpdater.SetTarget(null);
                SwitchCam(false);
            }
        }

        LockOff();
    }

    public void LockOn()
    {
        Collider[] overlapColliders = Physics.OverlapSphere(overlapTransform.position, overlapRadius, overlapLayer);

        if (overlapColliders == null) return;

        float closestDistance = Mathf.Infinity;

        Collider closestEnemy = null;

        // 오버랩 충돌이 일어난 모든 게임오브젝트들 중에
        foreach (Collider collider in overlapColliders)
        {
            float distance = Vector3.Distance(gameObject.transform.position, collider.bounds.center);

            // 가장 가까운 적군을 찾기
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = collider;
            }
        }

        if (closestEnemy != null)
        {
            FocusEnemy = closestEnemy.GetComponent<EnemyController>().GetLockonPos();
            pointUpdater.SetTarget(FocusEnemy.transform);
            detectedEnemis = overlapColliders;
            SwitchCam(true);
        }
    }

    public void LockOff()
    {
        if (FocusEnemy == null) return;
        float distance = Vector3.Distance(gameObject.transform.position, FocusEnemy.transform.position);

        if (distance > maxDistanceToEnemy)
        {
            FocusEnemy = null;
            pointUpdater.SetTarget(null);
            detectedEnemis = null;
            SwitchCam(false);

            return;
        }
    }

    public void SwitchCam(bool isLockOn)
    {
        if (isLockOn)
        {
            lockOnCam.GetComponent<CinemachineCamera>().ForceCameraPosition(freeLookCam.transform.position, freeLookCam.transform.rotation);
            lockOnCam.SetActive(true);
            freeLookCam.SetActive(false);            
        }
        else
        {
            freeLookCam.GetComponent<CinemachineCamera>().ForceCameraPosition(lockOnCam.transform.position, lockOnCam.transform.rotation);
            freeLookCam.SetActive(true);
            lockOnCam.SetActive(false);
        }
    }
}
