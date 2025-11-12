using UnityEngine;
using System;
using System.IO;

namespace VikingSiegeBreaker.Core
{
    /// <summary>
    /// Handles all save/load operations using PlayerPrefs and JSON serialization.
    /// Saves: currencies, upgrades, evolutions, settings, and stats.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 60f; // seconds

        private float autoSaveTimer = 0f;
        private const string SAVE_KEY = "VSB_SaveData";

        // PlayerPrefs Keys (documented for reference)
        public const string KEY_NO_ADS_PURCHASED = "NoAdsPurchased";
        public const string KEY_FIRST_LAUNCH = "FirstLaunch";
        public const string KEY_MUSIC_VOLUME = "MusicVolume";
        public const string KEY_SFX_VOLUME = "SFXVolume";
        public const string KEY_TOTAL_PLAYTIME = "TotalPlaytime";

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

        private void Update()
        {
            // Auto-save timer
            if (autoSaveEnabled)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    SaveGame();
                }
            }
        }

        #region Save/Load

        /// <summary>
        /// Saves all game data to PlayerPrefs as JSON.
        /// </summary>
        public void SaveGame()
        {
            try
            {
                SaveData data = new SaveData
                {
                    // Currencies
                    coins = Systems.CurrencyManager.Instance.Coins,
                    gems = Systems.CurrencyManager.Instance.Gems,
                    xp = Systems.CurrencyManager.Instance.XP,
                    currentEra = Systems.EvolutionManager.Instance.CurrentEra,

                    // Upgrades (get levels from UpgradeManager)
                    launchPowerLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("LaunchPower"),
                    maxHealthLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("MaxHealth"),
                    momentumDecayLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("MomentumDecay"),
                    critChanceLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("CritChance"),
                    critDamageLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("CritDamage"),
                    coinMultiplierLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("CoinMultiplier"),
                    xpMultiplierLevel = Systems.UpgradeManager.Instance.GetUpgradeLevel("XPMultiplier"),

                    // Stats
                    totalDistance = PlayerPrefs.GetFloat("TotalDistance", 0f),
                    totalCoins = PlayerPrefs.GetInt("TotalCoins", 0),
                    totalEnemies = PlayerPrefs.GetInt("TotalEnemies", 0),
                    totalRuns = PlayerPrefs.GetInt("TotalRuns", 0),
                    bestDistance = PlayerPrefs.GetFloat("BestDistance", 0f),

                    // Metadata
                    saveVersion = 1,
                    lastSaveTime = DateTime.Now.ToString("o") // ISO 8601 format
                };

                string json = JsonUtility.ToJson(data, true);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();

                Debug.Log($"[SaveSystem] Game saved successfully at {DateTime.Now}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e.Message}");
            }
        }

        /// <summary>
        /// Loads all game data from PlayerPrefs.
        /// </summary>
        public void LoadGame()
        {
            try
            {
                if (!PlayerPrefs.HasKey(SAVE_KEY))
                {
                    Debug.Log("[SaveSystem] No save data found - starting fresh");
                    InitializeNewGame();
                    return;
                }

                string json = PlayerPrefs.GetString(SAVE_KEY);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // Restore currencies
                Systems.CurrencyManager.Instance.LoadData(data.coins, data.gems, data.xp);

                // Restore upgrades
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("LaunchPower", data.launchPowerLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("MaxHealth", data.maxHealthLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("MomentumDecay", data.momentumDecayLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("CritChance", data.critChanceLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("CritDamage", data.critDamageLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("CoinMultiplier", data.coinMultiplierLevel);
                Systems.UpgradeManager.Instance.LoadUpgradeLevel("XPMultiplier", data.xpMultiplierLevel);

                // Restore evolution
                Systems.EvolutionManager.Instance.LoadEra(data.currentEra);

                Debug.Log($"[SaveSystem] Game loaded successfully (Version {data.saveVersion})");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed: {e.Message}");
                InitializeNewGame();
            }
        }

        #endregion

        #region New Game

        /// <summary>
        /// Initializes a fresh game with default values.
        /// </summary>
        private void InitializeNewGame()
        {
            Debug.Log("[SaveSystem] Initializing new game");

            // Set first launch flag
            PlayerPrefs.SetInt(KEY_FIRST_LAUNCH, 1);

            // Default audio settings
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, 0.7f);
            PlayerPrefs.SetFloat(KEY_SFX_VOLUME, 1.0f);

            // Initialize currencies (starts at 0, handled by CurrencyManager)
            // Initialize upgrades (starts at level 0, handled by UpgradeManager)

            SaveGame();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Deletes all save data (for debug/testing).
        /// </summary>
        public void DeleteSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[SaveSystem] All save data deleted");
        }

        /// <summary>
        /// Exports save data as JSON string (for cloud save or debugging).
        /// </summary>
        public string ExportSaveData()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                return PlayerPrefs.GetString(SAVE_KEY);
            }
            return null;
        }

        /// <summary>
        /// Imports save data from JSON string (for cloud save restore).
        /// </summary>
        public void ImportSaveData(string json)
        {
            try
            {
                // Validate JSON
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                LoadGame();
                Debug.Log("[SaveSystem] Save data imported successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Import failed: {e.Message}");
            }
        }

        /// <summary>
        /// Checks if this is the first time the game is launched.
        /// </summary>
        public bool IsFirstLaunch()
        {
            return PlayerPrefs.GetInt(KEY_FIRST_LAUNCH, 1) == 1;
        }

        /// <summary>
        /// Marks first launch as complete.
        /// </summary>
        public void CompleteFirstLaunch()
        {
            PlayerPrefs.SetInt(KEY_FIRST_LAUNCH, 0);
            PlayerPrefs.Save();
        }

        #endregion

        #region Stats Tracking

        /// <summary>
        /// Updates lifetime statistics.
        /// </summary>
        public void UpdateStats(float distance, int coins, int enemies)
        {
            // Update totals
            float totalDistance = PlayerPrefs.GetFloat("TotalDistance", 0f);
            PlayerPrefs.SetFloat("TotalDistance", totalDistance + distance);

            int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
            PlayerPrefs.SetInt("TotalCoins", totalCoins + coins);

            int totalEnemies = PlayerPrefs.GetInt("TotalEnemies", 0);
            PlayerPrefs.SetInt("TotalEnemies", totalEnemies + enemies);

            int totalRuns = PlayerPrefs.GetInt("TotalRuns", 0);
            PlayerPrefs.SetInt("TotalRuns", totalRuns + 1);

            // Update best distance
            float bestDistance = PlayerPrefs.GetFloat("BestDistance", 0f);
            if (distance > bestDistance)
            {
                PlayerPrefs.SetFloat("BestDistance", distance);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the best distance record.
        /// </summary>
        public float GetBestDistance()
        {
            return PlayerPrefs.GetFloat("BestDistance", 0f);
        }

        #endregion
    }

    /// <summary>
    /// Serializable data structure for save/load.
    /// Add new fields here as game expands.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // Currencies
        public int coins;
        public int gems;
        public int xp;
        public int currentEra;

        // Upgrades (store levels only, data comes from ScriptableObjects)
        public int launchPowerLevel;
        public int maxHealthLevel;
        public int momentumDecayLevel;
        public int critChanceLevel;
        public int critDamageLevel;
        public int coinMultiplierLevel;
        public int xpMultiplierLevel;

        // Lifetime Stats
        public float totalDistance;
        public int totalCoins;
        public int totalEnemies;
        public int totalRuns;
        public float bestDistance;

        // Metadata
        public int saveVersion;
        public string lastSaveTime;
    }
}
