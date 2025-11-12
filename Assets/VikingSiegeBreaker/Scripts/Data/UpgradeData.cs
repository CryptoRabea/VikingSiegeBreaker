using UnityEngine;

namespace VikingSiegeBreaker.Data
{
    /// <summary>
    /// ScriptableObject defining upgrade parameters and formulas.
    /// Create instances via: Assets > Create > VikingSiegeBreaker > Data > Upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "VikingSiegeBreaker/Data/Upgrade", order = 1)]
    public class UpgradeData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for code reference")]
        public string upgradeName = "LaunchPower";

        [Tooltip("Display name shown in UI")]
        public string displayName = "Launch Power";

        [Tooltip("Category for grouping (Combat/Economy/Utility)")]
        public string category = "Combat";

        [Tooltip("Description shown in UI")]
        [TextArea(2, 4)]
        public string description = "Increases catapult launch power.";

        [Header("Visual")]
        public Sprite icon;
        public Color backgroundColor = Color.white;

        [Header("Cost Formula")]
        [Tooltip("Base cost for level 1")]
        public int baseCost = 100;

        [Tooltip("Cost multiplier per level (exponential growth)")]
        [Range(1.01f, 2f)]
        public float costMultiplier = 1.15f;

        [Header("Effect Formula")]
        [Tooltip("Base effect value at level 1")]
        public float baseEffect = 10f;

        [Tooltip("Effect increase per level (additive)")]
        public float effectPerLevel = 5f;

        [Tooltip("Use multiplicative scaling instead of additive")]
        public bool useMultiplicativeScaling = false;

        [Tooltip("Effect multiplier per level (if multiplicative)")]
        [Range(0.01f, 0.5f)]
        public float effectMultiplier = 0.1f;

        [Header("Display Settings")]
        [Tooltip("Unit to display (%, dmg, m/s, etc.)")]
        public string unit = "";

        [Tooltip("Show as percentage in UI")]
        public bool displayAsPercentage = false;

        [Header("Example Values (Editor Only)")]
        [SerializeField] private int[] exampleLevels = { 1, 10, 25, 50, 100 };

        /// <summary>
        /// Calculates cost for a given level.
        /// </summary>
        public int CalculateCost(int level)
        {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level - 1));
        }

        /// <summary>
        /// Calculates effect for a given level.
        /// </summary>
        public float CalculateEffect(int level)
        {
            if (useMultiplicativeScaling)
            {
                return baseEffect * Mathf.Pow(1f + effectMultiplier, level);
            }
            else
            {
                return baseEffect + (effectPerLevel * level);
            }
        }

        /// <summary>
        /// Gets formatted effect text for UI.
        /// </summary>
        public string GetEffectText(int level)
        {
            float effect = CalculateEffect(level);

            if (displayAsPercentage)
            {
                return $"+{effect:F1}%";
            }
            else if (!string.IsNullOrEmpty(unit))
            {
                return $"+{effect:F0} {unit}";
            }
            else
            {
                return $"+{effect:F0}";
            }
        }

        // Editor validation
        private void OnValidate()
        {
            // Show example calculations in inspector
            if (exampleLevels != null && exampleLevels.Length > 0)
            {
                string examples = "Example values:\n";
                foreach (int level in exampleLevels)
                {
                    int cost = CalculateCost(level);
                    float effect = CalculateEffect(level);
                    examples += $"Lv.{level}: Cost={cost:N0}, Effect={effect:F2}\n";
                }
                // Note: This is visible in debug/console if needed
            }
        }
    }
}
