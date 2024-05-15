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
                LoadAd();
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
            case AdsType.Banner: return "ca-app-pub-9489260011896658/9770620520";
            case AdsType.Test: return "ca-app-pub-3940256099942544/5224354917";

            default:
                Debug.LogError("identifier not found");
                return "";
        }
    }
    
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        if(_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

      
        _bannerView.LoadAd(adRequest);
    }
    
    BannerView _bannerView;

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        // Use the AdSize argument to set a custom size for the ad.

        _bannerView = new BannerView(GetAdsIdentifier(AdsType.Banner), AdSize.SmartBanner, AdPosition.Bottom);
    }
    
    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    
    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                      + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                           + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
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