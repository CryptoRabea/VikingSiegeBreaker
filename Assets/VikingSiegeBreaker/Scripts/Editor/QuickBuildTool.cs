using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace VikingSiegeBreaker.Editor
{
    /// <summary>
    /// Quick Build Tool - One-click scene setup and build automation
    /// Provides menu shortcuts for rapid development and testing
    /// </summary>
    public static class QuickBuildTool
    {
        private const string SCENES_PATH = "Assets/VikingSiegeBreaker/Scenes";

        // ========== MENU ITEMS ==========

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/1. Build All Scenes %&b")]
        public static void QuickBuildAllScenes()
        {
            Debug.Log("[QuickBuild] Starting automated scene build...");

            var window = ScriptableObject.CreateInstance<SceneBuilder>();

            // Build all scenes programmatically
            CreateFolderStructure();
            BuildMainMenu();
            BuildGameplay();
            BuildGameOver();
            SetupBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[QuickBuild] ✓ All scenes built successfully!");
            EditorUtility.DisplayDialog("Quick Build Complete",
                "All scenes have been created and configured!\n\n" +
                "Next steps:\n" +
                "• Open MainMenu scene to start testing\n" +
                "• Run 'Tools > Quick Build > Test Build' to verify",
                "Got it!");
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/2. Open MainMenu Scene %&1")]
        public static void OpenMainMenuScene()
        {
            OpenScene("MainMenu");
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/3. Open Gameplay Scene %&2")]
        public static void OpenGameplayScene()
        {
            OpenScene("Gameplay");
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/4. Open GameOver Scene %&3")]
        public static void OpenGameOverScene()
        {
            OpenScene("GameOver");
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/5. Play from MainMenu %&p")]
        public static void PlayFromMainMenu()
        {
            if (OpenScene("MainMenu"))
            {
                EditorApplication.isPlaying = true;
            }
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/---")]
        private static void Separator1() { }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/Test Build (Validate Setup)")]
        public static void TestBuild()
        {
            Debug.Log("[QuickBuild] Running validation tests...");

            int passed = 0;
            int failed = 0;

            // Test 1: Check scenes exist
            if (TestSceneExists("MainMenu")) passed++; else failed++;
            if (TestSceneExists("Gameplay")) passed++; else failed++;
            if (TestSceneExists("GameOver")) passed++; else failed++;

            // Test 2: Check build settings
            if (TestBuildSettings()) passed++; else failed++;

            // Test 3: Check scripts compile
            if (TestScriptsCompile()) passed++; else failed++;

            // Results
            string result = $"Build Validation Results:\n\n" +
                          $"✓ Passed: {passed}\n" +
                          $"✗ Failed: {failed}\n\n";

            if (failed == 0)
            {
                result += "All tests passed! Project is ready for building.";
                Debug.Log("[QuickBuild] ✓ All validation tests passed!");
                EditorUtility.DisplayDialog("Test Build - Success", result, "Excellent!");
            }
            else
            {
                result += "Some tests failed. Check the Console for details.";
                Debug.LogWarning("[QuickBuild] ⚠ Some validation tests failed!");
                EditorUtility.DisplayDialog("Test Build - Issues Found", result, "OK");
            }
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/Create ScriptableObjects")]
        public static void CreateDefaultScriptableObjects()
        {
            Debug.Log("[QuickBuild] Creating default ScriptableObjects...");

            CreateFolderStructure();
            CreateDefaultGameSettings();
            CreateSampleUpgrades();
            CreateSampleEnemies();
            CreateSamplePickups();
            CreateSampleEvolutions();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[QuickBuild] ✓ ScriptableObjects created!");
            EditorUtility.DisplayDialog("ScriptableObjects Created",
                "Default ScriptableObjects have been created!\n\n" +
                "Find them in:\nAssets/VikingSiegeBreaker/ScriptableObjects/",
                "OK");
        }

        [MenuItem("Tools/Viking Siege Breaker/Quick Build/Clean All (Reset Project)")]
        public static void CleanAll()
        {
            if (!EditorUtility.DisplayDialog("Clean All",
                "This will DELETE all generated scenes and ScriptableObjects!\n\n" +
                "Are you sure you want to continue?",
                "Yes, Delete Everything", "Cancel"))
            {
                return;
            }

            Debug.Log("[QuickBuild] Cleaning project...");

            // Delete scenes
            DeleteIfExists($"{SCENES_PATH}/MainMenu.unity");
            DeleteIfExists($"{SCENES_PATH}/Gameplay.unity");
            DeleteIfExists($"{SCENES_PATH}/GameOver.unity");

            // Delete ScriptableObjects
            DeleteFolder("Assets/VikingSiegeBreaker/ScriptableObjects");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[QuickBuild] ✓ Project cleaned!");
            EditorUtility.DisplayDialog("Clean Complete", "All generated files have been deleted.", "OK");
        }

        // ========== SCENE BUILDERS ==========

        private static void BuildMainMenu()
        {
            Debug.Log("[QuickBuild] Building MainMenu scene...");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Setup scene hierarchy
            CreateSectionHeader("CORE SYSTEMS");
            CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));
            CreateGameObject("NetworkCheck", typeof(NetworkCheck));

            CreateSectionHeader("MANAGERS");
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));
            CreateGameObject("AdsManager", typeof(AdsManager));
            CreateGameObject("IAPManager", typeof(IAPManager));

            CreateSectionHeader("UI");
            var canvas = CreateCanvas("MainMenuCanvas");
            CreateGameObject("MenuController", typeof(MenuController), canvas.transform);

            CreateSectionHeader("CAMERA");
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color(0.2f, 0.3f, 0.5f);
            camera.GetComponent<Camera>().orthographic = true;

            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/MainMenu.unity");
        }

        private static void BuildGameplay()
        {
            Debug.Log("[QuickBuild] Building Gameplay scene...");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateSectionHeader("CORE SYSTEMS");
            CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));

            CreateSectionHeader("GAME SYSTEMS");
            CreateGameObject("SpawnManager", typeof(SpawnManager));
            CreateGameObject("UpgradeManager", typeof(UpgradeManager));
            CreateGameObject("EvolutionManager", typeof(EvolutionManager));
            CreateGameObject("CurrencyManager", typeof(CurrencyManager));

            CreateSectionHeader("MANAGERS");
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));

            CreateSectionHeader("PLAYER");
            var player = CreateGameObject("Player", typeof(PlayerController), typeof(MomentumSystem),
                typeof(Rigidbody2D), typeof(CircleCollider2D));
            var rb = player.GetComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var catapult = CreateGameObject("Catapult", typeof(CatapultController));
            catapult.transform.position = new Vector3(-8f, -3f, 0f);

            CreateSectionHeader("WORLD");
            var ground = CreateGameObject("Ground", typeof(BoxCollider2D));
            ground.transform.position = new Vector3(0f, -4f, 0f);
            ground.transform.localScale = new Vector3(100f, 1f, 1f);

            CreateSectionHeader("UI");
            var canvas = CreateCanvas("GameplayCanvas");
            CreateGameObject("HUDController", typeof(HUDController), canvas.transform);

            CreateSectionHeader("CAMERA");
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            var cam = camera.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.5f, 0.7f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;

            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/Gameplay.unity");
        }

        private static void BuildGameOver()
        {
            Debug.Log("[QuickBuild] Building GameOver scene...");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateSectionHeader("CORE SYSTEMS");
            CreateGameObject("GameManager", typeof(GameManager));
            CreateGameObject("SaveSystem", typeof(SaveSystem));

            CreateSectionHeader("SYSTEMS");
            CreateGameObject("UpgradeManager", typeof(UpgradeManager));
            CreateGameObject("CurrencyManager", typeof(CurrencyManager));

            CreateSectionHeader("MANAGERS");
            CreateGameObject("UIManager", typeof(UIManager));
            CreateGameObject("AudioManager", typeof(AudioManager));
            CreateGameObject("AdsManager", typeof(AdsManager));

            CreateSectionHeader("UI");
            var canvas = CreateCanvas("GameOverCanvas");
            CreateGameObject("GameOverPanel", typeof(GameOverPanel), canvas.transform);
            CreateGameObject("UpgradePanel", typeof(UpgradePanel), canvas.transform);
            CreateGameObject("ShopPanel", typeof(ShopPanel), canvas.transform);

            CreateSectionHeader("CAMERA");
            var camera = CreateGameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color(0.2f, 0.2f, 0.3f);
            camera.GetComponent<Camera>().orthographic = true;

            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/GameOver.unity");
        }

        // ========== SCRIPTABLEOBJECT CREATORS ==========

        private static void CreateDefaultGameSettings()
        {
            string path = "Assets/VikingSiegeBreaker/ScriptableObjects/Settings/GameSettings.asset";
            if (File.Exists(path)) return;

            var settings = ScriptableObject.CreateInstance<GameSettings>();
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"[QuickBuild] Created: {path}");
        }

        private static void CreateSampleUpgrades()
        {
            string basePath = "Assets/VikingSiegeBreaker/ScriptableObjects/Upgrades";
            string[] upgrades = { "LaunchPower", "MaxHealth", "MomentumRegen", "CoinMultiplier" };

            foreach (var upgrade in upgrades)
            {
                string path = $"{basePath}/{upgrade}.asset";
                if (File.Exists(path)) continue;

                var data = ScriptableObject.CreateInstance<UpgradeData>();
                AssetDatabase.CreateAsset(data, path);
            }

            Debug.Log($"[QuickBuild] Created {upgrades.Length} sample upgrades");
        }

        private static void CreateSampleEnemies()
        {
            string basePath = "Assets/VikingSiegeBreaker/ScriptableObjects/Enemies";
            string[] enemies = { "BasicSoldier", "HeavyKnight", "Archer" };

            foreach (var enemy in enemies)
            {
                string path = $"{basePath}/{enemy}.asset";
                if (File.Exists(path)) continue;

                var data = ScriptableObject.CreateInstance<EnemyData>();
                AssetDatabase.CreateAsset(data, path);
            }

            Debug.Log($"[QuickBuild] Created {enemies.Length} sample enemies");
        }

        private static void CreateSamplePickups()
        {
            string basePath = "Assets/VikingSiegeBreaker/ScriptableObjects/Pickups";
            string[] pickups = { "Meat", "Shield", "Coin", "Gem" };

            foreach (var pickup in pickups)
            {
                string path = $"{basePath}/{pickup}.asset";
                if (File.Exists(path)) continue;

                var data = ScriptableObject.CreateInstance<PickupData>();
                AssetDatabase.CreateAsset(data, path);
            }

            Debug.Log($"[QuickBuild] Created {pickups.Length} sample pickups");
        }

        private static void CreateSampleEvolutions()
        {
            string basePath = "Assets/VikingSiegeBreaker/ScriptableObjects/Evolutions";
            string[] evolutions = { "VikingWarrior", "BerserkerChief", "LegendaryKing" };

            foreach (var evolution in evolutions)
            {
                string path = $"{basePath}/{evolution}.asset";
                if (File.Exists(path)) continue;

                var data = ScriptableObject.CreateInstance<EvolutionData>();
                AssetDatabase.CreateAsset(data, path);
            }

            Debug.Log($"[QuickBuild] Created {evolutions.Length} sample evolutions");
        }

        // ========== HELPER FUNCTIONS ==========

        private static GameObject CreateSectionHeader(string name)
        {
            var go = new GameObject($"=== {name} ===");
            return go;
        }

        private static GameObject CreateGameObject(string name, params System.Type[] components)
        {
            return CreateGameObject(name, components, null);
        }

        private static GameObject CreateGameObject(string name, System.Type[] components, Transform parent)
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

        private static GameObject CreateCanvas(string name)
        {
            var canvasGO = new GameObject(name);
            var canvas = canvasGO.AddComponent<UnityEngine.UI.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvasGO;
        }

        private static void CreateEventSystem()
        {
            CreateGameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        private static void CreateFolderStructure()
        {
            CreateFolder(SCENES_PATH);
            CreateFolder("Assets/VikingSiegeBreaker/Prefabs");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects/Upgrades");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects/Enemies");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects/Pickups");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects/Evolutions");
            CreateFolder("Assets/VikingSiegeBreaker/ScriptableObjects/Settings");
        }

        private static void CreateFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static bool OpenScene(string sceneName)
        {
            string path = $"{SCENES_PATH}/{sceneName}.unity";

            if (!File.Exists(path))
            {
                Debug.LogError($"[QuickBuild] Scene not found: {path}");
                EditorUtility.DisplayDialog("Scene Not Found",
                    $"Scene '{sceneName}' doesn't exist!\n\nRun 'Quick Build > Build All Scenes' first.",
                    "OK");
                return false;
            }

            EditorSceneManager.OpenScene(path);
            Debug.Log($"[QuickBuild] Opened scene: {sceneName}");
            return true;
        }

        private static void SetupBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene($"{SCENES_PATH}/MainMenu.unity", true),
                new EditorBuildSettingsScene($"{SCENES_PATH}/Gameplay.unity", true),
                new EditorBuildSettingsScene($"{SCENES_PATH}/GameOver.unity", true)
            };

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[QuickBuild] Build settings configured with 3 scenes");
        }

        // ========== VALIDATION TESTS ==========

        private static bool TestSceneExists(string sceneName)
        {
            string path = $"{SCENES_PATH}/{sceneName}.unity";
            bool exists = File.Exists(path);

            if (exists)
                Debug.Log($"[QuickBuild] ✓ Scene exists: {sceneName}");
            else
                Debug.LogError($"[QuickBuild] ✗ Scene missing: {sceneName}");

            return exists;
        }

        private static bool TestBuildSettings()
        {
            var buildScenes = EditorBuildSettings.scenes;
            bool valid = buildScenes != null && buildScenes.Length >= 3;

            if (valid)
                Debug.Log($"[QuickBuild] ✓ Build settings configured ({buildScenes.Length} scenes)");
            else
                Debug.LogError("[QuickBuild] ✗ Build settings not configured");

            return valid;
        }

        private static bool TestScriptsCompile()
        {
            // Check if there are any compilation errors
            bool hasErrors = EditorUtility.scriptCompilationFailed;

            if (!hasErrors)
                Debug.Log("[QuickBuild] ✓ All scripts compile successfully");
            else
                Debug.LogError("[QuickBuild] ✗ Script compilation errors detected");

            return !hasErrors;
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
                Debug.Log($"[QuickBuild] Deleted: {path}");
            }
        }

        private static void DeleteFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.DeleteAsset(path);
                Debug.Log($"[QuickBuild] Deleted folder: {path}");
            }
        }
    }
}
