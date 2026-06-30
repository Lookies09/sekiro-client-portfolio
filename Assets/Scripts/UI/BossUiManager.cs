using UnityEngine;
using UnityEngine.UI;

public class BossUiManager : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    [SerializeField] private GameObject BossHealthUi;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Text bossNameShadowText;
    [SerializeField] private HealthUi healthUi;
    [SerializeField] private PostureUi postureUi;



    public void SetBossUi(bool isOn, string bossName, EnemyHealth newEnemyHealth)
    {
        if (isOn)
        {
            if (enemyHealth != null)
            {
                enemyHealth.OnHpChanged -= healthUi.SetHpUiFillAnount;
                enemyHealth.OnPostureChanged -= postureUi.SetPostureUiFillAnount;
            }

            enemyHealth = newEnemyHealth;

            BossHealthUi.SetActive(true);
            bossNameText.text = bossName;
            bossNameShadowText.text = bossName;

            enemyHealth.OnHpChanged += healthUi.SetHpUiFillAnount;
            enemyHealth.OnPostureChanged += postureUi.SetPostureUiFillAnount;
        }
        else
        {
            if (enemyHealth != null)
            {
                enemyHealth.OnHpChanged -= healthUi.SetHpUiFillAnount;
                enemyHealth.OnPostureChanged -= postureUi.SetPostureUiFillAnount;
                enemyHealth = null;
            }

            BossHealthUi.SetActive(false);
            bossNameText.text = "";
            bossNameShadowText.text = "";
        }
    }

}
