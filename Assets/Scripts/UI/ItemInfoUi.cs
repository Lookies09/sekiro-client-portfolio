using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUi : MonoBehaviour
{
    // 아이템 이름
    [SerializeField] private Text itemName;
    // 아이템 아이콘
    [SerializeField] private Image itemIcon;
    // 아이템 소지수
    [SerializeField] private Text itemCount;
    // 아이템 설명
    [SerializeField] private Text itemDescript;


    public void ItemSelected(Item item)
    {
        itemName.text = item.ItemName;
        itemIcon.sprite = item.ItemIcon;
        itemCount.text = item.ItemCount.ToString();
        itemDescript.text = item.Description;
    }
}
