using UnityEngine;

public class AngleBasedDetect : MonoBehaviour, IDetector
{
    [SerializeField, Range(0f, 180f)] private float interactionAngle = 60f;
    [SerializeField, Range(-180f, 180f)] private float offsetAngle = 0f;
    [SerializeField, Min(0f)] private float distance = 10f;

    public bool FindTarget(GameObject player)
    {
        Vector3 forward = Quaternion.Euler(0f, offsetAngle, 0f) * transform.forward;
        Vector3 toPlayer = (player.transform.position - transform.position).normalized;

        float angle = Vector3.Angle(forward, toPlayer);

        return angle <= interactionAngle * 0.5f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 forward = Quaternion.Euler(0f, offsetAngle, 0f) * transform.forward;
        float debugRadius = 0.05f;

        // 기본 디버그 원
        Gizmos.DrawWireSphere(origin, debugRadius);

        // 아크 시각화
        int segments = 30; // 조절 가능: 많을수록 부드러움
        float startAngle = -interactionAngle;
        float endAngle = interactionAngle;

        Vector3 prevPoint = origin + Quaternion.Euler(0, startAngle, 0) * forward * distance;

        for (int i = 1; i <= segments; i++)
        {
            float lerpT = i / (float)segments;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, lerpT);
            Vector3 nextPoint = origin + Quaternion.Euler(0, currentAngle, 0) * forward * distance;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;

            // 포인트 마커 (선택 사항)
            Gizmos.DrawWireSphere(nextPoint, debugRadius * 0.5f);
        }

        // 양 끝선도 같이 그리기 (왼쪽/오른쪽 경계)
        Vector3 left = Quaternion.Euler(0f, interactionAngle, 0f) * forward;
        Vector3 right = Quaternion.Euler(0f, -interactionAngle, 0f) * forward;

        Gizmos.DrawLine(origin, origin + left * distance);
        Gizmos.DrawLine(origin, origin + right * distance);
    }

}
