using UnityEngine;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Main menu UI controller.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject menuPanel;

        [Header("Display")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI gemsText;
        [SerializeField] private TextMeshProUGUI eraText;
        [SerializeField] private TextMeshProUGUI bestDistanceText;

        private void Start()
        {
            UpdateDisplay();

            // Subscribe to currency changes
            Systems.CurrencyManager.Instance.OnCoinsChanged += OnCurrencyChanged;
            Systems.CurrencyManager.Instance.OnGemsChanged += OnCurrencyChanged;
        }

        private void OnCurrencyChanged(int value)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (coinsText != null)
            {
                coinsText.text = Systems.CurrencyManager.FormatCurrency(Systems.CurrencyManager.Instance.Coins);
            }

            if (gemsText != null)
            {
                gemsText.text = Systems.CurrencyManager.Instance.Gems.ToString();
            }

            if (eraText != null)
            {
                eraText.text = Systems.EvolutionManager.Instance.GetCurrentEraDisplayText();
            }

            if (bestDistanceText != null)
            {
                float best = Core.SaveSystem.Instance.GetBestDistance();
                bestDistanceText.text = $"Best: {best:F0}m";
            }
        }

        public void Show()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
                UpdateDisplay();
            }
        }

        public void Hide()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Systems.CurrencyManager.Instance != null)
            {
                Systems.CurrencyManager.Instance.OnCoinsChanged -= OnCurrencyChanged;
                Systems.CurrencyManager.Instance.OnGemsChanged -= OnCurrencyChanged;
            }
        }
    }
}
