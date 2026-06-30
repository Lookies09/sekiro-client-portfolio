using UnityEngine;
using UnityEngine.UI;


public class EnemyUiManager : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    [SerializeField] private RectTransform enemyUi;
    [SerializeField] private Transform enemyUiTransform;
    [SerializeField] private HealthUi healthUi;
    [SerializeField] private PostureUi postureUi;

    [Header("UI 크기 조절 설정")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        enemyHealth.OnHpChanged += healthUi.SetHpUiFillAnount;
        enemyHealth.OnPostureChanged += postureUi.SetPostureUiFillAnount;
    }

    private void LateUpdate()
    {
        UpdateUIPosition();
        UpdateUIScale();
    }

    private void UpdateUIPosition()
    {
        // 적의 월드 좌표를 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyUiTransform.position);

        // UI 요소의 화면 좌표 업데이트
        enemyUi.position = screenPos;
    }


    private void UpdateUIScale()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, enemyUiTransform.position);

        // distance를 [minDistance, maxDistance] 범위로 클램핑하고 0~1로 정규화
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance); // 거리가 가까울수록 scale이 커지도록 역방향
        float scale = Mathf.Lerp(minScale, maxScale, t);

        enemyUi.localScale = Vector3.one * scale;
    }

    public void SetActiveEnemyUi(bool isActive)
    {
        enemyUi.gameObject.SetActive(isActive);
    }

}
