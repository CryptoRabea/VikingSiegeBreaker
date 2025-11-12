using UnityEngine;
using System;
using System.Collections.Generic;

namespace VikingSiegeBreaker.Systems
{
    /// <summary>
    /// Manages player evolution system - tracks XP, handles era progression, and visual upgrades.
    /// Each era unlocks new visuals, abilities, and stat bonuses.
    /// </summary>
    public class EvolutionManager : MonoBehaviour
    {
        public static EvolutionManager Instance { get; private set; }

        [Header("Evolution Data")]
        [SerializeField] private List<Data.EvolutionData> allEvolutions = new List<Data.EvolutionData>();

        [Header("Current State")]
        [SerializeField] private int currentEra = 0; // 0-indexed (0 = Era 1)
        [SerializeField] private int totalXP = 0;

        [Header("Era Thresholds")]
        [SerializeField] private int[] xpThresholds = new int[]
        {
            0,      // Era 1 (starting era)
            1000,   // Era 2
            5000,   // Era 3
            15000,  // Era 4
            40000,  // Era 5
            100000  // Era 6 (max)
        };

        // Events
        public event Action<int, Data.EvolutionData> OnEvolved; // newEra, evolutionData
        public event Action<int, int> OnXPGained; // currentXP, nextThreshold

        // Properties
        public int CurrentEra => currentEra;
        public int TotalXP => totalXP;
        public int MaxEra => xpThresholds.Length - 1;

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
            // Load evolution data from Resources if not assigned
            if (allEvolutions.Count == 0)
            {
                var evolutions = Resources.LoadAll<Data.EvolutionData>("ScriptableObjects/Evolutions");
                allEvolutions.AddRange(evolutions);

                // Sort by era
                allEvolutions.Sort((a, b) => a.era.CompareTo(b.era));
            }

            Debug.Log($"[EvolutionManager] Initialized with {allEvolutions.Count} evolutions");
        }

        #region Evolution Checking

        /// <summary>
        /// Checks if the player should evolve based on current XP.
        /// Called by CurrencyManager when XP is gained.
        /// </summary>
        public void CheckForEvolution()
        {
            totalXP = CurrencyManager.Instance.XP;

            // Check if we can evolve
            for (int i = currentEra + 1; i < xpThresholds.Length; i++)
            {
                if (totalXP >= xpThresholds[i])
                {
                    EvolveToEra(i);
                }
                else
                {
                    break; // Stop at first threshold not met
                }
            }
        }

        /// <summary>
        /// Evolves the player to a new era.
        /// </summary>
        private void EvolveToEra(int newEra)
        {
            if (newEra <= currentEra || newEra >= xpThresholds.Length)
            {
                return;
            }

            currentEra = newEra;
            var evolutionData = GetEvolutionData(currentEra);

            Debug.Log($"[EvolutionManager] EVOLVED to Era {currentEra + 1}: {evolutionData?.displayName}");

            // Apply evolution effects
            ApplyEvolutionEffects(evolutionData);

            // Notify listeners
            OnEvolved?.Invoke(currentEra, evolutionData);

            // Show evolution popup (handled by UIManager)
            Managers.UIManager.Instance?.ShowEvolutionPopup(evolutionData);

            // Save
            Core.SaveSystem.Instance.SaveGame();
        }

        #endregion

        #region Evolution Effects

        /// <summary>
        /// Applies stat bonuses and visual changes for an evolution.
        /// </summary>
        private void ApplyEvolutionEffects(Data.EvolutionData evolution)
        {
            if (evolution == null) return;

            // Apply stat bonuses (these are permanent boosts)
            // These would be applied to player stats in PlayerController/MomentumSystem

            Debug.Log($"[EvolutionManager] Applied bonuses: " +
                      $"HP +{evolution.healthBonus}, " +
                      $"DMG +{evolution.damageBonus}%, " +
                      $"SPD +{evolution.speedBonus}%");

            // Visual swap handled by PlayerController or separate visual manager
            SwapPlayerVisuals(evolution);
        }

