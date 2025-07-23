using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TutoUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> screens;
    private int index;

    private void Start()
    {
        index = 0;
        Time.timeScale = 1f;

        DisableAllScreens();
        screens[index].SetActive(true);
    }

    private void DisableAllScreens()
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            index++;
            if (index >= screens.Count)
            {
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                screens[index].SetActive(true);
            }
        }
    }
}
