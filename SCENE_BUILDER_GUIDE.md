# 🏗️ Scene Builder Guide - Viking Siege Breaker

**Automated Scene Setup & Build Tool**

---

## 📋 Overview

The Scene Builder is an automated tool that creates and configures all Unity scenes for Viking Siege Breaker with a single click. It eliminates manual scene setup, ensuring consistency and saving hours of development time.

### Features
- ✅ **One-Click Scene Generation** - Build all scenes instantly
- ✅ **Auto-Configured Hierarchy** - GameObjects with proper components
- ✅ **Build Settings Integration** - Automatically adds scenes to build
- ✅ **ScriptableObject Creation** - Generates default data assets
- ✅ **Validation Tools** - Verify project setup before testing
- ✅ **Quick Access Shortcuts** - Keyboard shortcuts for rapid workflow

---

## 🚀 Quick Start (60 Seconds)

### Step 1: Build All Scenes (5 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → 1. Build All Scenes
```
Or press: **Ctrl+Alt+B** (Windows) / **Cmd+Alt+B** (Mac)

**This will:**
- ✓ Create MainMenu.unity
- ✓ Create Gameplay.unity
- ✓ Create GameOver.unity
- ✓ Set up scene hierarchies
- ✓ Configure build settings

### Step 2: Create ScriptableObjects (5 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → Create ScriptableObjects
```

**This will:**
- ✓ Create sample Upgrades (LaunchPower, MaxHealth, etc.)
- ✓ Create sample Enemies (BasicSoldier, HeavyKnight, etc.)
- ✓ Create sample Pickups (Meat, Shield, Coin, Gem)
- ✓ Create sample Evolutions (VikingWarrior, BerserkerChief, etc.)

### Step 3: Test (10 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → 5. Play from MainMenu
```
Or press: **Ctrl+Alt+P** (Windows) / **Cmd+Alt+P** (Mac)

**Done!** Your project is now fully set up and ready for development! 🎉

---

## 🛠️ Tools Available

### 1. Scene Builder Window (Advanced Options)
```
Tools → Viking Siege Breaker → Scene Builder
```

**GUI Interface with options:**
- Select which scenes to build
- Configure build settings
- Individual scene builders
- Folder structure creation
- Project validation

### 2. Quick Build Menu (Fast Workflow)
```
Tools → Viking Siege Breaker → Quick Build
```

| Menu Item | Shortcut | Description |
|-----------|----------|-------------|
| **1. Build All Scenes** | Ctrl+Alt+B | Creates all three scenes |
| **2. Open MainMenu Scene** | Ctrl+Alt+1 | Opens MainMenu.unity |
| **3. Open Gameplay Scene** | Ctrl+Alt+2 | Opens Gameplay.unity |
| **4. Open GameOver Scene** | Ctrl+Alt+3 | Opens GameOver.unity |
| **5. Play from MainMenu** | Ctrl+Alt+P | Opens MainMenu and hits Play |
| **Test Build (Validate Setup)** | - | Runs validation tests |
| **Create ScriptableObjects** | - | Generates sample data assets |
| **Clean All (Reset Project)** | - | Deletes all generated files |

---

## 📁 Generated Scene Structure

### MainMenu Scene
```
MainMenu
├── === CORE SYSTEMS ===
│   ├── GameManager
│   ├── SaveSystem
│   └── NetworkCheck
├── === MANAGERS ===
│   ├── UIManager
│   ├── AudioManager
│   ├── AdsManager
│   └── IAPManager
├── === UI ===
│   ├── MainMenuCanvas
│   │   └── MenuController
│   └── EventSystem
└── === CAMERA ===
    └── Main Camera
```

### Gameplay Scene
```
Gameplay
├── === CORE SYSTEMS ===
│   ├── GameManager
│   └── SaveSystem
├── === GAME SYSTEMS ===
│   ├── SpawnManager
│   ├── UpgradeManager
│   ├── EvolutionManager
│   └── CurrencyManager
├── === MANAGERS ===
│   ├── UIManager
│   └── AudioManager
├── === PLAYER ===
│   ├── Player (Rigidbody2D, Collider, PlayerController, MomentumSystem)
│   └── Catapult (CatapultController)
├── === WORLD ===
│   └── Ground (BoxCollider2D)
├── === UI ===
│   ├── GameplayCanvas
│   │   └── HUDController
│   └── EventSystem
└── === CAMERA ===
    └── Main Camera
```

### GameOver Scene
```
GameOver
├── === CORE SYSTEMS ===
│   ├── GameManager
│   └── SaveSystem
├── === SYSTEMS ===
│   ├── UpgradeManager
│   └── CurrencyManager
├── === MANAGERS ===
│   ├── UIManager
│   ├── AudioManager
│   └── AdsManager
├── === UI ===
│   ├── GameOverCanvas
│   │   ├── GameOverPanel
│   │   ├── UpgradePanel
│   │   └── ShopPanel
│   └── EventSystem
└── === CAMERA ===
    └── Main Camera
