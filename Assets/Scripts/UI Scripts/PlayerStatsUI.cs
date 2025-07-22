using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private GameObject staminaBarObject;

    private float originalStaminaWidth;
    private float originalHealthWidth;
    private float originalResistanceWidth;

    private void Start()
    {
        originalHealthWidth = healthBarObject.GetComponent<RectTransform>().rect.width;
        originalStaminaWidth = staminaBarObject.GetComponent<RectTransform>().rect.width;
        originalResistanceWidth = frostBarObject.GetComponent<RectTransform>().rect.width;

        PlayerStats.Instance.OnStaminaChanged += UpdateStaminaBar;
        PlayerStats.Instance.OnMaxHealthChanged += UpdateMaxHealthBar;
        PlayerStats.Instance.OnHealthChanged += UpdateHealthBar;
        PlayerStats.Instance.OnPoisonChanged += UpdatePoisonBar;
        PlayerStats.Instance.OnFrostChanged += UpdateFrostBar;
        PlayerStats.Instance.OnStunChanged += UpdateStunBar;

        PlayerStats.Instance.OnPlayerUpgradedSingle += PlayerUpgradeSingle;
    }

    private void PlayerUpgradeSingle(PlayerStats.UpgradeType type, float scale)
    {
        if (type == PlayerStats.UpgradeType.Stamina)
        {
            float width = staminaBarObject.GetComponent<RectTransform>().rect.width;
            width *= scale;

            staminaBarObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);            
        }
        else if (type == PlayerStats.UpgradeType.Health)
        {
            float width = healthBarObject.GetComponent<RectTransform>().rect.width;
            width *= scale;

            healthBarObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);  
        }
        else
        {
            float width = poisonBarObject.GetComponent<RectTransform>().rect.width;
            width *= scale;

            poisonBarObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);  
            stunBarObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);  
            frostBarObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);  
        }
    }

    private void OnDisable()
    {
        PlayerStats.Instance.OnStaminaChanged -= UpdateStaminaBar;
        PlayerStats.Instance.OnMaxHealthChanged -= UpdateMaxHealthBar;
        PlayerStats.Instance.OnHealthChanged -= UpdateHealthBar;
        PlayerStats.Instance.OnPoisonChanged -= UpdatePoisonBar;
        PlayerStats.Instance.OnFrostChanged -= UpdateFrostBar;
        PlayerStats.Instance.OnStunChanged -= UpdateStunBar;

        PlayerStats.Instance.OnPlayerUpgradedSingle -= PlayerUpgradeSingle;
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