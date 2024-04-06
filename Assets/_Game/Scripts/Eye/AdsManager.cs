using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoManager
{  
    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(state =>
        {
            Debug.LogWarning(state);
        });
    }

    private string GetAdsIdentifier(AdsType type)
    {
        switch (type)
        {
            case AdsType.RewardedAd: return "ca-app-pub-9489260011896658/9925833817";

            default:
                Debug.LogError("identifier not found");
                return "";
        }
    }

    // These ad units are configured to always serve test ads.
    const string adUnitId = "ca-app-pub-3940256099942544/5224354917";


    private RewardedAd _rewardedAd;

    private void LoadRewardedAd()
    {
        // Load a rewarded ad
        RewardedAd.Load(adUnitId, new AdRequest(),
            (ad, loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Rewarded ad failed to load with error: " +
                              loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Rewarded ad failed to load.");
                    return;
                }

                Debug.Log("Rewarded ad loaded.");
                _rewardedAd = ad;
                Action<Reward> a = null;
                _rewardedAd.Show(a);
            });
    }
    
    /*private bool LoadRewardedAd(Action<Reward> rewardAction)
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("shop-element-buy");

        RewardedAd.Load
        (GetAdsIdentifier(AdsType.RewardedAd),
            adRequest, (ad, error) =>
            {
                if (ad == null || error != null)
                {
                    Debug.LogWarning("reward ad error > " + error);
                    return;
                }

                _rewardedAd = ad;
            }
        );

        return _rewardedAd != null;
    }*/

    private void EventRewardedAd
    (
        Action<Reward> rewardAction,
        Action<bool> adsShowState,
        out Action startAds
    )
    {
        startAds = null;

        var state = _rewardedAd != null && _rewardedAd.CanShowAd();

        if (state)
        {
            startAds = () => { _rewardedAd.Show(rewardAction); };
            _rewardedAd.Show(rewardAction);
        }

        adsShowState.Invoke(state);
    }

    public void TryStartAds
    (
        AdsType type,
        Action<bool> adsShowState,
        out Action startAdsEvent,
        Action<Reward> rewardValue = null
    )
    {
        LoadRewardedAd();
        /*startAdsEvent = null;
        switch (type)
        {
            case AdsType.RewardedAd:
                if (LoadRewardedAd(rewardValue)) EventRewardedAd(rewardValue, adsShowState, out startAdsEvent);
                break;
        }*/
        startAdsEvent = default;   
    }
}