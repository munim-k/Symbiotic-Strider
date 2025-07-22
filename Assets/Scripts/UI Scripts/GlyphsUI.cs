using System;
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
            OnMountainButtonClicked?.Invoke();
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        });
        fortressButton.onClick.AddListener(() =>
        {
            OnFortressButtonClicked?.Invoke();
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        });
        lanceButton.onClick.AddListener(() =>
        {
            OnLanceButtonClicked?.Invoke();
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        });
        cometButton.onClick.AddListener(() =>
        {
            OnCometButtonClicked?.Invoke();
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        });
    }
    void Start()
    {
        Time.timeScale = 0f;
    }
}
