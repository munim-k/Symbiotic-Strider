using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance { get; private set; }
    [SerializeField] private Button healthButton;
    [SerializeField] private Button satminaButton;
    [SerializeField] private Button stunButton;
    [SerializeField] private Button frostButton;
    [SerializeField] private Button poisonButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button moveSpeedButton;


    public Action OnHealthUpgraded;
    public Action OnStaminaUpgraded;
    public Action OnFrostUpgraded;
    public Action OnPoisonUpgraded;
    public Action OnStunUpgraded;
    public Action OnMoveSpeedUpgraded;
    public Action OnDamageUpgraded;

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
        healthButton.onClick.AddListener(() =>
        {
            OnHealthUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        satminaButton.onClick.AddListener(() =>
        {
            OnStaminaUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        poisonButton.onClick.AddListener(() =>
        {
            OnPoisonUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        stunButton.onClick.AddListener(() =>
        {
            OnStunUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        frostButton.onClick.AddListener(() =>
        {
            OnFrostUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        moveSpeedButton.onClick.AddListener(() =>
        {
            OnMoveSpeedUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
        damageButton.onClick.AddListener(() =>
        {
            OnDamageUpgraded?.Invoke();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
    }

    private void Start()
    {
        gameObject.SetActive(false);
        PlayerStats.Instance.OnPlayerUpgraded += PlayerStats_OnPlayerUpgraded;
    }

    private void PlayerStats_OnPlayerUpgraded(float obj)
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        PlayerStats.Instance.OnPlayerUpgraded -= PlayerStats_OnPlayerUpgraded;
    }
}
