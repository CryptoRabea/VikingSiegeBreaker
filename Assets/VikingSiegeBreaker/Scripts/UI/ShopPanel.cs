using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Shop panel - shows IAP products and gem purchases.
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject panel;

        [Header("Display")]
        [SerializeField] private TextMeshProUGUI gemsText;

        [Header("IAP Products")]
        [SerializeField] private Button removeAdsButton;
        [SerializeField] private Button starterPackButton;

        private void Start()
        {
            UpdateDisplay();
            UpdateIAPButtons();
        }

        private void UpdateDisplay()
        {
            if (gemsText != null)
            {
                gemsText.text = Systems.CurrencyManager.Instance.Gems.ToString();
            }
        }

        private void UpdateIAPButtons()
        {
            // Check if Remove Ads is already purchased
            bool noAdsPurchased = PlayerPrefs.GetInt(Core.SaveSystem.KEY_NO_ADS_PURCHASED, 0) == 1;

            if (removeAdsButton != null)
            {
                var buttonText = removeAdsButton.GetComponentInChildren<TextMeshProUGUI>();
                if (noAdsPurchased)
                {
                    removeAdsButton.interactable = false;
                    if (buttonText != null) buttonText.text = "Purchased";
                }
                else
                {
                    if (buttonText != null) buttonText.text = $"Remove Ads\n${Data.GameSettings.Instance.removeAdsPrice:F2}";
                }
            }
        }

        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                UpdateDisplay();
                UpdateIAPButtons();
            }
        }

        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // Button callbacks
        public void OnRemoveAdsClicked()
        {
            Managers.IAPManager.Instance.PurchaseRemoveAds();
        }

        public void OnStarterPackClicked()
        {
            Managers.IAPManager.Instance.PurchaseStarterPack();
        }
    }
}
