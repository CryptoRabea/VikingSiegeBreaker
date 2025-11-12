using UnityEngine;

namespace VikingSiegeBreaker.Utilities
{
    /// <summary>
    /// Debug/cheat tools for testing and QA.
    /// Enable via Data.GameSettings.enableDebugMode or build with DEVELOPMENT_BUILD.
    /// </summary>
    public class DebugTools : MonoBehaviour
    {
        public static DebugTools Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool enableCheats = false;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;

        [Header("UI")]
        [SerializeField] private bool showDebugUI = false;
        [SerializeField] private Rect windowRect = new Rect(20, 20, 300, 500);

        private Vector2 scrollPosition;

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
            // Enable cheats in development builds or if set in GameSettings
            enableCheats = Debug.isDebugBuild ||
                          (Data.GameSettings.Instance != null && Data.GameSettings.Instance.enableDebugMode);

            if (!enableCheats)
            {
                enabled = false;
                Debug.Log("[DebugTools] Disabled (not in debug mode)");
            }
        }

        private void Update()
        {
            if (!enableCheats) return;

            // Toggle debug UI
            if (Input.GetKeyDown(toggleKey))
            {
                showDebugUI = !showDebugUI;
            }

            // Quick cheats (keyboard shortcuts)
            if (Input.GetKeyDown(KeyCode.F2)) CheatAddCoins(1000);
            if (Input.GetKeyDown(KeyCode.F3)) CheatAddGems(100);
            if (Input.GetKeyDown(KeyCode.F4)) CheatAddXP(1000);
            if (Input.GetKeyDown(KeyCode.F5)) CheatMaxAllUpgrades();
            if (Input.GetKeyDown(KeyCode.F6)) CheatEvolveToMax();
        }

        #region Debug UI

        private void OnGUI()
        {
            if (!enableCheats || !showDebugUI) return;

            windowRect = GUILayout.Window(0, windowRect, DrawDebugWindow, "Viking Siege Breaker - Debug Tools");
        }

        private void DrawDebugWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("=== CURRENCY ===");
            if (GUILayout.Button("+1000 Coins (F2)")) CheatAddCoins(1000);
            if (GUILayout.Button("+100 Gems (F3)")) CheatAddGems(100);
            if (GUILayout.Button("+1000 XP (F4)")) CheatAddXP(1000);

            GUILayout.Space(10);
            GUILayout.Label("=== UPGRADES ===");
            if (GUILayout.Button("Max All Upgrades (F5)")) CheatMaxAllUpgrades();
            if (GUILayout.Button("Reset All Upgrades")) CheatResetAllUpgrades();

            GUILayout.Space(10);
            GUILayout.Label("=== EVOLUTION ===");
            if (GUILayout.Button("Evolve to Max (F6)")) CheatEvolveToMax();
            if (GUILayout.Button("Reset to Era 1")) CheatResetEra();

            GUILayout.Space(10);
            GUILayout.Label("=== GAMEPLAY ===");
            if (GUILayout.Button("Add 50 Momentum")) CheatAddMomentum(50);
            if (GUILayout.Button("Heal Player")) CheatHealPlayer();
            if (GUILayout.Button("Kill All Enemies")) CheatKillAllEnemies();

            GUILayout.Space(10);
            GUILayout.Label("=== SAVE/LOAD ===");
            if (GUILayout.Button("Save Game")) Core.SaveSystem.Instance.SaveGame();
            if (GUILayout.Button("Load Game")) Core.SaveSystem.Instance.LoadGame();
            if (GUILayout.Button("Delete Save (CAUTION!)")) CheatDeleteSave();

            GUILayout.Space(10);
            GUILayout.Label("=== IAP/ADS ===");
            if (GUILayout.Button("Simulate Remove Ads Purchase")) CheatPurchaseRemoveAds();
            if (GUILayout.Button("Reset Remove Ads Purchase")) CheatResetRemoveAds();
            if (GUILayout.Button("Simulate Offline Mode")) Core.NetworkCheck.Instance.SimulateConnectionLoss();
            if (GUILayout.Button("Simulate Online Mode")) Core.NetworkCheck.Instance.SimulateConnectionRestore();

            GUILayout.Space(10);
            GUILayout.Label("=== SCENES ===");
            if (GUILayout.Button("Load Main Menu")) Core.GameManager.Instance.LoadMainMenu();
            if (GUILayout.Button("Load Gameplay")) Core.GameManager.Instance.LoadGameplay();

            GUILayout.Space(10);
            GUILayout.Label("=== INFO ===");
            GUILayout.Label($"State: {Core.GameManager.Instance.CurrentState}");
            GUILayout.Label($"Distance: {Core.GameManager.Instance.CurrentDistance:F2}m");
            GUILayout.Label($"Coins: {Systems.CurrencyManager.Instance.Coins}");
            GUILayout.Label($"Gems: {Systems.CurrencyManager.Instance.Gems}");
            GUILayout.Label($"XP: {Systems.CurrencyManager.Instance.XP}");
            GUILayout.Label($"Era: {Systems.EvolutionManager.Instance.CurrentEra + 1}");

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        #endregion

