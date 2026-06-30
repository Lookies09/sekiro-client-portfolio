using UnityEngine;
using UnityEngine.UI;

public class HealthUi : MonoBehaviour
{
    [Header("체력 UI 감소 속도")]
    [SerializeField] private float decreaseSpeed;
    [Header("체력 UI 감소 딜레이 시간")]
    [SerializeField] private float decreaseDelayTime;
    [Header("메인 체력바 이미지")]
    [SerializeField] private Image mainHpImg;
    [SerializeField] private Image inventoryHpImg;
    [Header("서브 체력바 이미지")]
    [SerializeField] private Image subHpImg;

    private float targetFill;
    private float delayTimer;
    private bool isHpChanged = false;

    private void Awake()
    {
        mainHpImg.fillAmount = 1;
        subHpImg.fillAmount = 1;
    }

    private void LateUpdate()
    {
        SetHpUi();
    }

    public void SetHpUiFillAnount(float currentHp, float maxHp)
    {
        targetFill = currentHp.Remap(0, maxHp, 0, 1);
        mainHpImg.fillAmount = targetFill;
        //inventoryHpImg.fillAmount = targetFill;

        // 딜레이 시작
        if (!Mathf.Approximately(subHpImg.fillAmount, targetFill))
        {
            isHpChanged = true;
            delayTimer = 0f;
        }

    }

    public void SetHpUi()
    {
        if (!isHpChanged) return;

        delayTimer += Time.deltaTime;

        if (delayTimer >= decreaseDelayTime)
        {
            subHpImg.fillAmount = Mathf.MoveTowards(subHpImg.fillAmount, targetFill, decreaseSpeed * Time.deltaTime);

            // 감소 완료 시 종료
            if (Mathf.Approximately(subHpImg.fillAmount, targetFill))
            {
                isHpChanged = false;
            }
        }
    }

}
