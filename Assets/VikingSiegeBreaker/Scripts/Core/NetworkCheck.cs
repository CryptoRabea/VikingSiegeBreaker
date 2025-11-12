using UnityEngine;
using System;
using System.Collections;

namespace VikingSiegeBreaker.Core
{
    /// <summary>
    /// Monitors network connectivity and enforces offline-lock logic:
    /// - Game requires internet by default
    /// - If offline AND "No Ads" NOT purchased: block gameplay, show purchase popup
    /// - If offline AND "No Ads" purchased: allow offline play
    /// </summary>
    public class NetworkCheck : MonoBehaviour
    {
        public static NetworkCheck Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float checkInterval = 5f; // How often to check connection
        [SerializeField] private bool requireInternetForPlay = true; // Can disable for testing

        [Header("Debug")]
        [SerializeField] private bool isConnected = true;
        [SerializeField] private bool hasNoAdsPurchased = false;

        // Events
        public event Action OnConnectionLost;
        public event Action OnConnectionRestored;
        public event Action OnOfflinePlayBlocked; // Trigger purchase popup

        private Coroutine checkRoutine;

        public bool IsConnected => isConnected;
        public bool CanPlayOffline => hasNoAdsPurchased;

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
            // Check if "No Ads" is purchased
            hasNoAdsPurchased = PlayerPrefs.GetInt(SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;

            // Start monitoring
            CheckConnection();
            StartMonitoring();
        }

        #region Connection Monitoring

        /// <summary>
        /// Starts periodic connection checking.
        /// </summary>
        public void StartMonitoring()
        {
            if (checkRoutine != null)
            {
                StopCoroutine(checkRoutine);
            }
            checkRoutine = StartCoroutine(MonitorConnection());
        }

        /// <summary>
        /// Stops connection monitoring.
        /// </summary>
        public void StopMonitoring()
        {
            if (checkRoutine != null)
            {
                StopCoroutine(checkRoutine);
                checkRoutine = null;
            }
        }

        private IEnumerator MonitorConnection()
        {
            while (true)
            {
                CheckConnection();
                yield return new WaitForSeconds(checkInterval);
            }
        }

        /// <summary>
        /// Checks current network status.
        /// </summary>
        public void CheckConnection()
        {
            bool wasConnected = isConnected;

            // Check Unity's network reachability
            isConnected = Application.internetReachability != NetworkReachability.NotReachable;

            // Handle state changes
            if (wasConnected && !isConnected)
            {
                OnConnectionLost?.Invoke();
                Debug.LogWarning("[NetworkCheck] Connection lost");
            }
            else if (!wasConnected && isConnected)
            {
                OnConnectionRestored?.Invoke();
                Debug.Log("[NetworkCheck] Connection restored");
            }
        }

        #endregion

        #region Offline Play Logic

        /// <summary>
        /// Validates if the player can start a game session.
        /// Returns true if allowed, false if blocked (offline + no IAP).
        /// </summary>
        public bool ValidatePlayPermission()
        {
            // If internet not required (debug mode), always allow
            if (!requireInternetForPlay)
            {
                return true;
            }

            // If connected, always allow
            if (isConnected)
            {
                return true;
            }

            // If offline but "No Ads" purchased, allow
            if (hasNoAdsPurchased)
            {
                Debug.Log("[NetworkCheck] Offline play allowed (No Ads purchased)");
                return true;
            }

            // Offline and no purchase - BLOCK
            Debug.LogWarning("[NetworkCheck] Offline play blocked - internet required or purchase 'Remove Ads'");
            OnOfflinePlayBlocked?.Invoke();
            return false;
        }

        /// <summary>
        /// Call this when "Remove Ads" is purchased to update permission.
        /// </summary>
        public void OnNoAdsPurchased()
        {
            hasNoAdsPurchased = true;
            PlayerPrefs.SetInt(SaveSystem.KEY_NO_ADS_PURCHASED, 1);
            PlayerPrefs.Save();
            Debug.Log("[NetworkCheck] No Ads purchased - offline play now enabled");
        }

        /// <summary>
        /// Gets a message to display to the user when blocked.
        /// </summary>
        public string GetBlockedMessage()
        {
            return "Internet connection required to play.\n\n" +
                   "Purchase 'Remove Ads' to enable offline play!";
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Returns a user-friendly connection status string.
        /// </summary>
        public string GetConnectionStatusText()
        {
            if (isConnected)
            {
                return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork
                    ? "Mobile Data"
                    : "WiFi";
            }
            return "Offline";
        }

        /// <summary>
        /// Force refresh of "No Ads" status (call after IAP restore).
        /// </summary>
        public void RefreshPurchaseStatus()
        {
            hasNoAdsPurchased = PlayerPrefs.GetInt(SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;
            Debug.Log($"[NetworkCheck] Purchase status refreshed: {hasNoAdsPurchased}");
        }

        #endregion

        #region Debug Helpers

        /// <summary>
        /// Simulates connection loss (for testing).
        /// </summary>
        [ContextMenu("Simulate Connection Loss")]
        public void SimulateConnectionLoss()
        {
            isConnected = false;
            OnConnectionLost?.Invoke();
            Debug.Log("[NetworkCheck] SIMULATED connection loss");
        }

        /// <summary>
        /// Simulates connection restore (for testing).
        /// </summary>
        [ContextMenu("Simulate Connection Restore")]
        public void SimulateConnectionRestore()
        {
            isConnected = true;
            OnConnectionRestored?.Invoke();
            Debug.Log("[NetworkCheck] SIMULATED connection restore");
        }

        /// <summary>
        /// Toggle internet requirement (for testing).
        /// </summary>
        public void SetInternetRequired(bool required)
        {
            requireInternetForPlay = required;
            Debug.Log($"[NetworkCheck] Internet required: {required}");
        }

        #endregion
    }
}