        /// <summary>
        /// Swaps player visuals/prefab for the new era.
        /// </summary>
        private void SwapPlayerVisuals(Data.EvolutionData evolution)
        {
            if (evolution.playerPrefab == null) return;

            // Find current player instance
            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                // Store current position and state
                Vector3 pos = player.transform.position;
                Quaternion rot = player.transform.rotation;

                // Destroy old player
                Destroy(player.gameObject);

                // Instantiate new player prefab
                GameObject newPlayer = Instantiate(evolution.playerPrefab, pos, rot);

                Debug.Log($"[EvolutionManager] Swapped player visual to {evolution.displayName}");
            }
        }

        #endregion

        #region Progress Tracking

        /// <summary>
        /// Gets XP progress to next era (0-1 normalized).
        /// </summary>
        public float GetProgressToNextEra()
        {
            if (currentEra >= MaxEra) return 1f; // Max era

            int currentThreshold = xpThresholds[currentEra];
            int nextThreshold = xpThresholds[currentEra + 1];
            int xpInEra = totalXP - currentThreshold;
            int xpNeeded = nextThreshold - currentThreshold;

            return Mathf.Clamp01((float)xpInEra / xpNeeded);
        }

        /// <summary>
        /// Gets XP needed for next era.
        /// </summary>
        public int GetXPToNextEra()
        {
            if (currentEra >= MaxEra) return 0;

            int nextThreshold = xpThresholds[currentEra + 1];
            return Mathf.Max(0, nextThreshold - totalXP);
        }

        /// <summary>
        /// Gets the XP threshold for a specific era.
        /// </summary>
        public int GetEraThreshold(int era)
        {
            if (era < 0 || era >= xpThresholds.Length) return 0;
            return xpThresholds[era];
        }

        /// <summary>
        /// Checks if player is at max era.
        /// </summary>
        public bool IsMaxEra()
        {
            return currentEra >= MaxEra;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Gets evolution data for a specific era.
        /// </summary>
        public Data.EvolutionData GetEvolutionData(int era)
        {
            return allEvolutions.Find(e => e.era == era);
        }

        /// <summary>
        /// Gets current evolution data.
        /// </summary>
        public Data.EvolutionData GetCurrentEvolution()
        {
            return GetEvolutionData(currentEra);
        }

        /// <summary>
        /// Gets all evolution data.
        /// </summary>
        public List<Data.EvolutionData> GetAllEvolutions()
        {
            return allEvolutions;
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Loads era from save (called by SaveSystem).
        /// </summary>
        public void LoadEra(int loadedEra)
        {
            currentEra = Mathf.Clamp(loadedEra, 0, MaxEra);
            totalXP = CurrencyManager.Instance.XP;

            Debug.Log($"[EvolutionManager] Loaded Era {currentEra + 1}");

            // Apply evolution effects without triggering popup
            var evolutionData = GetEvolutionData(currentEra);
            if (evolutionData != null)
            {
                ApplyEvolutionEffects(evolutionData);
            }
        }

        #endregion

        #region Display Helpers

        /// <summary>
        /// Gets formatted display text for current era.
        /// Example: "Era 2: Berserker Chief"
        /// </summary>
        public string GetCurrentEraDisplayText()
        {
            var evolution = GetCurrentEvolution();
            if (evolution != null)
            {
                return $"Era {currentEra + 1}: {evolution.displayName}";
            }
            return $"Era {currentEra + 1}";
        }

        /// <summary>
        /// Gets formatted progress text.
        /// Example: "1,234 / 5,000 XP"
        /// </summary>
        public string GetProgressDisplayText()
        {
            if (IsMaxEra())
            {
                return "MAX ERA";
            }

            int nextThreshold = xpThresholds[currentEra + 1];
            return $"{CurrencyManager.FormatCurrency(totalXP)} / {CurrencyManager.FormatCurrency(nextThreshold)} XP";
        }

        #endregion

        #region Debug/Cheats

        /// <summary>
        /// Forces evolution to next era (for testing).
        /// </summary>
        [ContextMenu("Force Evolve")]
        public void CheatForceEvolve()
        {
            if (currentEra < MaxEra)
            {
                EvolveToEra(currentEra + 1);
            }
        }

        /// <summary>
        /// Evolves to max era (for testing).
        /// </summary>
        [ContextMenu("Evolve to Max")]
        public void CheatEvolveToMax()
        {
            EvolveToEra(MaxEra);
        }

        /// <summary>
        /// Resets to Era 1 (for testing).
        /// </summary>
        [ContextMenu("Reset to Era 1")]
        public void CheatResetEra()
        {
            currentEra = 0;
            Debug.Log("[EvolutionManager] CHEAT: Reset to Era 1");
        }

        #endregion
    }
}
