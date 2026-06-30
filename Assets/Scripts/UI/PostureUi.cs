using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PostureUi : MonoBehaviour
{
    [Header("체간 전체 오브젝트")]
    [SerializeField] private GameObject postureBarObject;

    [Header("체간 좌우 이미지")]
    [SerializeField] private Image[] postureBarImgs;

    [Header("장식 좌우 이미지")]
    [SerializeField] private Image[] barDecoImgs;

    private void Start()
    {
        postureBarObject.SetActive(false);
    }

    public void SetPostureUiFillAnount(float currentPosture, float maxPosture)
    {
        float targetFill = currentPosture.Remap(0, maxPosture, 0, 1);

        for (int i =0; i < postureBarImgs.Length; i++)
        {
            postureBarImgs[i].fillAmount = targetFill;
            UpdateDecoImgsPos(postureBarImgs[i], barDecoImgs[i].rectTransform, i == 0);
        }

        if (Mathf.Approximately(targetFill, 0f) && postureBarObject.activeSelf)
        {
            postureBarObject.SetActive(false);
        }
        else if(targetFill > 0 && !postureBarObject.activeSelf)
        {
            postureBarObject.SetActive(true);
        }
    }

    private void UpdateDecoImgsPos(Image postureImg, RectTransform decoPos, bool isRight)
    {
        RectTransform mainRect = postureImg.rectTransform;

        float width = mainRect.rect.width;
        float fillPosX = width * postureImg.fillAmount;

        Vector3 pos = decoPos.localPosition;
        pos.x = isRight ? -fillPosX : fillPosX;
        decoPos.localPosition = pos;
    }
}
