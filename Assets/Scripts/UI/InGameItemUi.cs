using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameItemUi : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemCountText;

    private Item currentItem;

    public void OnChangeItem(Item item)
    {
        itemIcon.sprite = item.ItemIcon;
        itemCountText.text = item.ItemCount.ToString();
        currentItem = item;
    }

    public void UseCurrentItem()
    {
        if (currentItem == null) return;
        currentItem.Use();
        OnChangeItem(currentItem);
    }
}
