using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

namespace VikingSiegeBreaker.Player
{
    /// <summary>
    /// Main player controller - handles input, airborne abilities, collision handling,
    /// damage, and animation triggers. Uses Unity's new Input System.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(MomentumSystem), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private MomentumSystem momentumSystem;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Combat Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float critChance = 0.1f; // 10%
        [SerializeField] private float critDamage = 2f; // 2x multiplier

        [Header("Abilities")]
        [SerializeField] private bool dashUnlocked = true;
        [SerializeField] private float dashForce = 30f;
        [SerializeField] private float dashCooldown = 3f;
        [SerializeField] private bool shieldActive = false;
        [SerializeField] private float shieldDuration = 5f;

        [Header("Physics")]
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private bool enableRagdoll = false;

        [Header("VFX")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject dashEffectPrefab;
        [SerializeField] private GameObject shieldEffectPrefab;
        [SerializeField] private ParticleSystem trailEffect;

        // State
        private bool isDashing = false;
        private bool canDash = true;
        private Coroutine shieldCoroutine;

        // Events
        public event Action<float> OnHealthChanged; // normalized 0-1
        public event Action OnPlayerDied;
        public event Action OnDashUsed;
        public event Action<bool> OnShieldChanged;

        // Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float NormalizedHealth => currentHealth / maxHealth;
        public bool IsShielded => shieldActive;
        public bool IsAlive => currentHealth > 0;

        // Input callbacks (assigned via Input Actions)
        private InputAction dashAction;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (momentumSystem == null) momentumSystem = GetComponent<MomentumSystem>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Apply upgrades
            ApplyUpgrades();

            // Initialize health
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(NormalizedHealth);

            // Setup input (using new Input System)
            SetupInput();

            // Start trail effect
            if (trailEffect != null)
            {
                trailEffect.Play();
            }
        }

        private void Update()
        {
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Playing)
                return;

            // Update animations
            UpdateAnimations();

