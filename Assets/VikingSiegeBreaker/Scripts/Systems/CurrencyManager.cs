using UnityEngine;
using System;

namespace VikingSiegeBreaker.Systems
{
    /// <summary>
    /// Manages all in-game currencies: Coins, Gems, and XP.
    /// Handles earning, spending, and persistence.
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [Header("Current Balances")]
        [SerializeField] private int coins = 0;
        [SerializeField] private int gems = 0;
        [SerializeField] private int xp = 0;

        [Header("Multipliers")]
        [SerializeField] private float coinMultiplier = 1f;
        [SerializeField] private float xpMultiplier = 1f;

        // Events
        public event Action<int> OnCoinsChanged;
        public event Action<int> OnGemsChanged;
        public event Action<int> OnXPChanged;

        // Properties
        public int Coins => coins;
        public int Gems => gems;
        public int XP => xp;
        public float CoinMultiplier => coinMultiplier;
        public float XPMultiplier => xpMultiplier;

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
            // Apply multipliers from upgrades
            ApplyMultipliers();
        }

        #region Coins

        /// <summary>
        /// Adds coins (applies multiplier).
        /// </summary>
        public void AddCoins(int amount)
        {
            int actualAmount = Mathf.RoundToInt(amount * coinMultiplier);
            coins += actualAmount;
            OnCoinsChanged?.Invoke(coins);

            Debug.Log($"[CurrencyManager] +{actualAmount} coins (total: {coins})");
        }

        /// <summary>
        /// Spends coins (returns false if insufficient).
        /// </summary>
        public bool SpendCoins(int amount)
        {
            if (coins < amount)
            {
                Debug.LogWarning($"[CurrencyManager] Insufficient coins (have: {coins}, need: {amount})");
                return false;
            }

            coins -= amount;
            OnCoinsChanged?.Invoke(coins);

            Debug.Log($"[CurrencyManager] -{amount} coins (total: {coins})");
            return true;
        }

        /// <summary>
        /// Checks if player has enough coins.
        /// </summary>
        public bool HasCoins(int amount)
        {
            return coins >= amount;
        }

        #endregion

        #region Gems

        /// <summary>
        /// Adds gems (premium currency, no multiplier).
        /// </summary>
        public void AddGems(int amount)
        {
            gems += amount;
            OnGemsChanged?.Invoke(gems);

            Debug.Log($"[CurrencyManager] +{amount} gems (total: {gems})");
        }

        /// <summary>
        /// Spends gems (returns false if insufficient).
        /// </summary>
        public bool SpendGems(int amount)
        {
            if (gems < amount)
            {
                Debug.LogWarning($"[CurrencyManager] Insufficient gems (have: {gems}, need: {amount})");
                return false;
            }

            gems -= amount;
            OnGemsChanged?.Invoke(gems);

            Debug.Log($"[CurrencyManager] -{amount} gems (total: {gems})");
            return true;
        }

        /// <summary>
        /// Checks if player has enough gems.
        /// </summary>
        public bool HasGems(int amount)
        {
            return gems >= amount;
        }

        #endregion

        #region XP

        /// <summary>
        /// Adds XP (applies multiplier).
        /// </summary>
        public void AddXP(int amount)
        {
            int actualAmount = Mathf.RoundToInt(amount * xpMultiplier);
            xp += actualAmount;
            OnXPChanged?.Invoke(xp);

            Debug.Log($"[CurrencyManager] +{actualAmount} XP (total: {xp})");

            // Check for evolution
            EvolutionManager.Instance?.CheckForEvolution();
        }

        /// <summary>
        /// Gets current XP amount.
        /// </summary>
        public int GetXP()
        {
            return xp;
        }

        #endregion

        #region Multipliers

        /// <summary>
        /// Applies multipliers from upgrades.
        /// </summary>
        private void ApplyMultipliers()
        {
            var upgradeManager = UpgradeManager.Instance;
            if (upgradeManager == null) return;

            // Get multipliers from upgrades (assumes % values like 10 = +10%)
            float coinBonus = upgradeManager.GetUpgradeValue("CoinMultiplier");
            coinMultiplier = 1f + (coinBonus / 100f);

            float xpBonus = upgradeManager.GetUpgradeValue("XPMultiplier");
            xpMultiplier = 1f + (xpBonus / 100f);

            Debug.Log($"[CurrencyManager] Multipliers - Coins: {coinMultiplier:F2}x, XP: {xpMultiplier:F2}x");
        }

        /// <summary>
        /// Updates multipliers (call after upgrading).
        /// </summary>
        public void RefreshMultipliers()
        {
            ApplyMultipliers();
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Loads currency data (called by SaveSystem).
        /// </summary>
        public void LoadData(int loadedCoins, int loadedGems, int loadedXP)
        {
            coins = loadedCoins;
            gems = loadedGems;
            xp = loadedXP;

            OnCoinsChanged?.Invoke(coins);
            OnGemsChanged?.Invoke(gems);
            OnXPChanged?.Invoke(xp);

            Debug.Log($"[CurrencyManager] Data loaded - Coins: {coins}, Gems: {gems}, XP: {xp}");
        }

        #endregion

        #region Debug/Cheats

        /// <summary>
        /// Adds coins without multiplier (for debug/testing).
        /// </summary>
        public void CheatAddCoins(int amount)
        {
            coins += amount;
            OnCoinsChanged?.Invoke(coins);
            Debug.Log($"[CurrencyManager] CHEAT: Added {amount} coins");
        }

        /// <summary>
        /// Adds gems (for debug/testing).
        /// </summary>
        public void CheatAddGems(int amount)
        {
            gems += amount;
            OnGemsChanged?.Invoke(gems);
            Debug.Log($"[CurrencyManager] CHEAT: Added {amount} gems");
        }

        /// <summary>
        /// Adds XP without multiplier (for debug/testing).
        /// </summary>
        public void CheatAddXP(int amount)
        {
            xp += amount;
            OnXPChanged?.Invoke(xp);
            Debug.Log($"[CurrencyManager] CHEAT: Added {amount} XP");
        }

        #endregion

        #region Formatting

        /// <summary>
        /// Formats currency for display (e.g., 1,234 or 1.2K).
        /// </summary>
        public static string FormatCurrency(int amount)
        {
            if (amount >= 1000000)
            {
                return $"{amount / 1000000f:F1}M";
            }
            else if (amount >= 1000)
            {
                return $"{amount / 1000f:F1}K";
            }
            else
            {
                return amount.ToString("N0");
            }
        }

        #endregion
    }
}
