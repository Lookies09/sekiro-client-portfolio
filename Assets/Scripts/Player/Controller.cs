using UnityEngine;


public abstract class Controller : ScriptableObject
{
    public PlayerMovement PlayerMovement { get; set; }

    public PlayerAttack PlayerAttack { get; set; }

    

    public abstract void Init();
    public abstract void OnCharacterUpdate();
    public abstract void OnCharacterFixedUpdate();
}
