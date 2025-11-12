using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Game Over screen - shows run stats and options (revive, restart, menu).
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject panel;

        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI distanceText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI enemiesText;
        [SerializeField] private TextMeshProUGUI xpEarnedText;
        [SerializeField] private TextMeshProUGUI newBestText;

        [Header("Buttons")]
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private Core.RunStats lastRunStats;

        private void Start()
        {
            // Subscribe to run ended event
            Core.GameManager.Instance.OnRunEnded += OnRunEnded;

            // Check revive availability
            UpdateReviveButton();
        }

        private void OnRunEnded(Core.RunStats stats)
        {
            lastRunStats = stats;
            UpdateDisplay(stats);
        }

        private void UpdateDisplay(Core.RunStats stats)
        {
            if (distanceText != null)
            {
                distanceText.text = $"{stats.distance:F0}m";
            }

            if (coinsText != null)
            {
                coinsText.text = $"+{stats.coinsCollected}";
            }

            if (enemiesText != null)
            {
                enemiesText.text = $"{stats.enemiesKilled}";
            }

            if (xpEarnedText != null)
            {
                xpEarnedText.text = $"+{stats.bonusXP} XP";
            }

            // Check if new best
            float bestDistance = Core.SaveSystem.Instance.GetBestDistance();
            if (newBestText != null)
            {
                if (stats.distance > bestDistance)
                {
                    newBestText.text = "NEW BEST!";
                    newBestText.gameObject.SetActive(true);
                }
                else
                {
                    newBestText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateReviveButton()
        {
            if (reviveButton != null)
            {
                bool canRevive = Core.GameManager.Instance.CanRevive;
                reviveButton.interactable = canRevive;

                var buttonText = reviveButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = canRevive ? "Revive (Ad)" : "No Revives";
                }
            }
        }

        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                UpdateReviveButton();
            }
        }

        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnRunEnded -= OnRunEnded;
            }
        }
    }
}
