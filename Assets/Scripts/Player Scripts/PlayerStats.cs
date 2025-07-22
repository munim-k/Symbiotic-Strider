using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class PlayerStats : MonoBehaviour
{
    // Singleton instance
    public static PlayerStats Instance { get; private set; }

    //Action for upgrade to send to UI with the scale of the thing
    public enum UpgradeType
    {
        Health,
        Stamina,
        Resistance,
    }
    public Action<UpgradeType, float> OnPlayerUpgradedSingle;

    //Events for UI to change the fill amounts
    public Action<float> OnStaminaChanged;
    public Action<float> OnMaxHealthChanged;
    public Action<float> OnHealthChanged;
    public Action<float> OnPoisonChanged;
    public Action<float> OnFrostChanged;
    public Action<float> OnStunChanged;

    public Action<bool> OnFrostProcced;
    public Action<bool> OnStunProcced;
    public Action<EnemyHealth> OnEnemyConsumed;

    public Action<float> OnPlayerUpgraded;

    [Header("Player Shader Settings")]
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Color frostColor = Color.cyan;
    [SerializeField] private Color poisonColor = Color.green;
    [SerializeField] private Color stunColor = Color.yellow;
    [SerializeField] private GameObject frostEffect;
    [SerializeField] private GameObject poisonEffect;
    [SerializeField] private GameObject stunEffect;

    [Header("Player Health and Stamina Settings")]
    [SerializeField] private float originalMaxHealth = 100f; // Maximum health of the player
    [SerializeField] private float originalMaxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 10f; // per second while moving
    [SerializeField] private float staminaRegenRate = 5f;  // per second while idle
    private float maxHealth;
    private float maxStamina;
    private float maxHealthAfterStamina = 100f;
    private float currentHealth; // Current health of the player
    private float currentStamina;

    [Header("Player Status Effects Settings\n Player Poison Settings")]
    [SerializeField] private float poisonDamagePerSecond = 1.5f; // Damage per second from poison
    [SerializeField] private float originalPoisonProcThreshhold = 50f;
    [SerializeField] private float poisonRegenMultiplier = 1f; // Multiplier for poison regeneration
    private float poisonProcThreshhold;
    private bool isPoisoned = false;
    private float currentPoison = 0f;

    [Header("Player Frost Settings")]
    [SerializeField] private float originalFrostProcThreshhold = 50f; // Threshold for frost effect to apply
    [SerializeField] private float frostRegenMultiplier = 2.5f; // Multiplier for frost regeneration
    private float frostProcThreshhold;
    private float currentFrost = 0f;
    private bool isFrosted = false;

    [Header("Player Stun Settings")]
    [SerializeField] private float originalStunProcThreshhold = 50f; // Threshold for stun effect to apply
    [SerializeField] private float stunRegenMultiplier = 5f; // Multiplier for stun regeneration
    private float stunProcThreshhold; // Threshold for stun effect to apply
    private bool isStunned = false;
    private float currentStun = 0f;
    private bool isMoving = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        maxHealth = originalMaxHealth;
        maxStamina = originalMaxStamina;
        poisonProcThreshhold = originalPoisonProcThreshhold;
        frostProcThreshhold = originalFrostProcThreshhold;
        stunProcThreshhold = originalStunProcThreshhold;
        
        currentStamina = maxStamina;
        currentHealth = maxHealthAfterStamina;

        frostEffect.SetActive(false);
        poisonEffect.SetActive(false);
        stunEffect.SetActive(false);

        Enemy.OnEnemyAttackedPlayer += damage =>
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealthAfterStamina);
        };

        PlayerMovement.Instance.OnMove += isMoving =>
        {
            this.isMoving = isMoving;
        };
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        //reset the material color
        playerMaterial.color = Color.white;
    }

    private void Update()
    {
        if (isMoving)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        maxHealthAfterStamina = Mathf.Lerp(0.75f, 1f, currentStamina / maxStamina) * maxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealthAfterStamina);

        OnStaminaChanged?.Invoke(currentStamina / maxStamina);
        OnMaxHealthChanged?.Invoke(Mathf.Lerp(0.25f, 0f, currentStamina / maxStamina));
        OnHealthChanged?.Invoke(currentHealth / maxHealthAfterStamina);

        HandleStatusEffects();
    }

    private void HandleStatusEffects()
    {
        if (!isPoisoned)
        {
            currentPoison -= poisonRegenMultiplier * Time.deltaTime;
        }
        else
        {
            // slowly reduce poison
            currentPoison -= poisonRegenMultiplier / 2 * Time.deltaTime;
            currentHealth -= poisonDamagePerSecond * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealthAfterStamina);

            if (currentPoison <= 0f)
            {
                isPoisoned = false;
                currentPoison = 0f;
                poisonEffect.SetActive(false);
                playerMaterial.color = Color.white;
            }
        }

        if (!isFrosted)
        {
            currentFrost -= frostRegenMultiplier * Time.deltaTime;
        }
        else
        {
            // slowly reduce frost
            currentFrost -= frostRegenMultiplier / 2 * Time.deltaTime;

            if (currentFrost <= 0f)
            {
                isFrosted = false;
                currentFrost = 0f;
                OnFrostProcced?.Invoke(false);
                frostEffect.SetActive(false);
                playerMaterial.color = Color.white;
            }
        }

        if (!isStunned)
        {
            currentStun -= stunRegenMultiplier * Time.deltaTime;
        }
        else
        {
            // slowly reduce stun
            currentStun -= stunRegenMultiplier * 2 * Time.deltaTime;

            if (currentStun <= 0f)
            {
                isStunned = false;
                currentStun = 0f;
                OnStunProcced?.Invoke(false);
                stunEffect.SetActive(false);
                playerMaterial.color = Color.white;
            }
        }

        currentPoison = Mathf.Max(currentPoison, 0f);
        currentFrost = Mathf.Max(currentFrost, 0f);
        currentStun = Mathf.Max(currentStun, 0f);

        OnPoisonChanged?.Invoke(currentPoison / poisonProcThreshhold);
        OnFrostChanged?.Invoke(currentFrost / frostProcThreshhold);
        OnStunChanged?.Invoke(currentStun / stunProcThreshhold);
    }

    public float GetStaminaRatio()
    {
        return Mathf.Clamp01(currentStamina / maxStamina);
    }
    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    private void OnTriggerEnter(Collider other)
    {
        ProjectileType projectileType;
        other.TryGetComponent(out projectileType);

        if (projectileType != null)
        {
            switch (projectileType.projectileType)
            {
                case ProjectileType.Projectile_Type.Frost:
                    //apply frost effect
                    if (!isFrosted)
                    {
                        currentFrost += projectileType.procBuildUp;
                        if (currentFrost >= frostProcThreshhold)
                        {
                            OnFrostProcced?.Invoke(true);
                            frostEffect.SetActive(true);
                            playerMaterial.color = frostColor;
                            isFrosted = true;
                        }
                    }
                    break;
                case ProjectileType.Projectile_Type.Poison:
                    //apply poison effect
                    if (!isPoisoned)
                    {
                        currentPoison += projectileType.procBuildUp;
                        if (currentPoison >= poisonProcThreshhold)
                        {
                            isPoisoned = true;
                            poisonEffect.SetActive(true);
                            playerMaterial.color = poisonColor;
                        }
                    }
                    break;
                case ProjectileType.Projectile_Type.Stun:
                    //apply stun effect
                    if (!isStunned)
                    {
                        currentStun += projectileType.procBuildUp;
                        if (currentStun >= stunProcThreshhold)
                        {
                            OnStunProcced?.Invoke(true);
                            isStunned = true;
                            stunEffect.SetActive(true);
                            playerMaterial.color = stunColor;
                        }
                    }
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.TryGetComponent(out EnemyHealth enemyHealth);

        if (enemyHealth == null || enemyHealth.GetCurrentHealth() == 0f)
        {
            return;
        }
        float minHealthRequiredToConsume = enemyHealth.GetMaxHealth() / 4;
        if (enemyHealth.GetCurrentHealth() <= minHealthRequiredToConsume)
        {
            OnEnemyConsumed?.Invoke(enemyHealth);

            UpgradePlayer(enemyHealth.GetCurrentScale() / 7.5f);
        }
    }

    private void UpgradePlayer(float scale)
    {
        transform.localScale += new Vector3(scale, scale, scale);

        float newScale = transform.localScale.x;
        OnPlayerUpgraded?.Invoke(newScale);

        maxHealth = originalMaxHealth * newScale;
        maxStamina = originalMaxStamina * newScale;
        frostProcThreshhold = originalFrostProcThreshhold * newScale;
        stunProcThreshhold = originalStunProcThreshhold * newScale;
        poisonProcThreshhold = originalPoisonProcThreshhold * newScale;

        OnPlayerUpgradedSingle?.Invoke(UpgradeType.Health, maxHealth / originalMaxHealth);
        OnPlayerUpgradedSingle?.Invoke(UpgradeType.Stamina, maxStamina / originalMaxStamina);
        OnPlayerUpgradedSingle?.Invoke(UpgradeType.Resistance, frostProcThreshhold / originalFrostProcThreshhold);
    }
}
