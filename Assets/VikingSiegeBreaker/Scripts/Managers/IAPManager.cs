using UnityEngine;
using System;

// UNITY IAP: Uncomment when Unity IAP package is installed
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Extension;

namespace VikingSiegeBreaker.Managers
{
    /// <summary>
    /// IAP integration wrapper for Unity IAP.
    /// Handles in-app purchases including "Remove Ads" with offline-lock unlock.
    ///
    /// SETUP INSTRUCTIONS:
    /// 1. Import Unity IAP package: Window > Package Manager > In App Purchasing
    /// 2. Enable IAP: Services > In-App Purchasing > Enable
    /// 3. Uncomment Unity IAP code below (marked with // UNITY IAP:)
    /// 4. Uncomment the IStoreListener interface implementation
    /// 5. Configure products in Google Play Console / App Store Connect
    /// 6. Test with sandbox accounts
    /// </summary>
    public class IAPManager : MonoBehaviour // UNITY IAP: Add: , IStoreListener
    {
        public static IAPManager Instance { get; private set; }

        [Header("Product IDs")]
        [SerializeField] private string removeAdsProductId = "com.yourgame.vikingsiegebreaker.removeads";
        [SerializeField] private string starterPackProductId = "com.yourgame.vikingsiegebreaker.starterpack";
        [SerializeField] private string gemPack100ProductId = "com.yourgame.vikingsiegebreaker.gems100";
        [SerializeField] private string gemPack500ProductId = "com.yourgame.vikingsiegebreaker.gems500";
        [SerializeField] private string gemPack1000ProductId = "com.yourgame.vikingsiegebreaker.gems1000";

        [Header("State")]
        [SerializeField] private bool isInitialized = false;
        [SerializeField] private bool isPurchasing = false;

        // UNITY IAP: Uncomment these
        // private IStoreController storeController;
        // private IExtensionProvider extensionProvider;

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
            InitializeIAP();
        }

        #region Initialization

        /// <summary>
        /// Initializes Unity IAP.
        /// </summary>
        private void InitializeIAP()
        {
            if (isInitialized)
            {
                Debug.Log("[IAPManager] Already initialized");
                return;
            }

            Debug.Log("[IAPManager] Initializing Unity IAP...");

            // UNITY IAP: Initialize
            /*
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add products
            builder.AddProduct(removeAdsProductId, ProductType.NonConsumable);
            builder.AddProduct(starterPackProductId, ProductType.NonConsumable);
            builder.AddProduct(gemPack100ProductId, ProductType.Consumable);
            builder.AddProduct(gemPack500ProductId, ProductType.Consumable);
            builder.AddProduct(gemPack1000ProductId, ProductType.Consumable);

            // Initialize
            UnityPurchasing.Initialize(this, builder);
            */

            // Test mode (without Unity IAP package)
            isInitialized = true;
            Debug.Log("[IAPManager] Initialized (test mode)");

            // Check for pending purchases (restore)
            RestorePurchases();
        }

        #endregion

        #region Purchase Methods

        /// <summary>
        /// Purchases the "Remove Ads" product.
        /// CRITICAL: Sets PlayerPrefs flag to unlock offline play.
        /// </summary>
        public void PurchaseRemoveAds()
        {
            if (isPurchasing)
            {
                Debug.LogWarning("[IAPManager] Purchase already in progress");
                return;
            }

            Debug.Log("[IAPManager] Initiating Remove Ads purchase...");

            // UNITY IAP: Initiate purchase
            // BuyProductID(removeAdsProductId);

            // Test mode: Simulate successful purchase
            SimulatePurchase(removeAdsProductId);
        }

        /// <summary>
        /// Purchases the starter pack (gems + no ads).
        /// </summary>
        public void PurchaseStarterPack()
        {
            if (isPurchasing) return;

            Debug.Log("[IAPManager] Initiating Starter Pack purchase...");

            // UNITY IAP: Initiate purchase
            // BuyProductID(starterPackProductId);

            SimulatePurchase(starterPackProductId);
        }

        /// <summary>
        /// Purchases a gem pack.
        /// </summary>
        public void PurchaseGemPack(int amount)
        {
            if (isPurchasing) return;

            string productId = amount switch
            {
                100 => gemPack100ProductId,
                500 => gemPack500ProductId,
                1000 => gemPack1000ProductId,
                _ => gemPack100ProductId
            };

            Debug.Log($"[IAPManager] Initiating Gem Pack purchase ({amount} gems)...");

            // UNITY IAP: Initiate purchase
            // BuyProductID(productId);

            SimulatePurchase(productId);
        }

        // UNITY IAP: Uncomment this method
        /*
        private void BuyProductID(string productId)
        {
            if (!isInitialized)
            {
                Debug.LogError("[IAPManager] IAP not initialized");
                return;
            }

            if (storeController == null || extensionProvider == null)
            {
                Debug.LogError("[IAPManager] Store controller not available");
                return;
            }

            Product product = storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"[IAPManager] Purchasing: {product.definition.id}");
                isPurchasing = true;
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogError($"[IAPManager] Product not available: {productId}");
            }
        }
        */

        #endregion

        #region Purchase Processing

