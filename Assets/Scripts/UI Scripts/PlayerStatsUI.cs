using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image staminaBar;
    [SerializeField] private UnityEngine.UI.Image maxHealthBar;
    [SerializeField] private UnityEngine.UI.Image healthBar;
    [SerializeField] private UnityEngine.UI.Image poisonBar;
    [SerializeField] private UnityEngine.UI.Image frostBar;
    [SerializeField] private UnityEngine.UI.Image stunBar;

    [SerializeField] private GameObject stunBarObject;
    [SerializeField] private GameObject frostBarObject;
    [SerializeField] private GameObject poisonBarObject;

    private void Start()
    {
        PlayerStats.Instance.OnStaminaChanged += UpdateStaminaBar;
        PlayerStats.Instance.OnMaxHealthChanged += UpdateMaxHealthBar;
        PlayerStats.Instance.OnHealthChanged += UpdateHealthBar;
        PlayerStats.Instance.OnPoisonChanged += UpdatePoisonBar;
        PlayerStats.Instance.OnFrostChanged += UpdateFrostBar;
        PlayerStats.Instance.OnStunChanged += UpdateStunBar;
    }

    private void OnDisable()
    {
        PlayerStats.Instance.OnStaminaChanged -= UpdateStaminaBar;
        PlayerStats.Instance.OnMaxHealthChanged -= UpdateMaxHealthBar;
        PlayerStats.Instance.OnHealthChanged -= UpdateHealthBar;
        PlayerStats.Instance.OnPoisonChanged -= UpdatePoisonBar;
        PlayerStats.Instance.OnFrostChanged -= UpdateFrostBar;
        PlayerStats.Instance.OnStunChanged -= UpdateStunBar;
    }

    private void UpdateStaminaBar(float stamina)
    {
        staminaBar.fillAmount = stamina;
    }

    private void UpdateMaxHealthBar(float maxHealth)
    {
        maxHealthBar.fillAmount = maxHealth;
    }

    private void UpdateHealthBar(float health)
    {
        healthBar.fillAmount = health;
    }
    private void UpdatePoisonBar(float poison)
    {
        poisonBar.fillAmount = poison;
        poisonBarObject.SetActive(poison > 0);
    }
    private void UpdateFrostBar(float frost)
    {
        frostBar.fillAmount = frost;
        frostBarObject.SetActive(frost > 0);
    }
    private void UpdateStunBar(float stun)
    {
        stunBar.fillAmount = stun;
        stunBarObject.SetActive(stun > 0);
    }
}