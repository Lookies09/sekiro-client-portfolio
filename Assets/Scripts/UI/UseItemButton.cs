using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItemButton : MonoBehaviour
{
    public Item item {  get; private set; }

    // 버튼 번호
    [SerializeField] private int num;

    [SerializeField] private Image iconImg;

    [SerializeField] private Text itemCount;

    [SerializeField] private InventoryUiManager inventoryUiManager;
    

    // 버튼 번호 넘겨주기
    public void OnUseItemButtonClick()
    {
        inventoryUiManager.ActivePanel("Equipment");
        inventoryUiManager.SetCurrentUseItemButton(this);
    }

    // 버튼에 아이콘 업데이트
    public void SetUseItemButton(Item item)
    {
        if (this.item != null)
        {
            this.item.OnItemChanged -= UpdateItemInfo;
        }
        this.item = item;
        iconImg.sprite = this.item.ItemIcon;
        itemCount.text = this.item.ItemCount.ToString();
        this.item.OnItemChanged += UpdateItemInfo;
    }

    public void UpdateItemInfo(Item item)
    {
        iconImg.sprite = item.ItemIcon;
        itemCount.text = item.ItemCount.ToString();
    }

}
