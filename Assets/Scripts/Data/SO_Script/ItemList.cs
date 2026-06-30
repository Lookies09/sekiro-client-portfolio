using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 아이템 리스트
[CreateAssetMenu(fileName = "ItemList" , menuName = "Item/ItemList")]
public class ItemList : ScriptableObject
{
    // 아이템 리스트
    [SerializeField] private List<Item> list;

    public List<Item> List { get => list; set => list = value; }
}
