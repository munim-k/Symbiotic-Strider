using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a temporary script that is used to test the logic of showing ads using buttons
/// </summary>
public class AdButtons : MonoBehaviour
{
    //get reference to the buttons
    [SerializeField] private Button interstitialAdShowButton;
    [SerializeField] private Button rewardedAdShowButton;

    private void Start()
    {
        interstitialAdShowButton.onClick.AddListener(() =>
        {
            AdManager.Instance.ShowInterstitialAd();
        });
        rewardedAdShowButton.onClick.AddListener(() =>
        {
            AdManager.Instance.ShowRewardedAd();
        });
    }
}
