using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Action OnEnemyDeath;
    public static Action<Enemy> OnEnemyDied;
    public static Action<float> OnEnemySupported;
    [SerializeField] private EnemyAnimation enemyAnimation;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 100f;
    [SerializeField] private float healthIncreaseOnSupport = 10f;

    [SerializeField] private Image healthImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private GameObject shieldObject;

    private float currentHealth;
    private float currentShield;

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = 0f;

        MinionAI.OnEnemyAttacked += MinionAI_OnEnemyAttacked;
        enemyAnimation.OnSupportComplete += EnemyAnimation_OnSupportComplete;
        OnEnemySupported += Enemy_OnEnemySupported;
        PlayerStats.Instance.OnEnemyConsumed += EnemyConsumed;

        HandleUI();
    }

    private void EnemyConsumed(EnemyHealth obj)
    {
        if (obj != this)
        {
            return;
        }

        currentHealth = 0f;
        currentShield = 0f;

        OnEnemyDeath?.Invoke();
        OnEnemyDied?.Invoke(gameObject.GetComponent<Enemy>());
        HandleUI();
    }

    private void Enemy_OnEnemySupported(float healthIncreaseOnSupport)
    {
        if (currentHealth == maxHealth)
        {
            currentShield += healthIncreaseOnSupport;
        }
        else
        {
            float difference = maxHealth - currentHealth;
            if (difference >= healthIncreaseOnSupport)
            {
                currentHealth += healthIncreaseOnSupport;
            }
            else
            {
                currentHealth = maxHealth;
                currentShield += healthIncreaseOnSupport - difference;
            }
        }

        HandleUI();
    }

    private void EnemyAnimation_OnSupportComplete()
    {
        OnEnemySupported?.Invoke(healthIncreaseOnSupport);
    }

    private void MinionAI_OnEnemyAttacked(Enemy enemy, float damage)
    {

        if (enemy == null || enemy.gameObject != gameObject)
        {
            return;
        }

        if (currentShield <= 0f)
            currentHealth -= damage;
        else
        {
            float difference = damage - currentShield;
            if (difference < 0f)
            {
                currentShield -= damage;
            }
            else
            {
                currentShield = 0f;
                currentHealth -= difference;
            }
        }
        if (currentHealth <= 0f)
        {
            OnEnemyDeath?.Invoke();
            OnEnemyDied?.Invoke(enemy);
        }

        HandleUI();
    }

    private void HandleUI()
    {
        if (currentShield == 0f)
        {
            shieldObject.SetActive(false);
        }
        else
        {
            shieldObject.SetActive(true);
        }

        shieldImage.fillAmount = currentShield / maxShield;
        healthImage.fillAmount = currentHealth / maxHealth;
    }

    private void OnDestroy()
    {
        MinionAI.OnEnemyAttacked -= MinionAI_OnEnemyAttacked;
        enemyAnimation.OnSupportComplete -= EnemyAnimation_OnSupportComplete;
        OnEnemySupported -= Enemy_OnEnemySupported;
        PlayerStats.Instance.OnEnemyConsumed -= EnemyConsumed;
    }

    public float GetCurrentHealth()
    {
        return currentHealth + currentShield;
    }
    public float GetMaxHealth()
    {
        return maxHealth + maxShield;
    }
    public float GetCurrentScale()
    {
        return transform.localScale.x;
    }
}
