using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUpdateUI : MonoBehaviour
{
    public static EnemyUpdateUI Instance { get; private set; }
    public Action OnTimeSkipped;
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI enemiesNeededToUpgrade;
    [SerializeField] private Button timeSkipButton;
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
        timeSkipButton.onClick.AddListener(() =>
        {
            OnTimeSkipped?.Invoke();
        });
    }

    private void Start()
    {
        EnemyManager.Instance.OnTimerChanged += OnTimerChanged;
        EnemyManager.Instance.OnCanTimeSkip += OnCanSkipTime;
        PlayerStats.Instance.OnMaxEnemiesIncreased += MaxEnemiesIncreased;
    }

    private void OnCanSkipTime(bool canSkipTime)
    {
        timeSkipButton.interactable = canSkipTime;
    }

    private void MaxEnemiesIncreased(int current, int total)
    {
        enemiesNeededToUpgrade.text = current.ToString() + "/" + total.ToString();
    }

    private void OnTimerChanged(float currentTime, float totalTime)
    {
        timerImage.fillAmount = currentTime / totalTime;
    }
}
