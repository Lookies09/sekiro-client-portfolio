using UnityEngine;

public interface IPosture
{
    // 체간 채우기
    bool TakePosture(int amount);

    // 체간 회복
    void HealPosture(float amount);
    
    // 체간 맥스 일때
    void OnFullPosture();
}
