using UnityEngine;

[CreateAssetMenu(fileName = "DmgUpCandy", menuName = "Item/Consumable/DmgUpCandy")]
public class DmgUpCandy : Item, IConsumableItem
{
    [SerializeField] private int upValue;


    public void Consum()
    {
        itemCount--;
    }

    public override void Use()
    {
        base.Use();
        Debug.Log("공격력 증가");
    }
}