```

---

## 🎯 Common Workflows

### Workflow 1: Fresh Project Setup
**Scenario:** You've cloned the repo and need to set up scenes for the first time.

```
1. Tools → Quick Build → Build All Scenes (Ctrl+Alt+B)
2. Tools → Quick Build → Create ScriptableObjects
3. Tools → Quick Build → Test Build (Validate Setup)
4. Tools → Quick Build → Play from MainMenu (Ctrl+Alt+P)
```

**Time:** ~20 seconds

---

### Workflow 2: Iterate on MainMenu
**Scenario:** You're designing the main menu UI and need to test frequently.

```
1. Tools → Quick Build → Open MainMenu Scene (Ctrl+Alt+1)
2. Make changes
3. Tools → Quick Build → Play from MainMenu (Ctrl+Alt+P)
```

**Time:** ~5 seconds per iteration

---

### Workflow 3: Rebuild Single Scene
**Scenario:** Your Gameplay scene got corrupted and you want to rebuild it.

```
1. Open Scene Builder window
2. Uncheck MainMenu and GameOver
3. Check Gameplay only
4. Click "Build Gameplay Only"
```

**Time:** ~10 seconds

---

### Workflow 4: Clean Reset
**Scenario:** You want to start fresh and delete all generated files.

```
1. Tools → Quick Build → Clean All (Reset Project)
2. Confirm deletion
3. Tools → Quick Build → Build All Scenes
4. Tools → Quick Build → Create ScriptableObjects
```

**Time:** ~30 seconds

---

## 🧪 Validation & Testing

### Test Build Feature
Runs automated checks to verify project setup:

```
Tools → Quick Build → Test Build (Validate Setup)
```

**Tests performed:**
1. ✓ MainMenu scene exists
2. ✓ Gameplay scene exists
3. ✓ GameOver scene exists
4. ✓ Build settings configured
5. ✓ All scripts compile successfully

**Results:**
- **All tests passed** → Project ready for development ✅
- **Some tests failed** → Check Console for details ⚠️

---

## 📂 Folder Structure Created

The Scene Builder automatically creates this folder structure:

```
Assets/VikingSiegeBreaker/
├── Scenes/                          ← Scene files (.unity)
│   ├── MainMenu.unity
│   ├── Gameplay.unity
│   └── GameOver.unity
├── Prefabs/                         ← Prefab assets
│   ├── Player/
│   ├── Enemies/
│   ├── Pickups/
│   ├── Environment/
│   ├── UI/
│   └── VFX/
└── ScriptableObjects/               ← Data assets
    ├── Upgrades/
    │   ├── LaunchPower.asset
    │   ├── MaxHealth.asset
    │   ├── MomentumRegen.asset
    │   └── CoinMultiplier.asset
    ├── Enemies/
    │   ├── BasicSoldier.asset
    │   ├── HeavyKnight.asset
    │   └── Archer.asset
    ├── Pickups/
    │   ├── Meat.asset
    │   ├── Shield.asset
    │   ├── Coin.asset
    │   └── Gem.asset
    ├── Evolutions/
    │   ├── VikingWarrior.asset
    │   ├── BerserkerChief.asset
    │   └── LegendaryKing.asset
    └── Settings/
        └── GameSettings.asset
