using System.Collections.Generic;
using UnityEngine;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] private InventoryUiManager inventoryUiManager;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private HealthUi healthUi;
    [SerializeField] private PostureUi postureUi;

    [SerializeField] private InGameItemUi inGameItemUi;

    private List<Item> equipedItemList = new List<Item>();
    private int currentItemIndex = 0;

    private void OnEnable()
    {
        inventoryUiManager.OnEquipItemListChanged += UpdateEquippedItemList;
        playerHealth.OnHpChanged += healthUi.SetHpUiFillAnount;
        playerHealth.OnPostureChanged += postureUi.SetPostureUiFillAnount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseCurrentItem();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeItem(1); // 다음 아이템
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeItem(-1); // 이전 아이템
        }
    }

    private void UseCurrentItem()
    {
        if (equipedItemList.Count == 0) return;

        inGameItemUi.UseCurrentItem();
    }

    private void ChangeItem(int direction)
    {
        if (equipedItemList.Count == 0) return;

        currentItemIndex = (currentItemIndex + direction + equipedItemList.Count) % equipedItemList.Count;
        inGameItemUi.OnChangeItem(equipedItemList[currentItemIndex]);
    }

    public void UpdateEquippedItemList(List<Item> newEquippedItems)
    {
        equipedItemList = newEquippedItems;
        currentItemIndex = Mathf.Clamp(currentItemIndex, 0, equipedItemList.Count - 1);

        if (equipedItemList.Count > 0)
        {
            inGameItemUi.OnChangeItem(equipedItemList[currentItemIndex]);
        }
    }
}
