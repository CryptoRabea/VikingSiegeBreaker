using UnityEngine;
using System;
using System.Collections.Generic;

namespace VikingSiegeBreaker.Systems
{
    /// <summary>
    /// Manages all game upgrades - handles purchasing, leveling, and applying effects.
    /// Supports 100 levels per upgrade with exponential cost/effect curves.
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Upgrade Data")]
        [SerializeField] private List<Data.UpgradeData> allUpgrades = new List<Data.UpgradeData>();

        [Header("Current Levels")]
        private Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();

        [Header("Settings")]
        [SerializeField] private int maxUpgradeLevel = 100;

        // Events
        public event Action<string, int> OnUpgradePurchased; // upgradeName, newLevel

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
            // Initialize all upgrades to level 0
            InitializeUpgrades();
        }

        #region Initialization

        private void InitializeUpgrades()
        {
            // Load upgrade data from Resources if not assigned
            if (allUpgrades.Count == 0)
            {
                var upgrades = Resources.LoadAll<Data.UpgradeData>("ScriptableObjects/Upgrades");
                allUpgrades.AddRange(upgrades);
            }

            // Initialize level tracking
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade != null && !upgradeLevels.ContainsKey(upgrade.upgradeName))
                {
                    upgradeLevels[upgrade.upgradeName] = 0;
                }
            }

            Debug.Log($"[UpgradeManager] Initialized {allUpgrades.Count} upgrades");
        }

        #endregion

        #region Upgrade Purchase

        /// <summary>
        /// Attempts to purchase/upgrade. Returns true if successful.
        /// </summary>
        public bool PurchaseUpgrade(string upgradeName)
        {
            var upgrade = GetUpgradeData(upgradeName);
            if (upgrade == null)
            {
                Debug.LogError($"[UpgradeManager] Upgrade not found: {upgradeName}");
                return false;
            }

            int currentLevel = GetUpgradeLevel(upgradeName);

            // Check max level
            if (currentLevel >= maxUpgradeLevel)
            {
                Debug.LogWarning($"[UpgradeManager] {upgradeName} is already max level");
                return false;
            }

            // Calculate cost for next level
            int cost = CalculateCost(upgrade, currentLevel + 1);

            // Check currency
            if (!CurrencyManager.Instance.HasCoins(cost))
            {
                Debug.LogWarning($"[UpgradeManager] Insufficient coins for {upgradeName} (need: {cost})");
                return false;
            }

            // Purchase
            if (CurrencyManager.Instance.SpendCoins(cost))
            {
                upgradeLevels[upgradeName] = currentLevel + 1;
                OnUpgradePurchased?.Invoke(upgradeName, currentLevel + 1);

                Debug.Log($"[UpgradeManager] Purchased {upgradeName} level {currentLevel + 1} for {cost} coins");

                // Refresh multipliers if this affects currency
                if (upgradeName.Contains("Multiplier"))
                {
                    CurrencyManager.Instance.RefreshMultipliers();
                }

                // Save
                Core.SaveSystem.Instance.SaveGame();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Purchases multiple levels at once (bulk buy).
        /// </summary>
        public bool PurchaseUpgradeMultiple(string upgradeName, int count)
        {
            int totalCost = 0;
            int currentLevel = GetUpgradeLevel(upgradeName);
            var upgrade = GetUpgradeData(upgradeName);

            if (upgrade == null) return false;

            // Calculate total cost
            for (int i = 0; i < count; i++)
            {
                if (currentLevel + i >= maxUpgradeLevel) break;
                totalCost += CalculateCost(upgrade, currentLevel + i + 1);
            }

            // Check currency
            if (!CurrencyManager.Instance.HasCoins(totalCost))
            {
                Debug.LogWarning($"[UpgradeManager] Insufficient coins for {count}x {upgradeName} (need: {totalCost})");
                return false;
            }

            // Purchase
            if (CurrencyManager.Instance.SpendCoins(totalCost))
            {
                int newLevel = Mathf.Min(currentLevel + count, maxUpgradeLevel);
                upgradeLevels[upgradeName] = newLevel;
                OnUpgradePurchased?.Invoke(upgradeName, newLevel);

                Debug.Log($"[UpgradeManager] Purchased {count} levels of {upgradeName} for {totalCost} coins");

                Core.SaveSystem.Instance.SaveGame();
                return true;
            }

            return false;
        }

        #endregion

        #region Cost Calculation

        /// <summary>
        /// Calculates the cost for a specific level using exponential formula.
        /// Formula: baseCost * (costMultiplier ^ (level - 1))
        /// </summary>
        public int CalculateCost(Data.UpgradeData upgrade, int level)
        {
            if (upgrade == null || level <= 0) return 0;

            float cost = upgrade.baseCost * Mathf.Pow(upgrade.costMultiplier, level - 1);
            return Mathf.RoundToInt(cost);
        }

        /// <summary>
        /// Gets the cost for the next level of an upgrade.
        /// </summary>
        public int GetNextLevelCost(string upgradeName)
        {
            var upgrade = GetUpgradeData(upgradeName);
            if (upgrade == null) return 0;

            int currentLevel = GetUpgradeLevel(upgradeName);
            return CalculateCost(upgrade, currentLevel + 1);
        }

        #endregion

        #region Effect Calculation

        /// <summary>
        /// Calculates the effect value for a specific level.
        /// Formula: baseEffect + (effectPerLevel * level)
        /// OR: baseEffect * (1 + effectMultiplier)^level (for multiplicative upgrades)
        /// </summary>
        public float CalculateEffect(Data.UpgradeData upgrade, int level)
        {
            if (upgrade == null || level <= 0) return 0f;

            if (upgrade.useMultiplicativeScaling)
            {
                return upgrade.baseEffect * Mathf.Pow(1f + upgrade.effectMultiplier, level);
            }
            else
            {
                return upgrade.baseEffect + (upgrade.effectPerLevel * level);
            }
        }

        /// <summary>
        /// Gets the current effect value for an upgrade.
        /// </summary>
        public float GetUpgradeValue(string upgradeName)
        {
            var upgrade = GetUpgradeData(upgradeName);
            if (upgrade == null) return 0f;

            int level = GetUpgradeLevel(upgradeName);
            return CalculateEffect(upgrade, level);
        }

        #endregion

        #region Level Tracking

        /// <summary>
        /// Gets the current level of an upgrade.
        /// </summary>
        public int GetUpgradeLevel(string upgradeName)
        {
            if (upgradeLevels.ContainsKey(upgradeName))
            {
                return upgradeLevels[upgradeName];
            }
            return 0;
        }

        /// <summary>
        /// Sets an upgrade level (for loading saves).
        /// </summary>
        public void LoadUpgradeLevel(string upgradeName, int level)
        {
            upgradeLevels[upgradeName] = level;
            Debug.Log($"[UpgradeManager] Loaded {upgradeName} level {level}");
        }

        /// <summary>
        /// Gets the max level for an upgrade.
        /// </summary>
        public int GetMaxLevel()
        {
            return maxUpgradeLevel;
        }

        /// <summary>
        /// Checks if an upgrade is at max level.
        /// </summary>
        public bool IsMaxLevel(string upgradeName)
        {
            return GetUpgradeLevel(upgradeName) >= maxUpgradeLevel;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Gets the UpgradeData ScriptableObject for an upgrade.
        /// </summary>
        public Data.UpgradeData GetUpgradeData(string upgradeName)
        {
            return allUpgrades.Find(u => u.upgradeName == upgradeName);
        }

        /// <summary>
        /// Gets all upgrades.
        /// </summary>
        public List<Data.UpgradeData> GetAllUpgrades()
        {
            return allUpgrades;
        }

        /// <summary>
        /// Gets upgrades by category.
        /// </summary>
        public List<Data.UpgradeData> GetUpgradesByCategory(string category)
        {
            return allUpgrades.FindAll(u => u.category == category);
        }

        #endregion

        #region Debug/Cheats

        /// <summary>
        /// Sets an upgrade to max level (for testing).
        /// </summary>
        [ContextMenu("Max All Upgrades")]
        public void CheatMaxAllUpgrades()
        {
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade != null)
                {
                    upgradeLevels[upgrade.upgradeName] = maxUpgradeLevel;
                }
            }
            Debug.Log("[UpgradeManager] CHEAT: All upgrades maxed");
        }

        /// <summary>
        /// Resets all upgrades to level 0 (for testing).
        /// </summary>
        [ContextMenu("Reset All Upgrades")]
        public void CheatResetAllUpgrades()
        {
            foreach (var key in new List<string>(upgradeLevels.Keys))
            {
                upgradeLevels[key] = 0;
            }
            Debug.Log("[UpgradeManager] CHEAT: All upgrades reset");
        }

        #endregion

        #region Utility

        /// <summary>
        /// Gets a formatted string showing upgrade info.
        /// Example: "Launch Power Lv.15 (+750 power)"
        /// </summary>
        public string GetUpgradeDisplayText(string upgradeName)
        {
            var upgrade = GetUpgradeData(upgradeName);
            if (upgrade == null) return "";

            int level = GetUpgradeLevel(upgradeName);
            float effect = GetUpgradeValue(upgradeName);
            int cost = GetNextLevelCost(upgradeName);

            if (IsMaxLevel(upgradeName))
            {
                return $"{upgrade.displayName} Lv.{level} (MAX) - +{effect:F0}";
            }

            return $"{upgrade.displayName} Lv.{level} - +{effect:F0} (Next: {CurrencyManager.FormatCurrency(cost)})";
        }

        #endregion
    }
}
