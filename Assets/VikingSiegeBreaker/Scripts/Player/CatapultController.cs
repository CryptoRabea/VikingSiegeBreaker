using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace VikingSiegeBreaker.Player
{
    /// <summary>
    /// Controls the catapult launch mechanic - handles aim, charge power, and trajectory preview.
    /// Uses touch/mouse hold-and-release for charging, with visual feedback arc.
    /// </summary>
    public class CatapultController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform catapultArm;
        [SerializeField] private Transform launchPoint;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private LineRenderer trajectoryLine;

        [Header("Launch Settings")]
        [SerializeField] private float minLaunchAngle = 30f;
        [SerializeField] private float maxLaunchAngle = 60f;
        [SerializeField] private float chargeSpeed = 0.5f; // How fast power charges
        [SerializeField] private float maxChargeTime = 2f; // Max seconds to full charge

        [Header("Charge State")]
        [SerializeField] private bool isCharging = false;
        [SerializeField] private float currentCharge = 0f; // 0 to 1
        [SerializeField] private float currentAngle = 45f;

        [Header("Trajectory Preview")]
        [SerializeField] private bool showTrajectory = true;
        [SerializeField] private int trajectoryPoints = 30;
        [SerializeField] private float trajectoryTimeStep = 0.1f;
        [SerializeField] private Color trajectoryColor = Color.yellow;

        [Header("Visual Feedback")]
        [SerializeField] private Transform chargeIndicator; // UI element for charge bar
        [SerializeField] private Color lowChargeColor = Color.red;
        [SerializeField] private Color mediumChargeColor = Color.yellow;
        [SerializeField] private Color fullChargeColor = Color.green;

        // State
        private GameObject spawnedPlayer;
        private MomentumSystem playerMomentum;
        private bool hasLaunched = false;

        // Events
        public event Action OnChargeStarted;
        public event Action<float> OnChargeUpdated; // 0-1 normalized
        public event Action<float> OnLaunched; // charge value

        // Input
        private InputAction aimAction;
        private InputAction launchAction;

        private void Awake()
        {
            SetupTrajectoryLine();
        }

        private void Start()
        {
            SetupInput();
            SpawnPlayer();
        }

        private void Update()
        {
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Playing || hasLaunched)
                return;

            HandleInput();
            UpdateVisuals();
        }

        #region Input Setup

        private void SetupInput()
        {
            // Try to load Input Actions asset
            var inputAsset = Resources.Load<InputActionAsset>("PlayerInputActions");
            if (inputAsset != null)
            {
                aimAction = inputAsset.FindAction("Aim");
                launchAction = inputAsset.FindAction("Launch");

                if (aimAction != null)
                {
                    aimAction.Enable();
                }

                if (launchAction != null)
                {
                    launchAction.started += OnLaunchStarted;
                    launchAction.canceled += OnLaunchReleased;
                    launchAction.Enable();
                }
            }
            else
            {
                Debug.LogWarning("[CatapultController] Input Actions not found, using fallback input");
            }
        }

        private void HandleInput()
        {
            // Fallback mouse/touch input
            if (launchAction == null)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began))
                {
                    StartCharging();
                }
                else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Ended))
                {
                    ReleaseLaunch();
                }
            }

            // Update charge
            if (isCharging)
            {
                UpdateCharge();
            }

            // Update aim (using mouse/touch position)
            if (aimAction != null && aimAction.IsPressed())
            {
                Vector2 aimPos = aimAction.ReadValue<Vector2>();
                UpdateAim(aimPos);
            }
        }

        private void OnLaunchStarted(InputAction.CallbackContext context)
        {
            StartCharging();
        }

        private void OnLaunchReleased(InputAction.CallbackContext context)
        {
            ReleaseLaunch();
        }

        #endregion

        #region Charging

        private void StartCharging()
        {
            if (hasLaunched) return;

            isCharging = true;
            currentCharge = 0f;
            OnChargeStarted?.Invoke();

            Debug.Log("[CatapultController] Charging started");

            // Play charge animation/sound
            Managers.AudioManager.Instance?.PlaySFX("ChargeStart");
        }

        private void UpdateCharge()
        {
            currentCharge += chargeSpeed * Time.deltaTime / maxChargeTime;
            currentCharge = Mathf.Clamp01(currentCharge);

            OnChargeUpdated?.Invoke(currentCharge);

            // Update trajectory preview
            if (showTrajectory)
            {
                DrawTrajectory();
            }

            // Play charging sound (looped)
            // Managers.AudioManager.Instance?.UpdateChargeSound(currentCharge);
        }

        private void ReleaseLaunch()
        {
            if (!isCharging || hasLaunched) return;

            isCharging = false;
            hasLaunched = true;

            Debug.Log($"[CatapultController] Launching with charge: {currentCharge:P2}");

            // Launch the player
            LaunchPlayer(currentCharge);

            OnLaunched?.Invoke(currentCharge);

            // Hide trajectory
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }

            // Play launch sound/VFX
            Managers.AudioManager.Instance?.PlaySFX("Launch");
        }

        #endregion

        #region Aiming

        private void UpdateAim(Vector2 screenPosition)
        {
            // Convert screen position to world and calculate angle
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            Vector2 direction = (worldPos - launchPoint.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentAngle = Mathf.Clamp(angle, minLaunchAngle, maxLaunchAngle);

            // Update catapult arm rotation
            if (catapultArm != null)
            {
                catapultArm.rotation = Quaternion.Euler(0, 0, currentAngle);
            }
        }

        #endregion

        #region Launch

        private void SpawnPlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("[CatapultController] Player prefab not assigned!");
                return;
            }

            spawnedPlayer = Instantiate(playerPrefab, launchPoint.position, Quaternion.identity);
            playerMomentum = spawnedPlayer.GetComponent<MomentumSystem>();

            // Position player on catapult
            spawnedPlayer.transform.SetParent(launchPoint);
            spawnedPlayer.transform.localPosition = Vector3.zero;
        }

        private void LaunchPlayer(float chargeMultiplier)
        {
            if (playerMomentum == null)
            {
                Debug.LogError("[CatapultController] Player MomentumSystem not found!");
                return;
            }

            // Detach player from catapult
            spawnedPlayer.transform.SetParent(null);

            // Launch with momentum system
            playerMomentum.Launch(chargeMultiplier);

            // Start run
            Core.GameManager.Instance.StartRun();

            // Catapult arm recoil animation
            if (catapultArm != null)
            {
                // Trigger animation (requires Animator on catapult)
                var animator = catapultArm.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("Launch");
                }
            }
        }

        #endregion

        #region Trajectory Preview

        private void SetupTrajectoryLine()
        {
            if (trajectoryLine == null)
            {
                trajectoryLine = gameObject.AddComponent<LineRenderer>();
            }

            trajectoryLine.positionCount = trajectoryPoints;
            trajectoryLine.startWidth = 0.1f;
            trajectoryLine.endWidth = 0.05f;
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
            trajectoryLine.startColor = trajectoryColor;
            trajectoryLine.endColor = trajectoryColor;
            trajectoryLine.enabled = false;
        }

        private void DrawTrajectory()
        {
            if (trajectoryLine == null || !showTrajectory) return;

            trajectoryLine.enabled = true;

            // Calculate launch velocity
            float launchPower = playerMomentum != null ? 50f * currentCharge : 50f * currentCharge; // Simplified
            Vector2 launchDirection = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;
            Vector2 launchVelocity = launchDirection * launchPower;

            // Simulate trajectory
            Vector3 startPos = launchPoint.position;
            Vector2 gravity = Physics2D.gravity;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryTimeStep;
                Vector3 pos = startPos + (Vector3)launchVelocity * time + 0.5f * (Vector3)gravity * time * time;
                trajectoryLine.SetPosition(i, pos);

                // Stop if trajectory goes below ground (y < -10)
                if (pos.y < -10f)
                {
                    trajectoryLine.positionCount = i + 1;
                    break;
                }
            }

            // Update color based on charge
            Color color = Color.Lerp(lowChargeColor, fullChargeColor, currentCharge);
            trajectoryLine.startColor = color;
            trajectoryLine.endColor = color;
        }

        #endregion

        #region Visual Feedback

        private void UpdateVisuals()
        {
            // Update charge indicator (if assigned)
            if (chargeIndicator != null && isCharging)
            {
                chargeIndicator.localScale = new Vector3(currentCharge, 1f, 1f);

                // Color feedback
                var spriteRenderer = chargeIndicator.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    if (currentCharge < 0.33f)
                        spriteRenderer.color = lowChargeColor;
                    else if (currentCharge < 0.66f)
                        spriteRenderer.color = mediumChargeColor;
                    else
                        spriteRenderer.color = fullChargeColor;
                }
            }

            // Catapult arm pullback animation
            if (catapultArm != null && isCharging)
            {
                float pullback = Mathf.Lerp(0f, -15f, currentCharge); // Rotate back
                catapultArm.localRotation = Quaternion.Euler(0, 0, currentAngle + pullback);
            }
        }

        #endregion

        #region Reset

        /// <summary>
        /// Resets the catapult for a new run.
        /// </summary>
        public void Reset()
        {
            isCharging = false;
            currentCharge = 0f;
            hasLaunched = false;
            currentAngle = 45f;

            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }

            if (spawnedPlayer != null)
            {
                Destroy(spawnedPlayer);
            }

            SpawnPlayer();

            Debug.Log("[CatapultController] Reset for new run");
        }

        #endregion

        private void OnDestroy()
        {
            // Cleanup input
            if (launchAction != null)
            {
                launchAction.started -= OnLaunchStarted;
                launchAction.canceled -= OnLaunchReleased;
                launchAction.Disable();
            }

            if (aimAction != null)
            {
                aimAction.Disable();
            }
        }

        private void OnDrawGizmos()
        {
            // Debug draw launch direction
            if (launchPoint != null)
            {
                Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(launchPoint.position, direction * 5f);
            }
        }
    }
}
