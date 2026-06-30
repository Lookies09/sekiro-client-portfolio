using UnityEngine;

public interface IParryableState
{
    bool isAttacking { get; }

    void Rebound();
}
