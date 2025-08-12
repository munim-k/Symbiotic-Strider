using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreIncreasePerSecond = 5;
    [SerializeField] private int scoreOnEnemyKill = 15;
    private int score;
    private const string scorePreText = "Score: ";

    private void Start()
    {
        score = 0;
        scoreText.text = scorePreText + score;
        EnemyHealth.OnEnemyDied += OnEnemyDeath;
        
        StartCoroutine(IncreaseScore());
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        score += scoreOnEnemyKill;
        scoreText.text = scorePreText + score.ToString("");
    }

    private IEnumerator IncreaseScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            score += scoreIncreasePerSecond;
            scoreText.text = scorePreText + score.ToString("");
        }
    }
}
