using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemInfo
{
    // 아이템 아이디
    public int itemId;

    // 갯수
    public int count;
}

public class ItemBag : MonoBehaviour
{
    // UI 위치
    [SerializeField] private Transform uiPos;

    // 아이템 상호작용 Ui 이미지
    [SerializeField] private Image objectUI_img;

    // 아이템 상호작용 Ui 텍스트
    [SerializeField] private Text objectUI_Text;

    // 상호작용 UI 전체
    [SerializeField] private GameObject info_UI_all;

    // 캔버스 렉트 트렌스폼
    [SerializeField] private RectTransform canvasRectTransform;

    // 플레이어 접촉 여부
    private bool isContact;

    // 인벤토리 시스템
    private InventoryManger inventoryManger;


    // 아이템 정보
    [SerializeField] private ItemInfo itemInfo;

    //아이템 id
    //[SerializeField] private int idNum;

    public ItemInfo ItemInfo { get => itemInfo; set => itemInfo = value; }

    void Start()
    {
        inventoryManger = FindFirstObjectByType<InventoryManger>();
    }


    private void Update()
    {
        if (isContact)
        {
            //objectUI_Text.text = "아이템을 줍는다";


            //info_UI_all.SetActive(true);

            //Vector3 targetPosition = Camera.main.WorldToViewportPoint(uiPos.position);

            //// 화면 좌표를 Canvas 영역으로 변환
            //Vector2 canvasPosition = new Vector2(
            //    ((targetPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
            //    ((targetPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f)));

            //objectUI_img.rectTransform.anchoredPosition = canvasPosition;

            // E키를 누르면
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 획득한 아이템 정보를 인벤토리 시스템에 추가
                bool invenAdded = inventoryManger.AddItem(ItemInfo);

                // 인벤토리 아이템 추가가 성공했다면
                if (invenAdded)
                {
                    isContact = false;

                    //info_UI_all.SetActive(false);

                    // 획득한 아이템 삭제
                    Destroy(gameObject);
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isContact = false;

            //info_UI_all.SetActive(false);
        }
    }
}
