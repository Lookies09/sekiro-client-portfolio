using UnityEngine;
using System;
using System.Collections;

public static class DangerEventManager
{
    public static event Action OnDangerAttack;

    public static void RaiseDangerEvent()
    {
        OnDangerAttack?.Invoke();
    }
}

public class DangerInfoUi : MonoBehaviour
{
    [SerializeField] private GameObject dangerUi;
    [SerializeField] private Transform uiPos;
    private Coroutine uiCoroutine;

    private void OnEnable()
    {
        DangerEventManager.OnDangerAttack += ShowDangerUI;
    }

    private void OnDisable()
    {
        DangerEventManager.OnDangerAttack -= ShowDangerUI;
    }

    private void LateUpdate()
    {
        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        // 적의 월드 좌표를 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(uiPos.position);

        // UI 요소의 화면 좌표 업데이트
        dangerUi.transform.position = screenPos;
    }


    private void ShowDangerUI()
    {
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
        }
        uiCoroutine = StartCoroutine(ShowUIRoutine(2f));
    }

    private IEnumerator ShowUIRoutine(float duration)
    {
        dangerUi.SetActive(true);
        yield return new WaitForSeconds(duration);
        dangerUi.SetActive(false);
    }
}

