using UnityEngine;

namespace VikingSiegeBreaker.Data
{
    /// <summary>
    /// ScriptableObject defining evolution/era progression data.
    /// Create instances via: Assets > Create > VikingSiegeBreaker > Data > Evolution
    /// </summary>
    [CreateAssetMenu(fileName = "NewEvolution", menuName = "VikingSiegeBreaker/Data/Evolution", order = 4)]
    public class EvolutionData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Era number (0-based: 0=Era1, 1=Era2, etc.)")]
        public int era = 0;

        [Tooltip("Display name")]
        public string displayName = "Viking Warrior";

        [Tooltip("Description/lore")]
        [TextArea(3, 5)]
        public string description = "A fierce Viking warrior ready for battle.";

        [Header("Visual")]
        [Tooltip("Icon for this era (shown in UI)")]
        public Sprite icon;

        [Tooltip("Player prefab for this era")]
        public GameObject playerPrefab;

        [Tooltip("Theme color for UI")]
        public Color themeColor = Color.blue;

        [Header("XP Requirement")]
        [Tooltip("Total XP required to reach this era")]
        public int xpRequired = 0;

        [Header("Stat Bonuses")]
        [Tooltip("Bonus health added at this era")]
        public float healthBonus = 0f;

        [Tooltip("Bonus damage % at this era")]
        [Range(0f, 100f)]
        public float damageBonus = 0f;

        [Tooltip("Bonus speed % at this era")]
        [Range(0f, 100f)]
        public float speedBonus = 0f;

        [Tooltip("Critical chance bonus %")]
        [Range(0f, 20f)]
        public float critChanceBonus = 0f;

        [Header("Unlocks")]
        [Tooltip("New abilities unlocked at this era")]
        public string[] unlockedAbilities = new string[0];

        [Tooltip("Special features unlocked")]
        [TextArea(2, 3)]
        public string specialFeatures = "";

        [Header("VFX")]
        [Tooltip("Evolution animation/effect")]
        public GameObject evolutionEffectPrefab;

        [Tooltip("Ambient particle effects for this era")]
        public GameObject ambientEffectPrefab;

        public AudioClip evolutionSound;

        /// <summary>
        /// Gets a summary of all bonuses for display.
        /// </summary>
        public string GetBonusSummary()
        {
            string summary = "";

            if (healthBonus > 0)
                summary += $"+{healthBonus:F0} HP\n";

            if (damageBonus > 0)
                summary += $"+{damageBonus:F0}% Damage\n";

            if (speedBonus > 0)
                summary += $"+{speedBonus:F0}% Speed\n";

            if (critChanceBonus > 0)
                summary += $"+{critChanceBonus:F0}% Crit Chance\n";

            if (unlockedAbilities.Length > 0)
            {
                summary += "\nUnlocked Abilities:\n";
                foreach (string ability in unlockedAbilities)
                {
                    summary += $"- {ability}\n";
                }
            }

            return summary.Trim();
        }

        /// <summary>
        /// Gets formatted era display text.
        /// Example: "Era 2: Berserker Chief"
        /// </summary>
        public string GetEraDisplayText()
        {
            return $"Era {era + 1}: {displayName}";
        }
    }
}
