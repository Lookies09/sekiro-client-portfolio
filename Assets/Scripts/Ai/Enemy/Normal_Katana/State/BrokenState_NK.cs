using Kinemation.MotionWarping.Runtime.Core;
using Kinemation.MotionWarping.Runtime.Utility;
using UnityEngine;

[System.Serializable]
public class WarpInteractionData
{
    [Range(0f, 180f)] public float interactionAngle;
    [Range(-180f, 180f)] public float offsetAngle;
    [Min(0f)] public float distance;
    public string targetAnimName;
    public MotionWarpingAsset motionWarpingAsset;
}

public class BrokenState_NK : EnemyState, IWarpPointProvider
{
    [SerializeField] private WarpInteractionData[] interactions;

    public override void EnterState(int state)
    {
        navMeshAgent.isStopped = true;
        animationController.CrossFadeAnimation("DefenseL_Broken");
    }    

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        
    }

    public WarpInteractionResult Interact(GameObject instigator)
    {
        var result = new WarpInteractionResult
        {
            success = false,
            points = null,
            asset = null
        };

        if (!controller.IsExecuteAbleState()) return result;

        if (instigator == null) return result;

        foreach (var data in interactions)
        {
            if (data.motionWarpingAsset == null) continue;

            float distanceToTarget = Vector3.Distance(instigator.transform.position, transform.position);
            if (distanceToTarget > data.distance) continue;

            Vector3 forward = Quaternion.Euler(0f, data.offsetAngle, 0f) * transform.forward;
            Vector3 toInstigator = (instigator.transform.position - transform.position).normalized;

            float angle = Vector3.Angle(forward, toInstigator);
            if (angle > data.interactionAngle) continue;

            result.asset = data.motionWarpingAsset;
            result.points = new[]
            {
                    new WarpPoint
                    {
                        transform = this.transform,
                        position = Vector3.zero,
                        rotation = Quaternion.identity
                    }
                };
            result.success = true;

            if (animationController != null)
            {
                animationController.CrossFadeAnimation(data.targetAnimName);
                controller.TransactionToState((int)EnemyController_NK.STATE.DEAD);
            }

            return result;
        }

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactions == null || interactions.Length == 0) return;

        Vector3 origin = transform.position;

        foreach (var data in interactions)
        {
            if (data == null) continue;

            Vector3 forward = Quaternion.Euler(0f, data.offsetAngle, 0f) * transform.forward;
            Vector3 left = Quaternion.Euler(0f, data.interactionAngle, 0f) * forward;
            Vector3 right = Quaternion.Euler(0f, -data.interactionAngle, 0f) * forward;

            // Draw vision lines
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + left * data.distance);
            Gizmos.DrawLine(origin, origin + right * data.distance);

            // Draw arc
            int segments = 20;
            Vector3 prevPoint = origin + (Quaternion.Euler(0, -data.interactionAngle, 0) * forward) * data.distance;
            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Lerp(-data.interactionAngle, data.interactionAngle, i / (float)segments);
                Vector3 point = origin + (Quaternion.Euler(0, angle, 0) * forward) * data.distance;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }

            // Draw center and forward marker
            Gizmos.DrawWireSphere(origin, 0.05f);
            Gizmos.DrawWireSphere(origin + forward * data.distance, 0.05f);
        }
    }
}
