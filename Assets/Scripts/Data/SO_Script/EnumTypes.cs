using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumTypes
{
    // 아이템 타입 (소모품)
    public enum ITEM_TYPE { CB }

    // 소모성 아이템 타입 (재화증가, 체력증가, 공격력증가)
    public enum CB_TYPE { GOLD_UP, HP_UP, DMG_UP }
}
