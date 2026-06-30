using UnityEngine;
using UnityEngine.UI;

public class LockOnUi : MonoBehaviour
{
    private GameObject target;  // 락온된 적
    private RectTransform uiElement;
    [SerializeField] private GameObject lockOnImg;

    private void Awake()
    {
        uiElement = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // PlayerLockOnSystem의 OnFocusEnemyChanged 이벤트를 구독
        PlayerLockOnSystem playerLockOnSystem = FindFirstObjectByType<PlayerLockOnSystem>();
        if (playerLockOnSystem != null)
        {
            playerLockOnSystem.OnFocusEnemyChanged.AddListener(SetTarget);
        }
    }

    private void OnDisable()
    {
        // 구독 해제
        PlayerLockOnSystem playerLockOnSystem = FindFirstObjectByType<PlayerLockOnSystem>();
        if (playerLockOnSystem != null)
        {
            playerLockOnSystem.OnFocusEnemyChanged.RemoveListener(SetTarget);
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            UpdateUIPosition();
        }
    }

    private void UpdateUIPosition()
    {
        // 적의 월드 좌표를 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

        // UI 요소의 화면 좌표 업데이트
        uiElement.position = screenPos;
    }

    public void SetTarget(GameObject target)
    {
        if (target == null)
        {
            lockOnImg.SetActive(false);
        }
        else
        {
            lockOnImg.SetActive(true);
        }
            this.target = target;
    }
}
