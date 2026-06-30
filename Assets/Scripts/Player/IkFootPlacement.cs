using UnityEngine;
using UnityEngine.Assertions.Must;

public class IkFootPlacement : MonoBehaviour
{
    #region Variables
    private Vector3 rightFootPos, leftFootPos, leftFootIkPos, rightFootIkPos;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPos_Y, lastRightFootPos_Y, lastLeftFootPos_Y;

    private Animator animator;

    [Header("Feet Grounder")]
    [SerializeField] private bool enableFeetIk = true;
    [Range(0, 2)][SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)][SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0, 1)][SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)][SerializeField] private float feetToIkPosSpeed = 0.5f;

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    #region FeetGrounding
    
    private void FixedUpdate()
    {
        if (enableFeetIk == false) { return; }
        if(!animator) { return; }

        AdjustFeetTarget(ref rightFootPos, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPos, HumanBodyBones.LeftFoot);

        // 오른발, 왼발 위치 처리기
        FeetPositionSolver(rightFootPos, ref rightFootIkPos, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPos, ref leftFootIkPos, ref leftFootIkRotation);
        

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFeetIk == false) { return; }
        if (!animator) { return; }

        MovePelvisHeight();

        // 오른발 처리
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);


        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPos, rightFootIkRotation, ref lastRightFootPos_Y);


        // 왼발 처리
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPos, leftFootIkRotation, ref lastLeftFootPos_Y);


    }
    #endregion


    #region FeetGroundingMethods

    void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPos_Y)
    {
        Vector3 targetIkPos = animator.GetIKPosition(foot);

        if (positionIkHolder != Vector3.zero)
        {
            targetIkPos = transform.InverseTransformPoint(targetIkPos);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPos_Y, positionIkHolder.y, feetToIkPosSpeed);
            targetIkPos.y += yVariable;

            lastFootPos_Y = yVariable;

            targetIkPos = transform.TransformPoint(targetIkPos);

            animator.SetIKRotation(foot, rotationIkHolder);
        }

        animator.SetIKPosition(foot, targetIkPos);

    }

    private void MovePelvisHeight()
    {
        if(rightFootIkPos == Vector3.zero || leftFootIkPos == Vector3.zero || lastPelvisPos_Y == 0)
        {
            lastPelvisPos_Y = animator.bodyPosition.y;
            return;
        }

        float lOff_SetPos = leftFootIkPos.y - transform.position.y;
        float rOff_setPos = rightFootIkPos.y - transform.position.y;

        float totalOffset = (lOff_SetPos < rOff_setPos) ? lOff_SetPos : rOff_setPos;

        Vector3 newPelvisPos = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPos.y = Mathf.Lerp(lastPelvisPos_Y, newPelvisPos.y, pelvisUpAndDownSpeed);

        animator.bodyPosition = newPelvisPos;

        lastPelvisPos_Y = animator.bodyPosition.y;
    }

    
    private void FeetPositionSolver(Vector3 fromSkyPos, ref Vector3 feetIkPoses, ref Quaternion feetIkRotations)
    {
        // 레이캐스트 핸들링 영역
        RaycastHit feetOutHit;

        if (showSolverDebug)
            Debug.DrawLine(fromSkyPos, fromSkyPos + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.red);

        if(Physics.Raycast(fromSkyPos, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            feetIkPoses = fromSkyPos;
            feetIkPoses.y = feetOutHit.point.y + pelvisOffset;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPoses = Vector3.zero;
    }


    private void AdjustFeetTarget(ref Vector3 feetPoses, HumanBodyBones foot)
    {
        feetPoses = animator.GetBoneTransform(foot).position;
        feetPoses.y = transform.position.y + heightFromGroundRaycast;
    }

    #endregion

    public void SetEnableFeetIk(bool isEnable)
    {
        enableFeetIk = isEnable;
    }


}
