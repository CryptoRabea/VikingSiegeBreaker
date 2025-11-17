using UnityEngine;
using System;
using System.Collections;

namespace VikingSiegeBreaker.Managers
{
    /// <summary>
    /// Central UI management system - handles all UI panels, popups, and transitions.
    /// References: HUD, menus, popups, and manages their visibility/state.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private UI.HUDController hudController;
        [SerializeField] private UI.MenuController menuController;
        [SerializeField] private UI.GameOverPanel gameOverPanel;
        [SerializeField] private UI.ShopPanel shopPanel;
        [SerializeField] private UI.UpgradePanel upgradePanel;

        [Header("Popups")]
        [SerializeField] private GameObject evolutionPopup;
        [SerializeField] private GameObject offlineBlockPopup;
        [SerializeField] private GameObject pausePopup;
        [SerializeField] private GameObject rewardedAdPopup;
        [SerializeField] private GameObject iapPopup;

        [Header("Transitions")]
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 0.5f;

        // Events
        public event Action OnMenuOpened;
        public event Action OnMenuClosed;

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
            // Subscribe to game state changes
            Core.GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

            // Subscribe to network check
            Core.NetworkCheck.Instance.OnOfflinePlayBlocked += ShowOfflineBlockPopup;

            // Initialize UI
            UpdateUIForState(Core.GameManager.Instance.CurrentState);
        }

        #region State Management

        private void HandleGameStateChanged(Core.GameState newState)
        {
            UpdateUIForState(newState);
        }

        private void UpdateUIForState(Core.GameState state)
        {
            switch (state)
            {
                case Core.GameState.MainMenu:
                    ShowMainMenu();
                    break;

                case Core.GameState.Playing:
                    ShowHUD();
                    break;

                case Core.GameState.Paused:
                    ShowPauseMenu();
                    break;

                case Core.GameState.GameOver:
                    ShowGameOver();
                    break;
            }
        }

        #endregion

        #region Panel Management

        public void ShowMainMenu()
        {
            if (menuController != null)
                menuController.Show();

            if (hudController != null)
                hudController.Hide();

            if (gameOverPanel != null)
                gameOverPanel.Hide();

            OnMenuOpened?.Invoke();
        }

        public void ShowHUD()
        {
            if (hudController != null)
                hudController.Show();

            if (menuController != null)
                menuController.Hide();

            if (gameOverPanel != null)
                gameOverPanel.Hide();

            HideAllPopups();
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                // Delay slightly for dramatic effect
                StartCoroutine(ShowGameOverDelayed(1f));
            }

            if (hudController != null)
                hudController.Hide();
        }

        private IEnumerator ShowGameOverDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameOverPanel != null)
                gameOverPanel.Show();
        }

        public void ShowPauseMenu()
        {
            if (pausePopup != null)
                pausePopup.SetActive(true);
        }

        public void HidePauseMenu()
        {
            if (pausePopup != null)
                pausePopup.SetActive(false);
        }

        public void ShowShop()
        {
            if (shopPanel != null)
                shopPanel.Show();
        }

        public void HideShop()
        {
            if (shopPanel != null)
                shopPanel.Hide();
        }

        public void ShowUpgrades()
        {
            if (upgradePanel != null)
                upgradePanel.Show();
        }

        public void HideUpgrades()
        {
            if (upgradePanel != null)
                upgradePanel.Hide();
        }

        #endregion

        #region Popups

        public void ShowEvolutionPopup(Data.EvolutionData evolution)
        {
            if (evolutionPopup != null)
            {
                evolutionPopup.SetActive(true);

                // Update evolution popup content (requires EvolutionPopup script)
                if (evolutionPopup.TryGetComponent<UI.EvolutionPopup>(out var popup))
                {
                    popup.Show(evolution);
                }
            }

            AudioManager.Instance?.PlaySFX("Evolution");
        }

        public void ShowOfflineBlockPopup()
        {
            if (offlineBlockPopup != null)
            {
                offlineBlockPopup.SetActive(true);

                // Update message
                var message = offlineBlockPopup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (message != null)
                {
                    message.text = Core.NetworkCheck.Instance.GetBlockedMessage();
                }
            }
        }

        public void HideOfflineBlockPopup()
        {
            if (offlineBlockPopup != null)
                offlineBlockPopup.SetActive(false);
        }

        public void ShowRewardedAdPrompt(string rewardText, Action onAccept, Action onDecline)
        {
            if (rewardedAdPopup != null)
            {
                rewardedAdPopup.SetActive(true);

                // Set reward text and callbacks (requires RewardedAdPopup script)
                var popup = rewardedAdPopup.GetComponent<UI.RewardedAdPopup>();
                if (popup != null)
                {
                    popup.Show(rewardText, onAccept, onDecline);
                }
            }
        }

        public void ShowIAPPopup(string productId)
        {
            if (iapPopup != null)
            {
                iapPopup.SetActive(true);

                // Configure IAP popup (requires IAPPopup script)
                var popup = iapPopup.GetComponent<UI.IAPPopup>();
                if (popup != null)
                {
                    popup.Show(productId);
                }
            }
        }

        public void HideAllPopups()
        {
            if (evolutionPopup != null) evolutionPopup.SetActive(false);
            if (offlineBlockPopup != null) offlineBlockPopup.SetActive(false);
            if (pausePopup != null) pausePopup.SetActive(false);
            if (rewardedAdPopup != null) rewardedAdPopup.SetActive(false);
            if (iapPopup != null) iapPopup.SetActive(false);
        }

        #endregion

        #region Screen Transitions

        public void FadeOut(Action onComplete = null)
        {
            StartCoroutine(FadeCoroutine(0f, 1f, onComplete));
        }

        public void FadeIn(Action onComplete = null)
        {
            StartCoroutine(FadeCoroutine(1f, 0f, onComplete));
        }

        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, Action onComplete)
        {
            if (fadePanel == null) yield break;

            float elapsed = 0f;
            fadePanel.alpha = startAlpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                yield return null;
            }

            fadePanel.alpha = endAlpha;
            onComplete?.Invoke();
        }

        #endregion

        #region Notifications

        public void ShowNotification(string message, float duration = 2f)
        {
            // Simple toast notification (can expand)
            Debug.Log($"[Notification] {message}");
            // TODO: Implement toast UI element
        }

        public void ShowErrorMessage(string error)
        {
            ShowNotification($"Error: {error}", 3f);
        }

        #endregion

        #region Button Callbacks (for UI events)

        public void OnPlayButtonClicked()
        {
            // Validate network permission
            if (!Core.NetworkCheck.Instance.ValidatePlayPermission())
            {
                return; // Popup shown by NetworkCheck
            }

            Core.GameManager.Instance.LoadGameplay();
        }

        public void OnShopButtonClicked()
        {
            ShowShop();
        }

        public void OnUpgradesButtonClicked()
        {
            ShowUpgrades();
        }

        public void OnSettingsButtonClicked()
        {
            // TODO: Show settings popup
        }

        public void OnPauseButtonClicked()
        {
            Core.GameManager.Instance.PauseGame();
        }

        public void OnResumeButtonClicked()
        {
            Core.GameManager.Instance.ResumeGame();
        }

        public void OnRestartButtonClicked()
        {
            Core.GameManager.Instance.RestartRun();
        }

        public void OnMainMenuButtonClicked()
        {
            Core.GameManager.Instance.LoadMainMenu();
        }

        public void OnReviveButtonClicked()
        {
            // Show rewarded ad
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                Core.GameManager.Instance.RevivePlayer();
            });
        }

        public void OnRemoveAdsButtonClicked()
        {
            IAPManager.Instance.PurchaseRemoveAds();
        }

        #endregion

        private void OnDestroy()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }

            if (Core.NetworkCheck.Instance != null)
            {
                Core.NetworkCheck.Instance.OnOfflinePlayBlocked -= ShowOfflineBlockPopup;
            }
        }
    }
}
