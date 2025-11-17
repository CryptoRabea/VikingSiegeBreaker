using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// In-App Purchase popup - displays IAP product information and purchase options.
    /// Shows product details and allows player to confirm or cancel purchase.
    /// </summary>
    public class IAPPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI productNameText;
        [SerializeField] private TextMeshProUGUI productDescriptionText;
        [SerializeField] private TextMeshProUGUI productPriceText;
        [SerializeField] private Image productIcon;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameObject panel;

        private string currentProductId;

        private void Start()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.AddListener(OnPurchaseClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }

        /// <summary>
        /// Show the IAP popup with the specified product ID.
        /// </summary>
        public void Show(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                Debug.LogWarning("IAPPopup: Attempted to show popup with null/empty product ID");
                return;
            }

            currentProductId = productId;

            // Update UI elements based on product ID
            // This would typically fetch product info from IAP Manager
            UpdateProductDisplay(productId);

            // Show the panel
            if (panel != null)
            {
                panel.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hide the IAP popup.
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void UpdateProductDisplay(string productId)
        {
            // This is a placeholder - in a real implementation, you would fetch
            // product details from the IAP system (Unity IAP, etc.)

            if (productNameText != null)
            {
                productNameText.text = GetProductName(productId);
            }

            if (productDescriptionText != null)
            {
                productDescriptionText.text = GetProductDescription(productId);
            }

            if (productPriceText != null)
            {
                productPriceText.text = GetProductPrice(productId);
            }
        }

        private string GetProductName(string productId)
        {
            // Placeholder - replace with actual IAP Manager integration
            switch (productId)
            {
                case "remove_ads":
                    return "Remove Ads";
                case "coin_pack_small":
                    return "Small Coin Pack";
                case "coin_pack_medium":
                    return "Medium Coin Pack";
                case "coin_pack_large":
                    return "Large Coin Pack";
                case "starter_pack":
                    return "Starter Pack";
                default:
                    return "Product";
            }
        }

        private string GetProductDescription(string productId)
        {
            // Placeholder - replace with actual IAP Manager integration
            switch (productId)
            {
                case "remove_ads":
                    return "Remove all ads from the game permanently!";
                case "coin_pack_small":
                    return "Get 1000 coins to upgrade your viking!";
                case "coin_pack_medium":
                    return "Get 5000 coins to upgrade your viking!";
                case "coin_pack_large":
                    return "Get 15000 coins to upgrade your viking!";
                case "starter_pack":
                    return "Get coins and exclusive items to start strong!";
                default:
                    return "In-app purchase item";
            }
        }

        private string GetProductPrice(string productId)
        {
            // Placeholder - replace with actual IAP Manager integration
            switch (productId)
            {
                case "remove_ads":
                    return "$2.99";
                case "coin_pack_small":
                    return "$0.99";
                case "coin_pack_medium":
                    return "$2.99";
                case "coin_pack_large":
                    return "$4.99";
                case "starter_pack":
                    return "$9.99";
                default:
                    return "$?.??";
            }
        }

        private void OnPurchaseClicked()
        {
            // Initiate purchase through IAP Manager
            if (Managers.IAPManager.Instance != null)
            {
                Managers.IAPManager.Instance.PurchaseProduct(currentProductId);
            }
            else
            {
                Debug.LogWarning("IAPManager instance not found!");
            }

            Hide();
        }

        private void OnCancelClicked()
        {
            Hide();
        }

        private void OnDestroy()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(OnCancelClicked);
            }
        }
    }
}