        /// <summary>
        /// Processes a successful purchase.
        /// CRITICAL: For "Remove Ads", sets PlayerPrefs flag and enables offline play.
        /// </summary>
        private void ProcessPurchase(string productId)
        {
            Debug.Log($"[IAPManager] Processing purchase: {productId}");

            if (productId == removeAdsProductId)
            {
                // CRITICAL: Set the offline play unlock flag
                PlayerPrefs.SetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 1);
                PlayerPrefs.Save();

                // Disable ads
                AdsManager.Instance.SetAdsEnabled(false);

                // Update network check
                Core.NetworkCheck.Instance.OnNoAdsPurchased();

                Debug.Log("[IAPManager] Remove Ads purchased - Offline play ENABLED");
                Managers.UIManager.Instance?.ShowNotification("Ads Removed! Offline play enabled.");
            }
            else if (productId == starterPackProductId)
            {
                // Grant starter pack contents
                PlayerPrefs.SetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 1);
                PlayerPrefs.Save();

                Systems.CurrencyManager.Instance.AddGems(Data.GameSettings.Instance.starterPackGems);
                AdsManager.Instance.SetAdsEnabled(false);
                Core.NetworkCheck.Instance.OnNoAdsPurchased();

                Debug.Log("[IAPManager] Starter Pack purchased");
                Managers.UIManager.Instance?.ShowNotification($"Starter Pack claimed! +{Data.GameSettings.Instance.starterPackGems} gems");
            }
            else if (productId == gemPack100ProductId)
            {
                Systems.CurrencyManager.Instance.AddGems(100);
                Managers.UIManager.Instance?.ShowNotification("+100 Gems!");
            }
            else if (productId == gemPack500ProductId)
            {
                Systems.CurrencyManager.Instance.AddGems(500);
                Managers.UIManager.Instance?.ShowNotification("+500 Gems!");
            }
            else if (productId == gemPack1000ProductId)
            {
                Systems.CurrencyManager.Instance.AddGems(1000);
                Managers.UIManager.Instance?.ShowNotification("+1000 Gems!");
            }

            // Save progress
            Core.SaveSystem.Instance.SaveGame();

            isPurchasing = false;
        }

        #endregion

        #region Restore Purchases

        /// <summary>
        /// Restores non-consumable purchases (for iOS requirement).
        /// </summary>
        public void RestorePurchases()
        {
            Debug.Log("[IAPManager] Restoring purchases...");

            // UNITY IAP: Restore purchases (iOS only)
            /*
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (extensionProvider != null)
                {
                    var apple = extensionProvider.GetExtension<IAppleExtensions>();
                    apple.RestoreTransactions((result) =>
                    {
                        Debug.Log($"[IAPManager] Restore result: {result}");
                    });
                }
            }
            */

            // Check PlayerPrefs for "Remove Ads" flag
            bool noAdsPurchased = PlayerPrefs.GetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;
            if (noAdsPurchased)
            {
                Debug.Log("[IAPManager] Remove Ads already purchased");
                AdsManager.Instance.SetAdsEnabled(false);
                Core.NetworkCheck.Instance.RefreshPurchaseStatus();
            }
        }

        #endregion

        #region IStoreListener Implementation (UNITY IAP)

        // UNITY IAP: Uncomment these methods when Unity IAP is integrated

        /*
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("[IAPManager] Unity IAP initialized successfully");
            storeController = controller;
            extensionProvider = extensions;
            isInitialized = true;

            // Check for pending purchases
            RestorePurchases();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"[IAPManager] Unity IAP initialization failed: {error}");
            isInitialized = false;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"[IAPManager] Unity IAP initialization failed: {error} - {message}");
            isInitialized = false;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log($"[IAPManager] Purchase successful: {args.purchasedProduct.definition.id}");

            ProcessPurchase(args.purchasedProduct.definition.id);

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"[IAPManager] Purchase failed: {product.definition.id} - Reason: {failureReason}");
            isPurchasing = false;

            Managers.UIManager.Instance?.ShowErrorMessage($"Purchase failed: {failureReason}");
        }
        */

        #endregion

        #region Test Mode

        /// <summary>
        /// Simulates a purchase (for testing without Unity IAP).
        /// </summary>
        private void SimulatePurchase(string productId)
        {
            Debug.Log($"[IAPManager] TEST MODE: Simulating purchase of {productId}");
            isPurchasing = true;

            // Simulate delay
            Invoke(nameof(CompleteSimulatedPurchase), 1f);
            currentSimulatedProductId = productId;
        }

        private string currentSimulatedProductId;

        private void CompleteSimulatedPurchase()
        {
            ProcessPurchase(currentSimulatedProductId);
            currentSimulatedProductId = null;
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Checks if "Remove Ads" is purchased.
        /// </summary>
        public bool IsRemoveAdsPurchased()
        {
            return PlayerPrefs.GetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;
        }

        /// <summary>
        /// Gets the price string for a product (requires Unity IAP).
        /// </summary>
        public string GetProductPrice(string productId)
        {
            // UNITY IAP: Get price from store
            /*
            if (storeController != null)
            {
                Product product = storeController.products.WithID(productId);
                if (product != null)
                {
                    return product.metadata.localizedPriceString;
                }
            }
            */

            // Fallback
            return "$?.??";
        }

        #endregion
    }
}
