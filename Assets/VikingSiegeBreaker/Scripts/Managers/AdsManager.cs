using UnityEngine;
using System;

namespace VikingSiegeBreaker.Managers
{
    /// <summary>
    /// Ads integration wrapper for LevelPlay (ironSource) SDK.
    /// Handles interstitial and rewarded video ads with callbacks.
    ///
    /// SETUP INSTRUCTIONS:
    /// 1. Import LevelPlay SDK (ironSource) from Unity Package Manager or Asset Store
    /// 2. Configure App Keys in ironSource dashboard
    /// 3. Uncomment LevelPlay API calls below (marked with // LEVELPLAY:)
    /// 4. Add using IronSource; at the top
    /// 5. Test with Test Suite before production
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool adsEnabled = true;
        [SerializeField] private bool testMode = true; // Use test ads

        [Header("Placement IDs")]
        [SerializeField] private string rewardedPlacementId = "Rewarded_Android";
        [SerializeField] private string interstitialPlacementId = "Interstitial_Android";

        [Header("State")]
        [SerializeField] private bool isInitialized = false;
        [SerializeField] private bool isRewardedReady = false;
        [SerializeField] private bool isInterstitialReady = false;

        [Header("Frequency")]
        [SerializeField] private int interstitialCounter = 0;
        [SerializeField] private int interstitialFrequency = 3; // Show every N game overs

