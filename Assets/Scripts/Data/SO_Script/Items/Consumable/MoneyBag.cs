using UnityEngine;

[CreateAssetMenu(fileName = "MoneyBag", menuName = "Item/Consumable/MoneyBag")]
public class MoneyBag : Item, IConsumableItem
{
    [SerializeField] private int upValue;

    public void Consum()
    {
        itemCount--;
    }

    public override void Use()
    {
        base.Use();
        Debug.Log("재화 추가");
    }
}
