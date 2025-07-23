using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;

    [SerializeField] private GameObject pauseUIObject;

    private void Awake()
    {
        pauseButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            pauseButton.gameObject.SetActive(false);
            pauseUIObject.SetActive(true);
        });
        resumeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            pauseUIObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        });
        homeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        });
        restartButton.onClick.AddListener(() =>
        {
        Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        });
    }
}