        // Callbacks
        private Action onRewardedSuccess;
        private Action onRewardedFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeAds();
        }

        #region Initialization

        /// <summary>
        /// Initializes the LevelPlay SDK.
        /// </summary>
        private void InitializeAds()
        {
            if (!adsEnabled)
            {
                Debug.Log("[AdsManager] Ads disabled");
                return;
            }

            // Check if Remove Ads is purchased
            bool noAdsPurchased = PlayerPrefs.GetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;
            if (noAdsPurchased)
            {
                Debug.Log("[AdsManager] Remove Ads purchased - disabling ads");
                adsEnabled = false;
                return;
            }

            Debug.Log("[AdsManager] Initializing LevelPlay SDK...");

            // LEVELPLAY: Initialize SDK
            // Example:
            // string appKey = "YOUR_APP_KEY"; // Get from ironSource dashboard
            // IronSource.Agent.validateIntegration();
            // IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

            // LEVELPLAY: Set user ID (optional, for analytics)
            // IronSource.Agent.setUserId(SystemInfo.deviceUniqueIdentifier);

            // LEVELPLAY: Register callbacks
            // RegisterCallbacks();

            // For testing without SDK:
            isInitialized = true;
            isRewardedReady = true;
            isInterstitialReady = true;

            Debug.Log("[AdsManager] SDK initialized (test mode)");
        }

        /// <summary>
        /// Registers LevelPlay callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            // LEVELPLAY: Rewarded Video Callbacks
            // IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpened;
            // IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosed;
            // IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoAdAvailable;
            // IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoAdUnavailable;
            // IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailed;
            // IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewarded;
            // IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoAdClicked;

            // LEVELPLAY: Interstitial Callbacks
            // IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReady;
            // IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailed;
            // IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialAdOpened;
            // IronSourceInterstitialEvents.onAdClosedEvent += InterstitialAdClosed;
            // IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialAdShowFailed;
            // IronSourceInterstitialEvents.onAdClickedEvent += InterstitialAdClicked;
        }

        #endregion

        #region Rewarded Video

        /// <summary>
        /// Shows a rewarded video ad.
        /// </summary>
        /// <param name="onSuccess">Callback when user watches full ad</param>
        /// <param name="onFailed">Callback when ad fails or user closes early</param>
        public void ShowRewardedAd(Action onSuccess, Action onFailed = null)
        {
            if (!adsEnabled || !isInitialized)
            {
                Debug.LogWarning("[AdsManager] Ads not available");
                onFailed?.Invoke();
                return;
            }

            // Store callbacks
            onRewardedSuccess = onSuccess;
            onRewardedFailed = onFailed;

            // LEVELPLAY: Check if rewarded ad is ready
            // if (IronSource.Agent.isRewardedVideoAvailable())
            // {
            //     IronSource.Agent.showRewardedVideo(rewardedPlacementId);
            // }
            // else
            // {
            //     Debug.LogWarning("[AdsManager] Rewarded video not ready");
            //     onFailed?.Invoke();
            // }

            // Test mode: Simulate successful ad
            if (testMode)
            {
                Debug.Log("[AdsManager] TEST MODE: Simulating rewarded ad success");
                Invoke(nameof(SimulateRewardedSuccess), 0.5f);
            }
        }

        private void SimulateRewardedSuccess()
        {
            onRewardedSuccess?.Invoke();
            onRewardedSuccess = null;
            onRewardedFailed = null;
        }

        /// <summary>
        /// Checks if a rewarded ad is available.
        /// </summary>
        public bool IsRewardedAdReady()
        {
            if (!adsEnabled || !isInitialized) return false;

            // LEVELPLAY: Check availability
            // return IronSource.Agent.isRewardedVideoAvailable();

            // Test mode
            return isRewardedReady;
        }

        #endregion

        #region Interstitial

        /// <summary>
        /// Shows an interstitial ad (typically after game over).
        /// </summary>
        public void ShowInterstitial()
        {
            if (!adsEnabled || !isInitialized) return;

            interstitialCounter++;

            // Show based on frequency
            if (interstitialCounter < interstitialFrequency)
            {
                Debug.Log($"[AdsManager] Interstitial skipped ({interstitialCounter}/{interstitialFrequency})");
                return;
            }

            interstitialCounter = 0;

            // LEVELPLAY: Check if interstitial is ready
            // if (IronSource.Agent.isInterstitialReady())
            // {
            //     IronSource.Agent.showInterstitial(interstitialPlacementId);
            // }
            // else
            // {
            //     Debug.LogWarning("[AdsManager] Interstitial not ready");
            //     LoadInterstitial();
            // }

            Debug.Log("[AdsManager] TEST MODE: Simulating interstitial ad");
        }

        /// <summary>
        /// Loads an interstitial ad.
        /// </summary>
        private void LoadInterstitial()
        {
            if (!adsEnabled || !isInitialized) return;

            // LEVELPLAY: Load interstitial
            // IronSource.Agent.loadInterstitial();

            Debug.Log("[AdsManager] Loading interstitial...");
        }

        #endregion

        #region Callbacks Implementation (LEVELPLAY)

        // REWARDED VIDEO CALLBACKS
        // Uncomment and implement when LevelPlay SDK is integrated

        /*
        private void RewardedVideoAdOpened(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Rewarded video opened");
        }

        private void RewardedVideoAdClosed(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Rewarded video closed");
            // If closed without reward, trigger failure callback
            if (onRewardedFailed != null)
            {
                onRewardedFailed.Invoke();
                onRewardedFailed = null;
                onRewardedSuccess = null;
            }
        }

        private void RewardedVideoAdAvailable(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Rewarded video available");
            isRewardedReady = true;
        }

        private void RewardedVideoAdUnavailable()
        {
            Debug.LogWarning("[AdsManager] Rewarded video unavailable");
            isRewardedReady = false;
        }

        private void RewardedVideoAdShowFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            Debug.LogError($"[AdsManager] Rewarded video show failed: {error.getDescription()}");
            onRewardedFailed?.Invoke();
            onRewardedFailed = null;
            onRewardedSuccess = null;
        }

        private void RewardedVideoAdRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            Debug.Log($"[AdsManager] Rewarded video completed! Placement: {placement.getPlacementName()}");
            onRewardedSuccess?.Invoke();
            onRewardedSuccess = null;
            onRewardedFailed = null;
        }

        private void RewardedVideoAdClicked(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Rewarded video clicked");
        }

        // INTERSTITIAL CALLBACKS

        private void InterstitialAdReady(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Interstitial ready");
            isInterstitialReady = true;
        }

        private void InterstitialAdLoadFailed(IronSourceError error)
        {
            Debug.LogError($"[AdsManager] Interstitial load failed: {error.getDescription()}");
            isInterstitialReady = false;
        }

        private void InterstitialAdOpened(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Interstitial opened");
        }

        private void InterstitialAdClosed(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Interstitial closed");
            LoadInterstitial(); // Preload next
        }

        private void InterstitialAdShowFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            Debug.LogError($"[AdsManager] Interstitial show failed: {error.getDescription()}");
            LoadInterstitial(); // Retry load
        }

        private void InterstitialAdClicked(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdsManager] Interstitial clicked");
        }
        */

        #endregion

        #region Application Lifecycle (Required for LevelPlay)

        private void OnApplicationPause(bool pauseStatus)
        {
            // LEVELPLAY: Important for proper ad lifecycle
            // IronSource.Agent.onApplicationPause(pauseStatus);
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Enables/disables ads (call after IAP purchase).
        /// </summary>
        public void SetAdsEnabled(bool enabled)
        {
            adsEnabled = enabled;
            Debug.Log($"[AdsManager] Ads enabled: {enabled}");
        }

        #endregion
    }
}
