using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuiUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
        tutorialButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("TutorialScene");
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
