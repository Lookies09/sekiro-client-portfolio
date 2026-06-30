using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoCell : MonoBehaviour
{
    // 셀 항목 출력 텍스트
    [SerializeField] private Image itemIMG;
    [SerializeField] private Text nameText;
    [SerializeField] private Text itemCount;
    [SerializeField] private Button button;

    private ItemInfoUi itemInfoUi;

    // 아이템 정보 저장
    private Item item;

    public event Action<Item> OnEquipItemChanged;

    private void Awake()
    {
        itemInfoUi = GameObject.FindFirstObjectByType<ItemInfoUi>();        
    }

    // Start is called before the first frame update
    public void Init(Item item, bool isForInfo)
    {
        this.item = item;

        nameText.text = item.ItemName;
        itemIMG.sprite = item.ItemIcon;
        itemCount.text = item.ItemCount.ToString();

        if (isForInfo)
        {
            button.onClick.AddListener(OnItemInfoCellClick);
        }
        else
        {
            button.onClick.AddListener(OnUseItemSelected);
        }

    }

    // 소지품 버튼 눌렀을때 정보 넘겨주기
    public void OnItemInfoCellClick()
    {
        itemInfoUi.ItemSelected(item);
    }

    // 장착 아이템 선택했을때 아이템 정보 넘겨주기
    public void OnUseItemSelected()
    {
        Debug.Log("아이템 장착");
        OnEquipItemChanged.Invoke(item);
        //inventoryUIManager.UseItemSelect(item);
    }

}
