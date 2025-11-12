using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// HUD display for gameplay - shows momentum, speed, distance, coins, XP, abilities, etc.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject hudPanel;

        [Header("Momentum/Speed")]
        [SerializeField] private Slider momentumBar;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private Image momentumFillImage;

        [Header("Distance")]
        [SerializeField] private TextMeshProUGUI distanceText;

        [Header("Currencies")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI xpText;
        [SerializeField] private Slider xpBar;

        [Header("Health")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Abilities")]
        [SerializeField] private Image dashCooldownImage;
        [SerializeField] private Image shieldIcon;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI enemiesKilledText;

        private Player.MomentumSystem momentumSystem;
        private Player.PlayerController playerController;

        private void Start()
        {
            // Find player references
            FindPlayerReferences();

            // Subscribe to events
            SubscribeToEvents();

            // Initialize UI
            UpdateUI();
        }

        private void Update()
        {
            if (Core.GameManager.Instance.CurrentState == Core.GameState.Playing)
            {
                UpdateDynamicUI();
            }
        }

        #region Initialization

        private void FindPlayerReferences()
        {
            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                playerController = player;
                momentumSystem = player.GetComponent<Player.MomentumSystem>();
            }
        }

        private void SubscribeToEvents()
        {
            // Momentum events
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged += UpdateMomentumBar;
                momentumSystem.OnSpeedChanged += UpdateSpeed;
            }

            // Player events
            if (playerController != null)
            {
                playerController.OnHealthChanged += UpdateHealthBar;
                playerController.OnShieldChanged += UpdateShieldIcon;
            }

            // Currency events
            Systems.CurrencyManager.Instance.OnCoinsChanged += UpdateCoins;
            Systems.CurrencyManager.Instance.OnXPChanged += UpdateXP;
        }

        #endregion

        #region UI Updates

        private void UpdateDynamicUI()
        {
            // Distance
            if (distanceText != null)
            {
                float distance = Core.GameManager.Instance.CurrentDistance;
                distanceText.text = $"{distance:F0}m";
            }

            // Enemies killed
            if (enemiesKilledText != null)
            {
                int kills = Core.GameManager.Instance.CurrentEnemiesKilled;
                enemiesKilledText.text = $"Kills: {kills}";
            }
        }

        private void UpdateMomentumBar(float normalizedMomentum)
        {
            if (momentumBar != null)
            {
                momentumBar.value = normalizedMomentum;

                // Color based on momentum
                if (momentumFillImage != null && momentumSystem != null)
                {
                    momentumFillImage.color = momentumSystem.GetSpeedColor();
                }
            }
        }

        private void UpdateSpeed(float speed)
        {
            if (speedText != null)
            {
                speedText.text = $"{speed:F1} m/s";
            }
        }

        private void UpdateHealthBar(float normalizedHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = normalizedHealth;
            }

            if (healthText != null && playerController != null)
            {
                healthText.text = $"{playerController.CurrentHealth:F0}/{playerController.MaxHealth:F0}";
            }
        }

        private void UpdateCoins(int coins)
        {
            if (coinsText != null)
            {
                coinsText.text = Systems.CurrencyManager.FormatCurrency(coins);
            }
        }

        private void UpdateXP(int xp)
        {
            if (xpText != null)
            {
                xpText.text = Systems.CurrencyManager.FormatCurrency(xp);
            }

            if (xpBar != null)
            {
                float progress = Systems.EvolutionManager.Instance.GetProgressToNextEra();
                xpBar.value = progress;
            }
        }

        private void UpdateShieldIcon(bool active)
        {
            if (shieldIcon != null)
            {
                shieldIcon.enabled = active;
            }
        }

        private void UpdateUI()
        {
            // Initial update
            if (momentumSystem != null)
            {
                UpdateMomentumBar(momentumSystem.NormalizedMomentum);
                UpdateSpeed(momentumSystem.CurrentSpeed);
            }

            if (playerController != null)
            {
                UpdateHealthBar(playerController.NormalizedHealth);
            }

            UpdateCoins(Systems.CurrencyManager.Instance.Coins);
            UpdateXP(Systems.CurrencyManager.Instance.XP);
        }

        #endregion

        #region Visibility

        public void Show()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(true);
            }

            // Refresh player references in case of new run
            FindPlayerReferences();
            SubscribeToEvents();
        }

        public void Hide()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(false);
            }
        }

        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged -= UpdateMomentumBar;
                momentumSystem.OnSpeedChanged -= UpdateSpeed;
            }

            if (playerController != null)
            {
                playerController.OnHealthChanged -= UpdateHealthBar;
                playerController.OnShieldChanged -= UpdateShieldIcon;
            }

            if (Systems.CurrencyManager.Instance != null)
            {
                Systems.CurrencyManager.Instance.OnCoinsChanged -= UpdateCoins;
                Systems.CurrencyManager.Instance.OnXPChanged -= UpdateXP;
            }
        }
    }
}
