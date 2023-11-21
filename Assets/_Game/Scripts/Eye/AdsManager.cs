using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoManager
{
    private RewardedAd _rewardedAd;

    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(state => { Debug.LogWarning(state); });
    }

    private string GetAdsIdentifier(AdsType type)
    {
        switch (type)
        {
            case AdsType.RewardedInterstitial: return "ca-app-pub-9489260011896658/5290500529";
            case AdsType.RewardedAd: return "ca-app-pub-9489260011896658/9925833817";

            default:
                Debug.LogError("identifier not found");
                return "";
        }
    }

    private bool LoadRewardedAd(Action<Reward> rewardAction)
    {
        print("start load");
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("shop-element-buy");

        RewardedAd.Load
        (GetAdsIdentifier(AdsType.RewardedInterstitial),
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
        startAdsEvent = null;
        switch (type)
        {
            case AdsType.RewardedAd:
                if (LoadRewardedAd(rewardValue)) EventRewardedAd(rewardValue, adsShowState, out startAdsEvent);
                break;
        }
    }
}