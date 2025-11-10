using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class GoogleAdsManager : MonoBehaviour
{
    //#if UNITY_ANDROID
    //    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test º¸»óÇü ±¤°í ID
    ////#elif UNITY_IPHONE
    ////  private string adUnitId = "ca-app-pub-3940256099942544/1712485313";
    //#else
    //  private string adUnitId = "unused";
    //#endif

    //#if UNITY_ANDROID
    //    private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // test ÀÎµí
    //#elif UNITY_IPHONE
    //  private string bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    //#else
    //  private string bannerAdUnitId = "unused";
    //#endif

    private static GoogleAdsManager instance = null;
    public static GoogleAdsManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GoogleAdsManager>();
            }

            return instance;
        }
    }

    public string rewardAdUnitId;
    public string bannerAdUnitId;
    [Tooltip("Àü¸é ±¤°í ID")]
    public string interstitialAdUnitId;
    public string nativeAdUnitId;

    private BannerView bannerView = null;
    private RewardedAd rewardedAd = null;
    private InterstitialAd interstitialAd = null;

    private string rewardAdUnitIdStr = "";
    private string bannerAdUnitIdStr = "";
    private string interstitialAdUnitIdStr = "";
    private string nativeAdUnitIdStr = ""; 

    private const string APP_OPENING_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/9257395921";
    private const string REWARD_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/5224354917";
    private const string BANNER_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/6300978111";
    private const string INTERSTITIAL_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/1033173712";
    private const string REWARD_INTERSTITIAL_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/5354046379";
    private const string NATIVE_TEST_AD_UNIT_ID_STR = "ca-app-pub-3940256099942544/2247696110";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        InitValues();

        // InitAd();
    }

    private void InitValues()
    {
        if (!Application.isMobilePlatform)
        {
            rewardAdUnitIdStr = REWARD_TEST_AD_UNIT_ID_STR;
            bannerAdUnitIdStr = BANNER_TEST_AD_UNIT_ID_STR;
            interstitialAdUnitIdStr = INTERSTITIAL_TEST_AD_UNIT_ID_STR;
            nativeAdUnitIdStr = NATIVE_TEST_AD_UNIT_ID_STR;
        }
        else
        {
            rewardAdUnitIdStr = rewardAdUnitId;
            bannerAdUnitIdStr = bannerAdUnitId;
            interstitialAdUnitIdStr= interstitialAdUnitId;
            nativeAdUnitIdStr = nativeAdUnitId;
        }
    }

    private void InitAd()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            
            LoadBottomBannerAd();
        });
    }

    #region Banner Ad
    public void RequestBanner()
    {
        // adUnitId = "";
    }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyBannerView();
        }
        
        AdSize adSize = new AdSize(320, 50);
        // bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView = new BannerView(bannerAdUnitIdStr, adSize, AdPosition.Bottom);
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBottomBannerAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        CustomDebug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);

        ListenToAdEvents();
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            CustomDebug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            CustomDebug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            CustomDebug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            CustomDebug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            CustomDebug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            CustomDebug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            CustomDebug.Log("Banner view full screen content closed.");
        };
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (bannerView != null)
        {
            CustomDebug.Log("Destroying banner view.");
            bannerView.Destroy();
            bannerView = null;
        }
    }
    #endregion

    #region Àü¸é ±¤°í(Interstitial)

    #endregion

    #region Native

    #endregion




    #region Reward Ad
    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    //public void LoadRewardedAd()
    //{
    //    // Clean up the old ad before loading a new one.
    //    if (rewardedAd != null)
    //    {
    //        rewardedAd.Destroy();
    //        rewardedAd = null;
    //    }

    //    CustomDebug.Log("Loading the rewarded ad.");

    //    // create our request used to load the ad.
    //    var adRequest = new AdRequest();

    //    // send the request to load the ad.
    //    RewardedAd.Load(rewardAdUnitIdStr, adRequest,
    //        (RewardedAd ad, LoadAdError error) =>
    //        {
    //            // if error is not null, the load request failed.
    //            if (error != null || ad == null)
    //            {
    //                CustomDebug.LogError("Rewarded ad failed to load an ad " +
    //                               "with error : " + error);
    //                return;
    //            }

    //            CustomDebug.Log("Rewarded ad loaded with response : "
    //                      + ad.GetResponseInfo());

    //            rewardedAd = ad;

    //            RegisterEventHandlers(rewardedAd);

    //            ShowRewardedAd();
    //        });
    //}

    //public void ShowRewardedAd()
    //{
    //    const string rewardMsg =
    //        "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

    //    if (rewardedAd != null && rewardedAd.CanShowAd())
    //    {
    //        rewardedAd.Show((Reward reward) =>
    //        {
    //            // TODO: Reward the user.
    //            CustomDebug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
    //        });
    //    }
    //}

    //private void RegisterEventHandlers(RewardedAd ad)
    //{
    //    // Raised when the ad is estimated to have earned money.
    //    ad.OnAdPaid += (AdValue adValue) =>
    //    {
    //        CustomDebug.Log(String.Format("Rewarded ad paid {0} {1}.",
    //            adValue.Value,
    //            adValue.CurrencyCode));
    //    };

    //    // Raised when an impression is recorded for an ad.
    //    ad.OnAdImpressionRecorded += () =>
    //    {
    //        CustomDebug.Log("Rewarded ad recorded an impression.");
    //    };

    //    // Raised when a click is recorded for an ad.
    //    ad.OnAdClicked += () =>
    //    {
    //        CustomDebug.Log("Rewarded ad was clicked.");
    //    };

    //    // Raised when an ad opened full screen content.
    //    ad.OnAdFullScreenContentOpened += () =>
    //    {
    //        CustomDebug.Log("Rewarded ad full screen content opened.");
    //    };

    //    // Raised when the ad closed full screen content.
    //    ad.OnAdFullScreenContentClosed += () =>
    //    {
    //        CustomDebug.Log("Rewarded ad full screen content closed.");
    //        // LoadRewardedAd();
    //    };

    //    // Raised when the ad failed to open full screen content.
    //    ad.OnAdFullScreenContentFailed += (AdError error) =>
    //    {
    //        CustomDebug.LogError("Rewarded ad failed to open full screen content " +
    //                       "with error : " + error);
    //        LoadRewardedAd();
    //    };
    //}
    #endregion
}