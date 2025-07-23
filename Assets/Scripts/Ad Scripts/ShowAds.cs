using Unity;
using UnityEngine;

public class ShowAds : MonoBehaviour
{
    [SerializeField] private float interstitialAdTime = 180f;
    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interstitialAdTime)
        {
            AdManager.Instance.ShowInterstitialAd();
            timer = 0f;
        }
    }
}