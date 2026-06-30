using UnityEngine;

[CreateAssetMenu(fileName = "HpPosion", menuName = "Item/Consumable/HpPosion")]
public class HpPosion : Item, IConsumableItem
{
    [SerializeField] private int upValue;
        
    public override void Use()
    {
        base.Use();
        Debug.Log("È¸º¹");
        player.GetComponent<PlayerHealth>().Heal(upValue);
    }

    public void Consum()
    {
        itemCount--;
    }

    
}
