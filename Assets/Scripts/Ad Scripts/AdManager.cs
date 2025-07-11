using UnityEngine;
using GoogleMobileAds.Api;
using System;
using Unity.VisualScripting;

/// <summary>
/// This script will be used to load and show ads based on the specific platform the game is being played on. Just remeber to set the ad IDs in the inspector.
/// As well as set the android and iOS ids in Assets -> Google Mobile Ads -> Settings
/// </summary>
public class AdManager : MonoBehaviour
{
    //Instance for the other classes
    public static AdManager Instance { get; private set; }

    //references for the ads
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    //ad Ids to load the specific ad
    private string interstitialAdID;
    private string rewardedAdID;


    //platform specific ad unit ids
    [Header("Android Ad Unit IDs")]
    [SerializeField] private string interstitialAdID_android;
    [SerializeField] private string rewardedAdID_android;

    [Header("iOS Ad Unit IDs")]
    [SerializeField] private string interstitialAdID_iOS;
    [SerializeField] private string rewardedAdID_iOS;

    private void Awake()
    {
        //set the singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // assign ids based on the platform
#if UNITY_ANDROID
        interstitialAdID = interstitialAdID_android;
        rewardedAdID = rewardedAdID_android;
#elif UNITY_IOS
        interstitialAdID = interstitialAdID_iOS;
        rewardedAdID = rewardedAdID_iOS;
#else
        interstitialAdID = "ca-app-pub-3940256099942544/1033173712"; // Test ID for editor
        rewardedAdID = "ca-app-pub-3940256099942544/5224354917";
#endif

        MobileAds.Initialize(initStatus => { });

        //load the ads at the start
        LoadInterstitialAd();
        LoadRewardedAd();
    }


    private void LoadInterstitialAd()
    {
        //make a request of loading the interstitial ad and set reference
        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialAdID, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to Load Ad");
                return;
            }

            interstitialAd = ad;

        });
    }
    private void LoadRewardedAd()
    {
        //make a request to load rewarded ad
        var adRequest = new AdRequest();
        RewardedAd.Load(rewardedAdID, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to Load Ad");
                return;
            }

            rewardedAd = ad;
        });
    }
    public void ShowInterstitialAd()
    {
        //check if interstitial ad exists and ad can be shown, then show ad
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            LoadInterstitialAd();
        }
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                //wouldnt work in editor
            });
            LoadRewardedAd();
        }
    }
}
