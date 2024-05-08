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
            try
            {
                LoadRewardedAd();
            }
            catch
            {
                // ignored
            }
        });
    }

    private string GetAdsIdentifier(AdsType type)
    {
        switch (type)
        {
            case AdsType.RewardedAd: return "ca-app-pub-9489260011896658/9925833817";
            case AdsType.Test: return "ca-app-pub-3940256099942544/5224354917";

            default:
                Debug.LogError("identifier not found");
                return "";
        }
    }

    private RewardedAd _rewardedAd;

    private bool LoadRewardedAd()
    {
        // Load a rewarded ad
        RewardedAd.Load(GetAdsIdentifier(AdsType.RewardedAd), new AdRequest(),
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
            });

        return _rewardedAd.CanShowAd();
    }

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
        startAdsEvent = null;
        switch (type)
        {
            case AdsType.RewardedAd:
                if (LoadRewardedAd())
                {
                    EventRewardedAd(rewardValue, adsShowState, out startAdsEvent);
                }
                break;
        }
        startAdsEvent = default;   
    }
}