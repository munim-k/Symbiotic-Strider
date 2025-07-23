using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    public static DeathUI Instance { get; private set; }
    public Action OnPlayerRevived;
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button restartButton;
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
        reviveButton.onClick.AddListener(() =>
        {
            AdManager.Instance.ShowRewardedAd(() =>
            {
                Time.timeScale = 1f;
                OnPlayerRevived?.Invoke();
                gameObject.SetActive(false);
            });
        });
        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
    }
    private void Start()
    {
        PlayerStats.Instance.OnPlayerDeath += OnPlayerDeath;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance == null)
            return;
        PlayerStats.Instance.OnPlayerDeath -= OnPlayerDeath;   
    }

    private void OnPlayerDeath(bool hasNotBeenRevived)
    {
        gameObject.SetActive(true);
        if (!hasNotBeenRevived)
        {
            reviveButton.enabled = false;
        }

        Time.timeScale = 0f;
    }
}
