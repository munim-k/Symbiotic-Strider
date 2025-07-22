using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUpdateUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI enemiesNeededToUpgrade;

    private void Start()
    {
        EnemyManager.Instance.OnTimerChanged += OnTimerChanged;
        PlayerStats.Instance.OnMaxEnemiesIncreased += MaxEnemiesIncreased;
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