```

---

## ⚙️ Customization

### Modifying Scene Templates

To customize what gets created in scenes, edit the builder scripts:

**File:** `Assets/VikingSiegeBreaker/Scripts/Editor/QuickBuildTool.cs`

**Example: Add a new GameObject to Gameplay scene**
```csharp
private static void BuildGameplay()
{
    // ... existing code ...

    CreateSectionHeader("MY CUSTOM SECTION");
    CreateGameObject("MyCustomObject", typeof(MyCustomComponent));

    // ... rest of code ...
}
```

### Adding New ScriptableObjects

**Example: Create a new PowerUp type**
```csharp
private static void CreateSamplePowerUps()
{
    string basePath = "Assets/VikingSiegeBreaker/ScriptableObjects/PowerUps";
    string[] powerups = { "SpeedBoost", "Invincibility", "DoubleCoins" };

    foreach (var powerup in powerups)
    {
        string path = $"{basePath}/{powerup}.asset";
        if (File.Exists(path)) continue;

        var data = ScriptableObject.CreateInstance<PowerUpData>();
        AssetDatabase.CreateAsset(data, path);
    }
}
```

Then call it from `CreateDefaultScriptableObjects()`:
```csharp
public static void CreateDefaultScriptableObjects()
{
    // ... existing code ...
    CreateSamplePowerUps(); // Add this line
}
```

---

## 🔧 Troubleshooting

### Problem: "Scene not found" error when opening scenes
**Solution:**
```
1. Tools → Quick Build → Build All Scenes
2. Check that Assets/VikingSiegeBreaker/Scenes/ exists
3. Verify files were created in Project window
```

---

### Problem: Build Settings not configured
**Solution:**
```
1. Tools → Quick Build → Build All Scenes
2. Or manually: File → Build Settings → Add Open Scenes
```

---

### Problem: Missing scripts on GameObjects
**Solution:**
```
1. Ensure all C# scripts are in correct folders
2. Check Console for compilation errors
3. Run: Tools → Quick Build → Test Build (Validate Setup)
```

---

### Problem: ScriptableObjects not created
**Solution:**
```
1. Tools → Quick Build → Create ScriptableObjects
2. Check Assets/VikingSiegeBreaker/ScriptableObjects/ in Project window
3. Verify no compilation errors in Console
```

---

### Problem: "Clean All" deleted everything, how to restore?
**Solution:**
```
1. Don't panic! Scripts are safe (only scenes/assets deleted)
2. Tools → Quick Build → Build All Scenes
3. Tools → Quick Build → Create ScriptableObjects
4. Everything restored in ~30 seconds
```

---

## 🎓 Best Practices

### 1. Use Quick Build shortcuts
Learn the keyboard shortcuts for maximum productivity:
- **Ctrl+Alt+B** - Build all scenes
- **Ctrl+Alt+1/2/3** - Switch between scenes
- **Ctrl+Alt+P** - Play from MainMenu

### 2. Validate before building
Always run **Test Build** before creating a build for distribution:
```
Tools → Quick Build → Test Build (Validate Setup)
```

### 3. Keep scenes organized
The scene builder creates section headers (=== CORE SYSTEMS ===) to keep hierarchy organized. Maintain this structure when adding new GameObjects.

### 4. Don't manually edit generated files (unless needed)
If you need to customize, either:
- **Option A:** Modify the builder scripts (persistent changes)
- **Option B:** Edit generated scenes (one-time changes)

### 5. Commit scenes to version control
After running Scene Builder, commit the generated .unity files:
```bash
git add Assets/VikingSiegeBreaker/Scenes/*.unity
git commit -m "feat: Add generated scene files"
```

---

## 🚀 Advanced Features

### Programmatic Scene Building
You can call the build functions from code:

```csharp
using VikingSiegeBreaker.Editor;

// Build all scenes programmatically
QuickBuildTool.QuickBuildAllScenes();

// Open a specific scene
QuickBuildTool.OpenGameplayScene();

// Validate project
QuickBuildTool.TestBuild();
```

### Batch Operations
Combine multiple operations:

```csharp
// Complete setup from scratch
QuickBuildTool.QuickBuildAllScenes();
QuickBuildTool.CreateDefaultScriptableObjects();
QuickBuildTool.TestBuild();
QuickBuildTool.PlayFromMainMenu();
```

---

## 📊 Build Time Estimates

| Operation | Time | Description |
|-----------|------|-------------|
| Build All Scenes | ~5s | Creates 3 scenes with hierarchies |
| Create ScriptableObjects | ~3s | Generates ~15 data assets |
| Test Build | ~2s | Runs 5 validation tests |
| Clean All | ~1s | Deletes generated files |
| **Total Fresh Setup** | **~11s** | Complete project setup |

---

## 🎯 What's Next?

After running the Scene Builder:

1. **✓ Scenes are ready** - MainMenu, Gameplay, GameOver
2. **✓ Hierarchy is set up** - All managers and systems in place
3. **✓ Build settings configured** - Ready to build
4. **✓ Sample data created** - ScriptableObjects for testing

### Next Steps:
1. **Add UI elements** - Design buttons, panels, HUD in Canvas
2. **Create prefabs** - Player, enemies, pickups
3. **Configure ScriptableObjects** - Set actual values for upgrades, enemies
4. **Add art assets** - Sprites, animations, VFX
5. **Test gameplay** - Play from MainMenu, test core loop
6. **Build for Android** - File → Build Settings → Build

---

## 📚 Related Documentation

- **PROJECT_STRUCTURE.md** - Detailed folder organization
- **UNITY_SETUP_GUIDE.md** - Complete Unity setup instructions
- **UPGRADE_FORMULAS_AND_TEST_DATA.md** - Progression balancing

---

## 🙋 FAQ

**Q: Do I need to run Scene Builder every time I open Unity?**
A: No! Run it once to generate scenes. They persist in your project.

**Q: Can I modify generated scenes?**
A: Yes! Edit them like any Unity scene. Scene Builder just creates the initial setup.

**Q: What if I accidentally delete a scene?**
A: Just run the scene builder again for that specific scene.

**Q: Can I use this on an existing project?**
A: Yes, but it will **overwrite** existing scenes with the same names. Backup first!

**Q: Does this work with Unity 2022 / 2021?**
A: Designed for Unity 6.2+, but should work on 2022+. Some features may differ.

**Q: Can I add my own custom GameObjects to the templates?**
A: Yes! Edit `QuickBuildTool.cs` and add your objects to the build functions.

---

## 🎉 Summary

The Scene Builder saves you hours of manual setup. With a single click:
- ✅ All scenes created and configured
- ✅ Proper hierarchy with all managers
- ✅ Build settings ready
- ✅ Sample data generated
- ✅ Ready for testing and development

**Total setup time: ~11 seconds** ⚡

Happy building! 🏗️⚔️

---

**Version:** 1.0.0
**Last Updated:** 2025
**Compatible with:** Unity 6.2+