            // Update rotation based on velocity
            UpdateRotation();
        }

        #region Input Setup

        /// <summary>
        /// Sets up input using the new Input System.
        /// Requires an Input Actions asset with "Dash" action mapped.
        /// </summary>
        private void SetupInput()
        {
            // Find the input action (assumes you have an Input Actions asset named "PlayerInputActions")
            // In production, assign this via inspector or InputActionAsset reference
            var inputAsset = Resources.Load<InputActionAsset>("PlayerInputActions");
            if (inputAsset != null)
            {
                dashAction = inputAsset.FindAction("Dash");
                if (dashAction != null)
                {
                    dashAction.performed += OnDashInput;
                    dashAction.Enable();
                }
            }
            else
            {
                Debug.LogWarning("[PlayerController] PlayerInputActions not found in Resources. Using touch input fallback.");
            }
        }

        /// <summary>
        /// Fallback touch/mouse input for dash (if Input Actions not set up).
        /// </summary>
        private void CheckTouchInput()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
            {
                UseDash();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                UseDash();
            }
        }

        private void OnDashInput(InputAction.CallbackContext context)
        {
            UseDash();
        }

        #endregion

        #region Abilities

        /// <summary>
        /// Activates the dash ability (adds forward momentum burst).
        /// </summary>
        public void UseDash()
        {
            if (!canDash || isDashing || !dashUnlocked) return;

            Debug.Log("[PlayerController] Dash activated!");

            isDashing = true;
            canDash = false;

            // Apply dash force
            Vector2 dashDirection = rb.linearVelocity.normalized;
            if (dashDirection.magnitude < 0.1f) dashDirection = Vector2.right;
            rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

            // VFX
            if (dashEffectPrefab != null)
            {
                Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
            }

            // Animation trigger
            animator.SetTrigger("Dash");

            // Cooldown
            StartCoroutine(DashCooldown());

            OnDashUsed?.Invoke();
        }

        private IEnumerator DashCooldown()
        {
            yield return new WaitForSeconds(0.2f);
            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        /// <summary>
        /// Activates shield (from pickup).
        /// </summary>
        public void ActivateShield()
        {
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
            shieldCoroutine = StartCoroutine(ShieldDuration());
        }

        private IEnumerator ShieldDuration()
        {
            shieldActive = true;
            OnShieldChanged?.Invoke(true);

            // Spawn shield VFX
            GameObject shieldVFX = null;
            if (shieldEffectPrefab != null)
            {
                shieldVFX = Instantiate(shieldEffectPrefab, transform);
            }

            Debug.Log("[PlayerController] Shield activated");

            yield return new WaitForSeconds(shieldDuration);

            shieldActive = false;
            OnShieldChanged?.Invoke(false);

            if (shieldVFX != null)
            {
                Destroy(shieldVFX);
            }

            Debug.Log("[PlayerController] Shield expired");
        }

        #endregion

        #region Combat

        /// <summary>
        /// Takes damage from enemies or hazards.
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (shieldActive)
            {
                Debug.Log("[PlayerController] Damage blocked by shield");
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - damage);
            OnHealthChanged?.Invoke(NormalizedHealth);

            // VFX
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Animation
            animator.SetTrigger("Hit");

            // Flash red
            StartCoroutine(DamageFlash());

            Debug.Log($"[PlayerController] Took {damage} damage (Health: {currentHealth}/{maxHealth})");

            // Check death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        /// <summary>
        /// Calculates damage dealt to enemies (with crit chance).
        /// </summary>
        public float CalculateDamage()
        {
            float damage = baseDamage;

            // Crit roll
            if (UnityEngine.Random.value < critChance)
            {
                damage *= critDamage;
                Debug.Log($"[PlayerController] CRITICAL HIT! {damage}");
            }

            // Bonus damage based on speed
            float speedBonus = momentumSystem.CurrentSpeed * 0.1f;
            damage += speedBonus;

            return damage;
        }

        #endregion

        #region Collision Handling

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Playing)
                return;

            // Enemy collision
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.gameObject.GetComponent<Entities.Enemy>();
                if (enemy != null)
                {
                    HandleEnemyCollision(enemy, collision);
                }
            }
            // Wall collision
            else if (collision.gameObject.CompareTag("Wall"))
            {
                momentumSystem.OnWallHit();
                Managers.AudioManager.Instance?.PlaySFX("WallHit");
            }
            // Ground collision
            else if (collision.gameObject.CompareTag("Ground"))
            {
                momentumSystem.OnGroundHit();
            }
        }

        private void HandleEnemyCollision(Entities.Enemy enemy, Collision2D collision)
        {
            // Deal damage to enemy
            float damage = CalculateDamage();
            enemy.TakeDamage(damage);

            // Take damage from enemy (if not shielded)
            if (!shieldActive)
            {
                TakeDamage(enemy.GetContactDamage());
            }

            // Momentum penalty
            momentumSystem.OnEnemyHit(enemy);

            // Knockback enemy
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockbackDir, momentumSystem.CurrentSpeed * 0.5f);

            // Audio
            Managers.AudioManager.Instance?.PlaySFX("EnemyHit");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Pickup collection
            if (collision.CompareTag("Pickup"))
            {
                var pickup = collision.GetComponent<Entities.Pickup>();
                if (pickup != null)
                {
                    pickup.Collect(this);
                }
            }
        }

        #endregion

        #region Animation & Visuals

        /// <summary>
        /// Updates animator parameters based on state.
        /// </summary>
        private void UpdateAnimations()
        {
            animator.SetFloat("Speed", momentumSystem.CurrentSpeed);
            animator.SetFloat("VelocityX", rb.linearVelocity.x);
            animator.SetFloat("VelocityY", rb.linearVelocity.y);
            animator.SetBool("IsGrounded", IsGrounded());
            animator.SetBool("IsShielded", shieldActive);
        }

        /// <summary>
        /// Rotates the player based on velocity direction (ragdoll-style).
        /// </summary>
        private void UpdateRotation()
        {
            if (enableRagdoll && rb.linearVelocity.magnitude > 1f)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region Death & Revive

        private void Die()
        {
            Debug.Log("[PlayerController] Player died");
            animator.SetTrigger("Death");
            OnPlayerDied?.Invoke();

            // Trigger game over (handled by MomentumSystem)
        }

        /// <summary>
        /// Revives the player (called after watching rewarded ad).
        /// </summary>
        public void Revive()
        {
            currentHealth = maxHealth * 0.5f; // Revive with 50% health
            OnHealthChanged?.Invoke(NormalizedHealth);

            momentumSystem.ResetForRevive();

            animator.SetTrigger("Revive");
            Debug.Log("[PlayerController] Player revived");
        }

        #endregion

        #region Upgrades

        private void ApplyUpgrades()
        {
            var upgradeManager = Systems.UpgradeManager.Instance;
            if (upgradeManager == null) return;

            maxHealth += upgradeManager.GetUpgradeValue("MaxHealth");
            critChance += upgradeManager.GetUpgradeValue("CritChance") * 0.01f; // % to decimal
            critDamage += upgradeManager.GetUpgradeValue("CritDamage") * 0.01f;

            Debug.Log($"[PlayerController] Upgrades applied - HP: {maxHealth}, Crit: {critChance:P2}");
        }

        #endregion

        #region Helpers

        private bool IsGrounded()
        {
            // Simple ground check (can improve with raycast)
            return Mathf.Abs(rb.linearVelocity.y) < 0.1f;
        }

        #endregion

        private void OnDestroy()
        {
            // Cleanup input
            if (dashAction != null)
            {
                dashAction.performed -= OnDashInput;
                dashAction.Disable();
            }
        }
    }
}
