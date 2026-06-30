using UnityEngine;

public class MidPointUpdater : MonoBehaviour
{
    [SerializeField] private Transform PlayerPos;
    private Transform target;

    private void FixedUpdate()
    {
        if (PlayerPos != null && target != null)
        {
            transform.position = (PlayerPos.position + target.position) / 2f;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
