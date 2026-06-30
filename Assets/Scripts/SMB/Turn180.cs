using UnityEngine;

public class Turn180 : AnimationBusy
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 현재 애니메이션 시간 계산
        float current_Time = stateInfo.normalizedTime * stateInfo.length;

        // 애니메이션 시간이 50% 이상일 때 동작
        if (current_Time > 0.5f && animator.IsInTransition(0))
        {
            // 루트 회전 변경
            Quaternion currentRot = animator.rootRotation;
            Quaternion targetRot = animator.gameObject.GetComponent<PlayerMovement>().Desired_rotation;

            float smoothSpeed = 20f;
            animator.rootRotation = Quaternion.Slerp(currentRot, targetRot, smoothSpeed * Time.deltaTime);
        }
    }
}
