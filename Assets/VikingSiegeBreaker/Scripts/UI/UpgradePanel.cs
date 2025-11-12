using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Upgrade panel - shows all available upgrades and allows purchasing.
    /// Dynamically generates upgrade UI elements from UpgradeData.
    /// </summary>
    public class UpgradePanel : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject panel;

        [Header("Display")]
        [SerializeField] private TextMeshProUGUI coinsText;

        [Header("Upgrade List")]
        [SerializeField] private Transform upgradeContainer;
        [SerializeField] private GameObject upgradeItemPrefab;

        [Header("Categories")]
        [SerializeField] private Button combatButton;
        [SerializeField] private Button economyButton;
        [SerializeField] private Button utilityButton;

        private string currentCategory = "All";
        private List<UpgradeItemUI> upgradeItems = new List<UpgradeItemUI>();

        private void Start()
        {
            GenerateUpgradeList();
            UpdateDisplay();

            // Subscribe to purchase events
            Systems.UpgradeManager.Instance.OnUpgradePurchased += OnUpgradePurchased;
        }

        private void GenerateUpgradeList()
        {
            if (upgradeContainer == null || upgradeItemPrefab == null) return;

            // Clear existing
            foreach (var item in upgradeItems)
            {
                if (item != null) Destroy(item.gameObject);
            }
            upgradeItems.Clear();

            // Get all upgrades
            var allUpgrades = Systems.UpgradeManager.Instance.GetAllUpgrades();

            foreach (var upgrade in allUpgrades)
            {
                if (upgrade == null) continue;

                // Instantiate UI item
                GameObject itemObj = Instantiate(upgradeItemPrefab, upgradeContainer);
                UpgradeItemUI item = itemObj.GetComponent<UpgradeItemUI>();

                if (item != null)
                {
                    item.Initialize(upgrade);
                    upgradeItems.Add(item);
                }
            }
        }

        private void OnUpgradePurchased(string upgradeName, int newLevel)
        {
            UpdateDisplay();
            RefreshUpgradeItems();
        }

        private void RefreshUpgradeItems()
        {
            foreach (var item in upgradeItems)
            {
                if (item != null) item.Refresh();
            }
        }

        private void UpdateDisplay()
        {
            if (coinsText != null)
            {
                coinsText.text = Systems.CurrencyManager.FormatCurrency(Systems.CurrencyManager.Instance.Coins);
            }
        }

        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                UpdateDisplay();
                RefreshUpgradeItems();
            }
        }

        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // Category filtering
        public void FilterCategory(string category)
        {
            currentCategory = category;
            // TODO: Implement filtering logic
        }

        private void OnDestroy()
        {
            if (Systems.UpgradeManager.Instance != null)
            {
                Systems.UpgradeManager.Instance.OnUpgradePurchased -= OnUpgradePurchased;
            }
        }
    }

    /// <summary>
    /// Individual upgrade item UI element (assign to prefab).
    /// </summary>
    public class UpgradeItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Image icon;

        private Data.UpgradeData upgradeData;

        public void Initialize(Data.UpgradeData data)
        {
            upgradeData = data;
            Refresh();

            // Hook up purchase button
            if (purchaseButton != null)
            {
                purchaseButton.onClick.AddListener(OnPurchaseClicked);
            }
        }

        public void Refresh()
        {
            if (upgradeData == null) return;

            int level = Systems.UpgradeManager.Instance.GetUpgradeLevel(upgradeData.upgradeName);
            int cost = Systems.UpgradeManager.Instance.GetNextLevelCost(upgradeData.upgradeName);
            bool isMaxLevel = Systems.UpgradeManager.Instance.IsMaxLevel(upgradeData.upgradeName);

            if (nameText != null)
            {
                nameText.text = upgradeData.displayName;
            }

            if (levelText != null)
            {
                levelText.text = isMaxLevel ? "MAX" : $"Lv.{level}";
            }

            if (descriptionText != null)
            {
                descriptionText.text = upgradeData.description;
            }

            if (costText != null)
            {
                costText.text = isMaxLevel ? "MAX" : Systems.CurrencyManager.FormatCurrency(cost);
            }

            if (icon != null && upgradeData.icon != null)
            {
                icon.sprite = upgradeData.icon;
            }

            if (purchaseButton != null)
            {
                purchaseButton.interactable = !isMaxLevel && Systems.CurrencyManager.Instance.HasCoins(cost);
            }
        }

        private void OnPurchaseClicked()
        {
            if (upgradeData != null)
            {
                bool success = Systems.UpgradeManager.Instance.PurchaseUpgrade(upgradeData.upgradeName);
                if (success)
                {
                    Managers.AudioManager.Instance?.PlaySFX("UpgradePurchased");
                    Refresh();
                }
            }
        }
    }
}
