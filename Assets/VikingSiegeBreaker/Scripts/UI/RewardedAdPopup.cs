using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Rewarded Ad popup - prompts player to watch an ad for rewards.
    /// Shows reward information and allows player to accept or decline.
    /// </summary>
    public class RewardedAdPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;
        [SerializeField] private GameObject panel;

        private Action onAcceptCallback;
        private Action onDeclineCallback;

        private void Start()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(OnAcceptClicked);
            }

            if (declineButton != null)
            {
                declineButton.onClick.AddListener(OnDeclineClicked);
            }
        }

        /// <summary>
        /// Show the rewarded ad popup with reward text and callbacks.
        /// </summary>
        public void Show(string reward, Action onAccept, Action onDecline)
        {
            if (string.IsNullOrEmpty(reward))
            {
                Debug.LogWarning("RewardedAdPopup: Attempted to show popup with null/empty reward text");
                return;
            }

            onAcceptCallback = onAccept;
            onDeclineCallback = onDecline;

            // Update UI elements
            if (rewardText != null)
            {
                rewardText.text = reward;
            }

            if (titleText != null)
            {
                titleText.text = "Watch Ad for Reward?";
            }

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
        /// Hide the rewarded ad popup.
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

            // Clear callbacks
            onAcceptCallback = null;
            onDeclineCallback = null;
        }

        private void OnAcceptClicked()
        {
            Hide();
            onAcceptCallback?.Invoke();
        }

        private void OnDeclineClicked()
        {
            Hide();
            onDeclineCallback?.Invoke();
        }

        private void OnDestroy()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(OnAcceptClicked);
            }

            if (declineButton != null)
            {
                declineButton.onClick.RemoveListener(OnDeclineClicked);
            }
        }
    }
}
