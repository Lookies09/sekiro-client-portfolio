using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    protected bool isDead = false;

    public bool isDefense { get; private set; } = false;

    [SerializeField] protected int maxHealth = 100;
    protected float currentHealth;

    [SerializeField] protected int maxPosture = 100;
    protected float currentPosture;

    protected EnemyAnimationController animationController;
    protected EnemyController enemyController;

    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnPostureChanged;

    protected virtual void Awake()
    {
        animationController = GetComponent<EnemyAnimationController>();
        enemyController = GetComponent<EnemyController>();
    }

    protected void Start()
    {
        currentHealth = maxHealth;
    }
    protected void InvokePostureChanged(float current, float max)
    {
        OnPostureChanged?.Invoke(current, max);
    }

    protected void InvokeHpChanged(float current, float max)
    {
        OnHpChanged?.Invoke(current, max);
    }    

    public void SetDefense(bool isDefense)
    {
        this.isDefense = isDefense;
    }

    public void IsDead()
    {
        isDead = true;
        currentHealth = 0;
        InvokeHpChanged(currentHealth, maxHealth);
    }
}
