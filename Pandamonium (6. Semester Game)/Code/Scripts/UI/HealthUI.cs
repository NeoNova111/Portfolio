using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [System.Serializable]
    public struct HealthBarContainer
    {
        public Image healthImage;
        public Image differenceImage;
        public TextMeshProUGUI healthValueText;
        private int i;
    }

    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private HealthBarContainer health;
    private float currentHealthDiffernceTimer;
    private float currentHealthbarDrainTime;
    private float healthDrainStartAmount;
    [SerializeField] private HealthBarContainer shield;
    private float currentShieldDiffernceTimer;
    private float currentShieldDrainTime;
    private float shieldDrainStartAmount;
    [SerializeField] private float draingDelayTime = 0.5f;
    [SerializeField] private float healthBarDrainSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealthDiffernceTimer = 0f;
        currentShieldDiffernceTimer = 0f;
        currentHealthbarDrainTime = 0f;
        currentShieldDrainTime = 0f;
        health.healthValueText.text = (int)playerStats.CurrentHealth + " | " + (int)playerStats.healthStat.TotalValue;
        health.healthImage.fillAmount = playerStats.CurrentHealth / playerStats.healthStat.TotalValue;
        health.differenceImage.fillAmount = health.healthImage.fillAmount;

        shield.healthValueText.text = (int)playerStats.CurrentShield + " | " + (int)playerStats.shieldStat.TotalValue;
        shield.healthImage.fillAmount = playerStats.CurrentShield / playerStats.shieldStat.TotalValue;
        shield.differenceImage.fillAmount = shield.healthImage.fillAmount;
    }

    private void Update()
    {
        currentHealthDiffernceTimer -= Time.deltaTime;
        if(currentHealthDiffernceTimer <= 0 && health.differenceImage.fillAmount != health.healthImage.fillAmount)
        {
            health.differenceImage.fillAmount = Mathf.Lerp(healthDrainStartAmount, health.healthImage.fillAmount, 1 - (currentHealthbarDrainTime / healthBarDrainSpeed));
            currentHealthbarDrainTime = Mathf.Clamp(currentHealthbarDrainTime - Time.deltaTime, 0, healthBarDrainSpeed);
        }

        currentShieldDiffernceTimer -= Time.deltaTime;
        if (currentShieldDiffernceTimer <= 0 && shield.differenceImage.fillAmount != shield.healthImage.fillAmount)
        {
            shield.differenceImage.fillAmount = Mathf.Lerp(shieldDrainStartAmount, shield.healthImage.fillAmount, 1 - (currentShieldDrainTime / healthBarDrainSpeed));
            currentShieldDrainTime = Mathf.Clamp(currentShieldDrainTime - Time.deltaTime, 0, healthBarDrainSpeed);
        }
    }

    public void UpdateHealthBars()
    {
        health.healthImage.fillAmount = playerStats.CurrentHealth / playerStats.healthStat.TotalValue;
        health.healthValueText.text = (int)playerStats.CurrentHealth + " | " + (int)playerStats.healthStat.TotalValue;
        if (health.healthImage.fillAmount <= health.differenceImage.fillAmount)
        {
            currentHealthDiffernceTimer = draingDelayTime;
            currentHealthbarDrainTime = healthBarDrainSpeed;
            healthDrainStartAmount = health.differenceImage.fillAmount;
        }
        else
        {
            health.differenceImage.fillAmount = playerStats.CurrentHealth / playerStats.healthStat.TotalValue;
        }

        shield.healthImage.fillAmount = playerStats.CurrentShield / playerStats.shieldStat.TotalValue;
        shield.healthValueText.text = (int)playerStats.CurrentShield + " | " + (int)playerStats.shieldStat.TotalValue;
        if (shield.healthImage.fillAmount <= shield.differenceImage.fillAmount)
        {
            currentShieldDiffernceTimer = draingDelayTime;
            currentShieldDrainTime = healthBarDrainSpeed;
            shieldDrainStartAmount = shield.differenceImage.fillAmount;
        }
        else
        {
            shield.differenceImage.fillAmount = playerStats.CurrentShield / playerStats.shieldStat.TotalValue;
        }
    }
}
