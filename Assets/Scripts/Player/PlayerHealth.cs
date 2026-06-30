using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth, IPosture
{
    [SerializeField] private int maxHealth = 100;
    private float currentHealth;

    [SerializeField] private int maxPosture = 100;
    private float currentPosture;

    private PlayerDefense playerDefense;
    private PlayerAnimationController playerAnimationController;

    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnPostureChanged;

    private void Awake()
    {
        playerDefense = GetComponent<PlayerDefense>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentPosture = 0;
    }

    private void LateUpdate()
    {
        HealPosture(2f);
    }


    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void TakeDamage(int amount, bool isHitFront, bool isDefenseable)
    {
        if (isDefenseable && playerDefense.isOnDefense)
        {
            Debug.Log("¹æ¾î");
            playerAnimationController.CrossFadeAnimation("Defense_Hit");
            TakePosture(amount);
        }
        else
        {
            playerAnimationController.CrossFadeAnimation("Hit_F");
            currentHealth -= amount;
            TakePosture(amount);

            OnHpChanged?.Invoke(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        playerAnimationController.isAnimationBusy= true;
    }

    public bool TakePosture(int amount)
    {
        currentPosture += amount;
        OnPostureChanged.Invoke(currentPosture, maxPosture);
        if (currentPosture >= maxPosture)
        {
            currentPosture = maxPosture;
            OnFullPosture();
            return true;
        }
        return false;
    }

    public void HealPosture(float amount)
    {
        if(currentPosture <= 0 && currentHealth != 0)
        {
            currentPosture = 0;
            OnPostureChanged.Invoke(currentPosture, maxPosture);
            return;
        }

        if (currentHealth <= maxHealth / 2) amount /= 3;

        amount = playerDefense.isOnDefense ? amount * 1.6f : amount;

        currentPosture -= amount * Time.deltaTime;
        OnPostureChanged.Invoke(currentPosture, maxPosture);
    }

    public void OnFullPosture()
    {

    }
}
