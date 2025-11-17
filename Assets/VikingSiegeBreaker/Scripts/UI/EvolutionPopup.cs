using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VikingSiegeBreaker.UI
{
    /// <summary>
    /// Evolution popup - displays evolution information when player levels up.
    /// Shows evolution details and allows player to confirm the evolution.
    /// </summary>
    public class EvolutionPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI evolutionNameText;
        [SerializeField] private TextMeshProUGUI evolutionDescriptionText;
        [SerializeField] private Image evolutionIcon;
        [SerializeField] private Button confirmButton;
        [SerializeField] private GameObject panel;

        private Data.EvolutionData currentEvolution;

        private void Start()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }
        }

        /// <summary>
        /// Show the evolution popup with the specified evolution data.
        /// </summary>
        public void Show(Data.EvolutionData evolution)
        {
            if (evolution == null)
            {
                Debug.LogWarning("EvolutionPopup: Attempted to show popup with null evolution data");
                return;
            }

            currentEvolution = evolution;

            // Update UI elements
            if (evolutionNameText != null)
            {
                evolutionNameText.text = evolution.evolutionName;
            }

            if (evolutionDescriptionText != null)
            {
                evolutionDescriptionText.text = evolution.description;
            }

            if (evolutionIcon != null && evolution.icon != null)
            {
                evolutionIcon.sprite = evolution.icon;
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
        /// Hide the evolution popup.
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

        private void OnConfirmClicked()
        {
            // Apply the evolution if needed
            // This would typically be handled by the EvolutionSystem

            Hide();

            // Notify that evolution was confirmed
            Debug.Log($"Evolution confirmed: {currentEvolution?.evolutionName}");
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(OnConfirmClicked);
            }
        }
    }
}
