using UnityEngine;

namespace VikingSiegeBreaker.Data
{
    /// <summary>
    /// ScriptableObject for global game settings and balance parameters.
    /// Create ONE instance via: Assets > Create > VikingSiegeBreaker > Data > GameSettings
    /// Reference this as a singleton in Resources/Settings/GameSettings.asset
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "VikingSiegeBreaker/Data/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameSettings>("Settings/GameSettings");
                    if (_instance == null)
                    {
                        Debug.LogError("[GameSettings] No GameSettings found in Resources/Settings/");
                    }
                }
                return _instance;
            }
        }

        [Header("Game Version")]
        public string version = "1.0.0";
        public int buildNumber = 1;

        [Header("Gameplay Balance")]
        [Tooltip("Global damage multiplier (for balancing)")]
        [Range(0.5f, 2f)]
        public float globalDamageMultiplier = 1f;

        [Tooltip("Global coin multiplier (for events)")]
        [Range(1f, 5f)]
        public float globalCoinMultiplier = 1f;

        [Tooltip("Global XP multiplier (for events)")]
        [Range(1f, 5f)]
        public float globalXPMultiplier = 1f;

        [Header("Starting Resources")]
        public int startingCoins = 0;
        public int startingGems = 0;

        [Header("Physics")]
        public float gravity = -9.81f;
        public float defaultMass = 1f;

        [Header("Difficulty Curve")]
        [Tooltip("Distance scaling curve (x=distance, y=difficulty 0-1)")]
        public AnimationCurve difficultyCurve = AnimationCurve.Linear(0f, 0f, 1000f, 1f);

        [Header("Economy")]
        [Tooltip("Gem to coin conversion rate")]
        public int gemToCoinRate = 100; // 1 gem = 100 coins

        [Tooltip("Revive cost in gems")]
        public int reviveGemCost = 10;

        [Header("Ads")]
        [Tooltip("Rewarded ad coin reward multiplier")]
        public float rewardedAdCoinMultiplier = 2f;

        [Tooltip("Interstitial ad show frequency (every X game overs)")]
        public int interstitialFrequency = 3;

        [Header("IAP")]
        [Tooltip("Remove Ads IAP price (USD)")]
        public float removeAdsPrice = 2.99f;

        [Tooltip("Starter pack gem amount")]
        public int starterPackGems = 500;

        [Header("Audio")]
        [Range(0f, 1f)]
        public float defaultMusicVolume = 0.7f;

        [Range(0f, 1f)]
        public float defaultSFXVolume = 1f;

        [Header("UI")]
        public float damageNumberDuration = 1f;
        public float coinPopupDuration = 0.5f;

        [Header("Debug")]
        public bool enableDebugMode = false;
        public bool skipTutorial = false;
        public bool unlockAllUpgrades = false;

        /// <summary>
        /// Gets difficulty at a given distance.
        /// </summary>
        public float GetDifficultyAtDistance(float distance)
        {
            return difficultyCurve.Evaluate(distance);
        }

        /// <summary>
        /// Applies global multipliers to a value.
        /// </summary>
        public int ApplyGlobalMultipliers(int baseValue, bool isCoins, bool isXP)
        {
            float multiplier = 1f;

            if (isCoins)
                multiplier *= globalCoinMultiplier;

            if (isXP)
                multiplier *= globalXPMultiplier;

            return Mathf.RoundToInt(baseValue * multiplier);
        }

        private void OnValidate()
        {
            // Ensure difficulty curve has proper range
            if (difficultyCurve == null || difficultyCurve.length == 0)
            {
                difficultyCurve = AnimationCurve.Linear(0f, 0f, 1000f, 1f);
            }
        }
    }
}
