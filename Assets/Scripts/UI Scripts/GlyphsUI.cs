using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class GlyphsUI : MonoBehaviour
{
    public static GlyphsUI Instance { get; private set; }
    public Action OnMountainButtonClicked;
    public Action OnFortressButtonClicked;
    public Action OnLanceButtonClicked;
    public Action OnCometButtonClicked;
    [SerializeField] private Button mountainButton;
    [SerializeField] private Button fortressButton;
    [SerializeField] private Button lanceButton;
    [SerializeField] private Button cometButton;

    [SerializeField] private GameObject mountainClickedButton;
    [SerializeField] private GameObject lanceClickedButton;
    [SerializeField] private GameObject cometClickedButton;
    [SerializeField] private GameObject fortressClickedButton;

    private bool isMountainButtonClicked = false;
    private bool isLanceButtonClicked = false;
    private bool isCometButtonClicked = false;
    private bool isFortressButtonClicked = false;
    private List<Button> clickedButtonsList = new List<Button>();
    private int clickedTime = 0;

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
        mountainButton.onClick.AddListener(() =>
        {
            if (isMountainButtonClicked)
                return;
            clickedButtonsList.Add(mountainButton);
            clickedTime++;
            mountainClickedButton.SetActive(true);
            isMountainButtonClicked = true;
            if (clickedTime == 2)
            {
                HandleAllClick();
            }
        });
        fortressButton.onClick.AddListener(() =>
        {
            if (isFortressButtonClicked)
                return;
            isFortressButtonClicked = true;
            clickedButtonsList.Add(fortressButton);
            clickedTime++;
            fortressClickedButton.SetActive(true);

            if (clickedTime == 2)
            {
                HandleAllClick();
            }
        });
        lanceButton.onClick.AddListener(() =>
        {
            if (isLanceButtonClicked)
                return;
            isLanceButtonClicked = true;
            clickedButtonsList.Add(lanceButton);
            clickedTime++;
            lanceClickedButton.SetActive(true);

            if (clickedTime == 2)
            {
                HandleAllClick();
            }
        });
        cometButton.onClick.AddListener(() =>
        {
            if (isCometButtonClicked)
                return;
            isCometButtonClicked = false;
            clickedButtonsList.Add(cometButton);
            clickedTime++;
            cometClickedButton.SetActive(true);

            if (clickedTime == 2)
            {
                HandleAllClick();
            }
        });
    }

    private void HandleAllClick()
    {
        foreach (Button clickedButton in clickedButtonsList)
        {
            if (clickedButton == mountainButton)
            {
                OnMountainButtonClicked?.Invoke();
            }
            else if (clickedButton == fortressButton)
            {
                OnFortressButtonClicked?.Invoke();
            }
            else if (clickedButton == lanceButton)
            {
                OnLanceButtonClicked?.Invoke();
            }
            else
            {
                OnCometButtonClicked?.Invoke();
            }
        }

        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    void Start()
    {
        Time.timeScale = 0f;
    }
}
