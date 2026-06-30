using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    // 아이템 아이디
    [SerializeField] protected int itemId;
    // 아이템 이름
    [SerializeField] protected string itemName;
    //아이템 설명
    [SerializeField] protected string description;
    // 아이템 아이콘
    [SerializeField] protected Sprite itemIcon;
    // 아이템 가격
    [SerializeField] protected int itemPrice;
    // 아이템 갯수
    [SerializeField] protected int itemCount;

    [SerializeField] protected GameObject itemPrefab;

    public int ItemId { get => itemId; set => itemId = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public string Description { get => description; set => description = value; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public int ItemPrice { get => itemPrice; set => itemPrice = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }


    protected GameObject player;

    public Action<Item> OnItemChanged;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // 아이템 복제
    public Item Clone()
    {
        Item newItem = Instantiate(this);
        return newItem;
    }

    public virtual void Use()
    {
        if (ItemCount <= 0) return;
        ItemCount--;
        OnItemChanged?.Invoke(this);
    }
}
