using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ParryVarious
{
    public Transform parryPos;
    public float detectRange;
    public LayerMask enemyLayer;
}

public class PlayerDefense : MonoBehaviour
{
    [SerializeField] private PlayerInputComponent inputComponent;
    [SerializeField] private PlayerAnimationController playerAnimationController;
    [SerializeField] private ParryVarious parryVarious;
    [SerializeField] private int parryDmg;
    public UnityEvent<Vector3> OnPlayParryEffect = new UnityEvent<Vector3>();   

    public static event Action<GameObject> OnParrySuccess;

    private bool isR_Parry;

    public float defenseTime { get; private set; } = 0f;
    public bool isOnDefense { get; private set; }

    [Header("방어 속도")]
    [Range(1, 10)]
    [SerializeField] private int defenseSpeed;

    private void Start()
    {
        isR_Parry = true;
    }

    private void Update()
    {
        SetDefenseTime();
        SetOnDefense();

        if (!isR_Parry && playerAnimationController.CheckAnimationEnd("Parry_L2R_Up", 0.8f, false))
        {
            isR_Parry = true;
            Debug.Log("패링 리셋");
        }
    }

    private void SetDefenseTime()
    {
        if (inputComponent.isDefense)
        {
            defenseTime += Time.deltaTime * defenseSpeed;
        }
        else
        {
            defenseTime -= Time.deltaTime * defenseSpeed;
        }

        defenseTime = Mathf.Clamp(defenseTime, 0f, 1f);
    }

    private void SetOnDefense()
    {
        if (defenseTime >= 0.5f && !isOnDefense)
        {
            Parry();
            isOnDefense = true;
        }
        else if (defenseTime < 0.5f && isOnDefense)
        {
            isOnDefense = false;

        }
    }

    private void Parry()
    {
        Vector3 center = transform.position;

        Collider[] colliders = Physics.OverlapSphere(parryVarious.parryPos.position, parryVarious.detectRange, parryVarious.enemyLayer);

        foreach (var hit in colliders)
        {
            var ParryAbleState = hit?.GetComponent<IParryableState>();
            if (ParryAbleState == null) continue;

            var posture = hit?.GetComponent<IPosture>();

            if (ParryAbleState.isAttacking && !playerAnimationController.isAnimationBusy)
            {
                if (isR_Parry)
                {
                    playerAnimationController.CrossFadeAnimation("Parry_R2L_Up");                    
                }
                else
                {
                    playerAnimationController.CrossFadeAnimation("Parry_L2R_Up");
                }
                isR_Parry = !isR_Parry;

                // 패링 성공 이벤트 호출, 대상 전달
                OnParrySuccess?.Invoke(hit.gameObject);

                OnPlayParryEffect.Invoke(parryVarious.parryPos.position);
                posture.TakePosture(parryDmg);
                break;
            }
        }

    }
}
