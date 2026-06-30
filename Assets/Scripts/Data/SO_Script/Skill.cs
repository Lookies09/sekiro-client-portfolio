using UnityEngine;

public class Skill : ScriptableObject
{
    // 스킬 아이디
    [SerializeField] protected int skillId;

    // 스킬 이름
    [SerializeField] protected string skillName;

    //스킬 설명
    [SerializeField] protected string description;

    // 스킬 아이콘
    [SerializeField] protected Sprite Icon;

    // 소비 카타시로
    [SerializeField] protected int skillCost;

    // 스킬 데미지
    [SerializeField] protected int Dmg;

    // 애니메이션 이름

    protected GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