        #region Cheat Commands

        [ContextMenu("Add 1000 Coins")]
        public void CheatAddCoins(int amount)
        {
            if (!enableCheats) return;
            Systems.CurrencyManager.Instance.CheatAddCoins(amount);
            Debug.Log($"[DebugTools] Added {amount} coins");
        }

        [ContextMenu("Add 100 Gems")]
        public void CheatAddGems(int amount)
        {
            if (!enableCheats) return;
            Systems.CurrencyManager.Instance.CheatAddGems(amount);
            Debug.Log($"[DebugTools] Added {amount} gems");
        }

        [ContextMenu("Add 1000 XP")]
        public void CheatAddXP(int amount)
        {
            if (!enableCheats) return;
            Systems.CurrencyManager.Instance.CheatAddXP(amount);
            Debug.Log($"[DebugTools] Added {amount} XP");
        }

        [ContextMenu("Max All Upgrades")]
        public void CheatMaxAllUpgrades()
        {
            if (!enableCheats) return;
            Systems.UpgradeManager.Instance.CheatMaxAllUpgrades();
            Debug.Log("[DebugTools] Maxed all upgrades");
        }

        [ContextMenu("Reset All Upgrades")]
        public void CheatResetAllUpgrades()
        {
            if (!enableCheats) return;
            Systems.UpgradeManager.Instance.CheatResetAllUpgrades();
            Debug.Log("[DebugTools] Reset all upgrades");
        }

        [ContextMenu("Evolve to Max")]
        public void CheatEvolveToMax()
        {
            if (!enableCheats) return;
            Systems.EvolutionManager.Instance.CheatEvolveToMax();
            Debug.Log("[DebugTools] Evolved to max era");
        }

        [ContextMenu("Reset to Era 1")]
        public void CheatResetEra()
        {
            if (!enableCheats) return;
            Systems.EvolutionManager.Instance.CheatResetEra();
            Debug.Log("[DebugTools] Reset to Era 1");
        }

        public void CheatAddMomentum(float amount)
        {
            if (!enableCheats) return;
            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                var momentum = player.GetComponent<Player.MomentumSystem>();
                if (momentum != null)
                {
                    momentum.AddMomentum(amount);
                    Debug.Log($"[DebugTools] Added {amount} momentum");
                }
            }
        }

        public void CheatHealPlayer()
        {
            if (!enableCheats) return;
            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                // Note: Would need a public Heal method in PlayerController
                Debug.Log("[DebugTools] Heal player (not implemented)");
            }
        }

        public void CheatKillAllEnemies()
        {
            if (!enableCheats) return;
            var enemies = FindObjectsByType<Entities.Enemy>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.TakeDamage(9999f);
            }
            Debug.Log($"[DebugTools] Killed {enemies.Length} enemies");
        }

        public void CheatDeleteSave()
        {
            if (!enableCheats) return;
            Core.SaveSystem.Instance.DeleteSaveData();
            Debug.Log("[DebugTools] Save data deleted");
        }

        public void CheatPurchaseRemoveAds()
        {
            if (!enableCheats) return;
            PlayerPrefs.SetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 1);
            PlayerPrefs.Save();
            Core.NetworkCheck.Instance.OnNoAdsPurchased();
            Managers.AdsManager.Instance.SetAdsEnabled(false);
            Debug.Log("[DebugTools] Simulated Remove Ads purchase");
        }

        public void CheatResetRemoveAds()
        {
            if (!enableCheats) return;
            PlayerPrefs.SetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 0);
            PlayerPrefs.Save();
            Core.NetworkCheck.Instance.RefreshPurchaseStatus();
            Managers.AdsManager.Instance.SetAdsEnabled(true);
            Debug.Log("[DebugTools] Reset Remove Ads purchase");
        }

        #endregion

        #region Console Commands (can be called from Unity Console)

        /// <summary>
        /// Sets player coins directly.
        /// Usage: DebugTools.SetCoins(5000)
        /// </summary>
        public static void SetCoins(int amount)
        {
            Systems.CurrencyManager.Instance.CheatAddCoins(amount - Systems.CurrencyManager.Instance.Coins);
        }

        /// <summary>
        /// Sets player gems directly.
        /// </summary>
        public static void SetGems(int amount)
        {
            Systems.CurrencyManager.Instance.CheatAddGems(amount - Systems.CurrencyManager.Instance.Gems);
        }

        /// <summary>
        /// Sets player XP directly.
        /// </summary>
        public static void SetXP(int amount)
        {
            Systems.CurrencyManager.Instance.CheatAddXP(amount - Systems.CurrencyManager.Instance.XP);
        }

        #endregion
    }
}
