using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace VikingSiegeBreaker.Editor
{
    /// <summary>
    /// Automated Scene Builder for Viking Siege Breaker
    /// Quickly generates and configures scenes with proper hierarchy and components
    /// Access via: Tools > Viking Siege Breaker > Scene Builder
    /// </summary>
    public class SceneBuilder : EditorWindow
    {
        private const string SCENES_PATH = "Assets/VikingSiegeBreaker/Scenes";
        private const string PREFABS_PATH = "Assets/VikingSiegeBreaker/Prefabs";

        private Vector2 scrollPosition;
        private bool createMainMenu = true;
        private bool createGameplay = true;
        private bool createGameOver = true;
        private bool setupBuildSettings = true;

        [MenuItem("Tools/Viking Siege Breaker/Scene Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneBuilder>("Scene Builder");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Viking Siege Breaker - Scene Builder", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Automated scene generation and setup tool", EditorStyles.miniLabel);
            EditorGUILayout.Space(10);

            DrawSceneOptions();
            EditorGUILayout.Space(10);

            DrawBuildOptions();
            EditorGUILayout.Space(10);

            DrawActionButtons();
            EditorGUILayout.Space(10);

            DrawQuickActions();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneOptions()
        {
            EditorGUILayout.LabelField("Scenes to Create", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select which scenes to generate. Each scene will be populated with required GameObjects and components.", MessageType.Info);

            createMainMenu = EditorGUILayout.ToggleLeft("MainMenu Scene (Main menu, settings, start game)", createMainMenu);
            createGameplay = EditorGUILayout.ToggleLeft("Gameplay Scene (Main game loop, player, enemies)", createGameplay);
            createGameOver = EditorGUILayout.ToggleLeft("GameOver Scene (Results, upgrades, continue)", createGameOver);
        }

        private void DrawBuildOptions()
        {
            EditorGUILayout.LabelField("Build Configuration", EditorStyles.boldLabel);
            setupBuildSettings = EditorGUILayout.ToggleLeft("Add scenes to Build Settings", setupBuildSettings);
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build All Selected Scenes", GUILayout.Height(40)))
            {
                BuildAllScenes();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build MainMenu Only", GUILayout.Height(30)))
            {
                BuildMainMenuScene();
            }
            if (GUILayout.Button("Build Gameplay Only", GUILayout.Height(30)))
            {
                BuildGameplayScene();
            }
            if (GUILayout.Button("Build GameOver Only", GUILayout.Height(30)))
            {
                BuildGameOverScene();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawQuickActions()
        {
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Folder Structure"))
            {
                CreateFolderStructure();
            }

            if (GUILayout.Button("Validate Project Setup"))
            {
                ValidateProjectSetup();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Need help? Check the documentation in SCENE_BUILDER_GUIDE.md", MessageType.None);
        }

        // ========== MAIN BUILD FUNCTIONS ==========

        private void BuildAllScenes()
        {
            if (!EditorUtility.DisplayDialog("Build All Scenes",
                "This will create and configure all selected scenes. Existing scenes will be overwritten. Continue?",
                "Yes, Build!", "Cancel"))
            {
                return;
            }

            CreateFolderStructure();

            if (createMainMenu) BuildMainMenuScene();
            if (createGameplay) BuildGameplayScene();
            if (createGameOver) BuildGameOverScene();

            if (setupBuildSettings)
            {
                SetupBuildSettings();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success!",
                "All scenes have been built successfully!\n\nYou can now open them from:\n" + SCENES_PATH,
                "OK");
        }

        // ========== INDIVIDUAL SCENE BUILDERS ==========

        private void BuildMainMenuScene()
        {
            Debug.Log("[SceneBuilder] Building MainMenu scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            // Core Systems
            CreateGameObject("=== CORE SYSTEMS ===", null);
            var gameManager = CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));
            CreateGameObject("NetworkCheck", typeof(NetworkCheck));

            // Managers
            CreateGameObject("=== MANAGERS ===", null);
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));
            CreateGameObject("AdsManager", typeof(AdsManager));
            CreateGameObject("IAPManager", typeof(IAPManager));

            // UI
            CreateGameObject("=== UI ===", null);
            var canvas = CreateCanvas("MainMenuCanvas");
            CreateGameObject("MenuController", typeof(MenuController), canvas.transform);
            CreateGameObject("SettingsPanel", null, canvas.transform);

            // Camera
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color(0.2f, 0.3f, 0.5f);
            camera.GetComponent<Camera>().orthographic = true;

            // EventSystem
            CreateGameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

            SaveScene(scene, "MainMenu");
            Debug.Log("[SceneBuilder] MainMenu scene created successfully!");
        }

        private void BuildGameplayScene()
        {
            Debug.Log("[SceneBuilder] Building Gameplay scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Gameplay";

            // Core Systems
            CreateGameObject("=== CORE SYSTEMS ===", null);
            CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));

            // Game Systems
            CreateGameObject("=== GAME SYSTEMS ===", null);
            CreateGameObject("SpawnManager", typeof(SpawnManager));
            CreateGameObject("UpgradeManager", typeof(UpgradeManager));
            CreateGameObject("EvolutionManager", typeof(EvolutionManager));
            CreateGameObject("CurrencyManager", typeof(CurrencyManager));

            // Managers
            CreateGameObject("=== MANAGERS ===", null);
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));

            // Player
            CreateGameObject("=== PLAYER ===", null);
            var player = CreateGameObject("Player", typeof(PlayerController), typeof(MomentumSystem),
                typeof(Rigidbody2D), typeof(CircleCollider2D));
            var rb = player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Catapult
            var catapult = CreateGameObject("Catapult", typeof(CatapultController));
            catapult.transform.position = new Vector3(-8f, -3f, 0f);

            // World
            CreateGameObject("=== WORLD ===", null);
            var ground = CreateGameObject("Ground", typeof(BoxCollider2D));
            ground.transform.position = new Vector3(0f, -4f, 0f);
            ground.transform.localScale = new Vector3(100f, 1f, 1f);

            var spawners = CreateGameObject("Spawners", null);
            CreateGameObject("EnemySpawner", null, spawners.transform);
            CreateGameObject("PickupSpawner", null, spawners.transform);
            CreateGameObject("ObstacleSpawner", null, spawners.transform);

            // UI
            CreateGameObject("=== UI ===", null);
            var canvas = CreateCanvas("GameplayCanvas");
            CreateGameObject("HUDController", typeof(HUDController), canvas.transform);
            CreateGameObject("PausePanel", null, canvas.transform);

            // Camera
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            var cam = camera.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.5f, 0.7f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;

            // EventSystem
            CreateGameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

            SaveScene(scene, "Gameplay");
            Debug.Log("[SceneBuilder] Gameplay scene created successfully!");
        }

        private void BuildGameOverScene()
        {
            Debug.Log("[SceneBuilder] Building GameOver scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "GameOver";

            // Core Systems
            CreateGameObject("=== CORE SYSTEMS ===", null);
            CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));

            // Systems
            CreateGameObject("=== SYSTEMS ===", null);
            CreateGameObject("UpgradeManager", typeof(UpgradeManager));
            CreateGameObject("CurrencyManager", typeof(CurrencyManager));

            // Managers
            CreateGameObject("=== MANAGERS ===", null);
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));
            CreateGameObject("AdsManager", typeof(AdsManager));

            // UI
            CreateGameObject("=== UI ===", null);
            var canvas = CreateCanvas("GameOverCanvas");
            CreateGameObject("GameOverPanel", typeof(GameOverPanel), canvas.transform);
            CreateGameObject("UpgradePanel", typeof(UpgradePanel), canvas.transform);
            CreateGameObject("ShopPanel", typeof(ShopPanel), canvas.transform);

            // Camera
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color(0.2f, 0.2f, 0.3f);
            camera.GetComponent<Camera>().orthographic = true;

            // EventSystem
            CreateGameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

            SaveScene(scene, "GameOver");
            Debug.Log("[SceneBuilder] GameOver scene created successfully!");
        }

        // ========== HELPER FUNCTIONS ==========

        private GameObject CreateGameObject(string name, params System.Type[] components)
        {
            return CreateGameObject(name, components, null);
        }

        private GameObject CreateGameObject(string name, System.Type[] components, Transform parent)
        {
            var go = new GameObject(name);

            if (components != null)
            {
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        go.AddComponent(component);
                    }
                }
            }

            if (parent != null)
            {
                go.transform.SetParent(parent);
            }

            return go;
        }

        private GameObject CreateCanvas(string name)
        {
            var canvasGO = new GameObject(name);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            return canvasGO;
        }

        private void SaveScene(Scene scene, string sceneName)
        {
            string path = $"{SCENES_PATH}/{sceneName}.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[SceneBuilder] Scene saved: {path}");
        }

        private void CreateFolderStructure()
        {
            CreateFolderIfNotExists(SCENES_PATH);
            CreateFolderIfNotExists(PREFABS_PATH);
            CreateFolderIfNotExists($"{PREFABS_PATH}/Player");
            CreateFolderIfNotExists($"{PREFABS_PATH}/Enemies");
            CreateFolderIfNotExists($"{PREFABS_PATH}/Pickups");
            CreateFolderIfNotExists($"{PREFABS_PATH}/Environment");
            CreateFolderIfNotExists($"{PREFABS_PATH}/UI");
            CreateFolderIfNotExists($"{PREFABS_PATH}/VFX");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects/Upgrades");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects/Enemies");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects/Pickups");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects/Evolutions");
            CreateFolderIfNotExists("Assets/VikingSiegeBreaker/ScriptableObjects/Settings");

            AssetDatabase.Refresh();
            Debug.Log("[SceneBuilder] Folder structure created!");
        }

        private void CreateFolderIfNotExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private void SetupBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

            if (createMainMenu && File.Exists($"{SCENES_PATH}/MainMenu.unity"))
            {
                scenes.Add(new EditorBuildSettingsScene($"{SCENES_PATH}/MainMenu.unity", true));
            }

            if (createGameplay && File.Exists($"{SCENES_PATH}/Gameplay.unity"))
            {
                scenes.Add(new EditorBuildSettingsScene($"{SCENES_PATH}/Gameplay.unity", true));
            }

            if (createGameOver && File.Exists($"{SCENES_PATH}/GameOver.unity"))
            {
                scenes.Add(new EditorBuildSettingsScene($"{SCENES_PATH}/GameOver.unity", true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[SceneBuilder] Build settings updated with {scenes.Count} scenes!");
        }

        private void ValidateProjectSetup()
        {
            Debug.Log("[SceneBuilder] Validating project setup...");

            int issues = 0;

            // Check for required scripts
            string[] requiredScripts = {
                "GameManager", "SaveSystem", "NetworkCheck",
                "UIManager", "AudioManager", "AdsManager", "IAPManager",
                "PlayerController", "MomentumSystem", "CatapultController",
                "SpawnManager", "UpgradeManager", "EvolutionManager", "CurrencyManager",
                "HUDController", "MenuController", "GameOverPanel", "UpgradePanel", "ShopPanel"
            };

            foreach (var scriptName in requiredScripts)
            {
                var type = System.Type.GetType($"VikingSiegeBreaker.{scriptName}, Assembly-CSharp") ??
                           System.Type.GetType($"{scriptName}, Assembly-CSharp");

                if (type == null)
                {
                    Debug.LogWarning($"[SceneBuilder] Missing script: {scriptName}.cs");
                    issues++;
                }
            }

            // Check folders
            if (!AssetDatabase.IsValidFolder(SCENES_PATH))
            {
                Debug.LogWarning("[SceneBuilder] Scenes folder missing!");
                issues++;
            }

            if (issues == 0)
            {
                EditorUtility.DisplayDialog("Validation Complete",
                    "Project setup looks good! All required components found.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Validation Issues",
                    $"Found {issues} issue(s). Check the Console for details.", "OK");
            }
        }
    }

    // ========== MISSING COMPONENT STUBS ==========
    // These are placeholder classes for components that may not exist yet
    // Replace with actual implementations

    #if !UNITY_EDITOR
    public class GameManager : MonoBehaviour { }
    public class SaveSystem : MonoBehaviour { }
    public class NetworkCheck : MonoBehaviour { }
    public class UIManager : MonoBehaviour { }
    public class AudioManager : MonoBehaviour { }
    public class AdsManager : MonoBehaviour { }
    public class IAPManager : MonoBehaviour { }
    public class MenuController : MonoBehaviour { }
    public class HUDController : MonoBehaviour { }
    public class GameOverPanel : MonoBehaviour { }
    public class UpgradePanel : MonoBehaviour { }
    public class ShopPanel : MonoBehaviour { }
    public class PlayerController : MonoBehaviour { }
    public class MomentumSystem : MonoBehaviour { }
    public class CatapultController : MonoBehaviour { }
    public class SpawnManager : MonoBehaviour { }
    public class UpgradeManager : MonoBehaviour { }
    public class EvolutionManager : MonoBehaviour { }
    public class CurrencyManager : MonoBehaviour { }
    #endif
}
