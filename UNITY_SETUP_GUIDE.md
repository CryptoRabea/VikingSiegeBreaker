# Viking Siege Breaker - Unity Setup Guide

Complete step-by-step setup instructions for Unity 6.2.

---

## 📋 Prerequisites

- **Unity 6.2+** (Unity 6.2.0 or newer)
- **Visual Studio 2022** or **Rider** (recommended)
- **Android Build Support** module installed
- **Good understanding of Unity basics**

---

## 1️⃣ Package Installation

### Required Packages

Install via **Window > Package Manager**:

1. **Input System** (com.unity.inputsystem)
   - Version: 1.7.0+
   - Enables new Input System for touch/gamepad support

2. **TextMeshPro** (com.unity.textmeshpro)
   - Version: 3.2.0+
   - Required for all UI text

3. **Unity IAP** (com.unity.purchasing)
   - Version: 4.10.0+
   - For in-app purchases

4. **2D Sprite** (com.unity.2d.sprite)
   - For 2D graphics

5. **2D Physics** (com.unity.2d.common)
   - For collision detection

### Optional Packages

- **LevelPlay SDK** (ironSource) - Download from [ironSource](https://developers.is.com/ironsource-mobile/unity/unity-plugin/)
- **Unity Ads** (alternative to LevelPlay)

### Post-Installation

After installing Input System, Unity will prompt to enable the new backend:
- Click **Yes** to restart with new Input System enabled

---

## 2️⃣ Project Settings

### Player Settings

Navigate to **Edit > Project Settings > Player**:

#### General
- **Company Name**: Your Studio Name
- **Product Name**: Viking Siege Breaker
- **Version**: 1.0.0
- **Default Icon**: Set your app icon

#### Android Settings
- **Package Name**: `com.yourcompany.vikingsiegebreaker`
- **Minimum API Level**: Android 7.0 (API Level 24)
- **Target API Level**: Android 14 (API Level 34) or latest
- **Scripting Backend**: IL2CPP (for better performance)
- **Target Architectures**: ✅ ARM64 (required for Google Play)
- **Internet Access**: Require

#### Graphics
- **Color Space**: Linear (recommended for better visuals)
- **Graphics API**: OpenGLES3, Vulkan

#### Other Settings
- **Scripting Define Symbols**: Add `DEVELOPMENT_BUILD` for debug builds
- **Managed Stripping Level**: Low (to avoid IAP issues)

### Input System Settings

Navigate to **Edit > Project Settings > Input System Package**:

- **Update Mode**: Process Events In Dynamic Update
- **Compensate For Screen Orientation**: ✅ Enabled

### Physics 2D Settings

Navigate to **Edit > Project Settings > Physics 2D**:

#### Layer Collision Matrix
Configure layer collisions:

| Layer | Player | Enemy | Ground | Wall | Pickup | Projectile |
|-------|--------|-------|--------|------|--------|------------|
| Player | ❌ | ✅ | ✅ | ✅ | ✅ | ❌ |
| Enemy | ✅ | ❌ | ✅ | ✅ | ❌ | ✅ |
| Ground | ✅ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Wall | ✅ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Pickup | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

#### Physics Settings
- **Gravity**: -9.81 (Y-axis)
- **Default Contact Offset**: 0.01

---

## 3️⃣ Input Actions Setup

### Create Input Actions Asset

1. **Create Input Actions**:
   - Right-click in `Assets/VikingSiegeBreaker/Resources/`
   - Create > Input Actions
   - Name it: `PlayerInputActions`

2. **Add Action Maps**:

   **Player Actions**:
   - `Launch` (Button) - Touch/Click to launch
     - Binding: `<Mouse>/leftButton`
     - Binding: `<Touchscreen>/primaryTouch/press`

   - `Aim` (Value: Vector2) - Aim direction
     - Binding: `<Mouse>/position`
     - Binding: `<Touchscreen>/primaryTouch/position`

   - `Dash` (Button) - Dash ability
     - Binding: `<Mouse>/leftButton`
     - Binding: `<Touchscreen>/touch0/press`

3. **Generate C# Class**:
   - Select the Input Actions asset
   - Inspector > Check "Generate C# Class"
   - Click "Apply"

---

## 4️⃣ Scene Setup

### Scene Structure

Create **3 scenes** in `Assets/VikingSiegeBreaker/Scenes/`:

1. **MainMenu.unity**
2. **Gameplay.unity**
3. **GameOver.unity** (optional, can be a popup in Gameplay)

### Build Settings

Add scenes to build:
- **File > Build Settings**
- Add scenes in order:
  1. MainMenu
  2. Gameplay
  3. GameOver

---

### MainMenu Scene Setup

#### Create UI Canvas

1. **Create Canvas**:
   - Right-click Hierarchy > UI > Canvas
   - Name: `MainMenuCanvas`

2. **Canvas Scaler**:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1080 x 1920** (portrait)
   - Match: **0.5** (balanced)

3. **UI Elements**:
   ```
   MainMenuCanvas
   ├── Background (Image)
   ├── Title (TextMeshPro)
   ├── PlayButton (Button)
   ├── UpgradesButton (Button)
   ├── ShopButton (Button)
   ├── SettingsButton (Button)
   ├── CurrencyPanel
   │   ├── CoinsText (TMP)
   │   └── GemsText (TMP)
   └── StatsPanel
       ├── EraText (TMP)
       └── BestDistanceText (TMP)
   ```

4. **Attach Scripts**:
   - Canvas → Add `MenuController.cs`
   - PlayButton → OnClick → UIManager.OnPlayButtonClicked
   - UpgradesButton → OnClick → UIManager.OnUpgradesButtonClicked
   - ShopButton → OnClick → UIManager.OnShopButtonClicked

#### Create Manager GameObjects

```
Hierarchy:
├── GameManager (empty GameObject)
│   └── Add GameManager.cs
├── SaveSystem (empty GameObject)
│   └── Add SaveSystem.cs
├── NetworkCheck (empty GameObject)
│   └── Add NetworkCheck.cs
├── CurrencyManager (empty GameObject)
│   └── Add CurrencyManager.cs
├── UpgradeManager (empty GameObject)
│   └── Add UpgradeManager.cs
├── EvolutionManager (empty GameObject)
│   └── Add EvolutionManager.cs
├── UIManager (empty GameObject)
│   └── Add UIManager.cs
├── AudioManager (empty GameObject)
│   └── Add AudioManager.cs
├── AdsManager (empty GameObject)
│   └── Add AdsManager.cs
├── IAPManager (empty GameObject)
│   └── Add IAPManager.cs
└── DebugTools (empty GameObject)
    └── Add DebugTools.cs
```

**Note**: These managers use `DontDestroyOnLoad`, so they persist across scenes.

---

### Gameplay Scene Setup

#### Create Catapult

1. **Create Catapult GameObject**:
   ```
   Catapult
   ├── CatapultArm (Sprite/Model)
   ├── LaunchPoint (Empty Transform - where player spawns)
   └── TrajectoryLine (LineRenderer)
   ```

2. **Attach Script**:
   - Catapult → Add `CatapultController.cs`
   - Assign Player Prefab
   - Assign LaunchPoint transform
   - Assign TrajectoryLine

#### Create Ground/World

1. **Ground**:
   - Create 2D Sprite or Tilemap
   - Add BoxCollider2D or TilemapCollider2D
   - Layer: `Ground`
   - Tag: `Ground`

2. **Walls/Obstacles**:
   - Create castle walls, towers, etc.
   - Add Collider2D
   - Layer: `Wall`
   - Tag: `Wall`

#### Create HUD

```
GameplayCanvas (Canvas)
├── HUD Panel
│   ├── MomentumBar (Slider)
│   ├── SpeedText (TMP)
│   ├── DistanceText (TMP)
│   ├── CoinsText (TMP)
│   ├── HealthBar (Slider)
│   ├── XPBar (Slider)
│   ├── AbilityIcons
│   │   ├── DashIcon (Image)
│   │   └── ShieldIcon (Image)
│   └── PauseButton (Button)
└── GameOverPanel
    ├── StatsDisplay
    ├── ReviveButton (Button)
    ├── RestartButton (Button)
    └── MenuButton (Button)
```

Attach scripts:
- HUD Panel → `HUDController.cs`
- GameOverPanel → `GameOverPanel.cs`

#### Create Spawn Manager

```
Hierarchy:
└── SpawnManager (empty GameObject)
    └── Add SpawnManager.cs
```

Assign:
- Enemy prefabs array
- Obstacle prefabs array
- Pickup prefabs array

---

## 5️⃣ Prefab Creation

### Player Prefab

Create in `Assets/VikingSiegeBreaker/Prefabs/Player/`:

1. **Create Viking_Era1.prefab**:
   ```
   Viking_Era1
   ├── Sprite (SpriteRenderer)
   ├── Collider (CapsuleCollider2D)
   │   └── Is Trigger: ❌
   ├── Rigidbody2D
   │   ├── Body Type: Dynamic
   │   ├── Mass: 1
   │   ├── Linear Drag: 0.5
   │   ├── Angular Drag: 0.5
   │   └── Gravity Scale: 1
   └── VFX
       ├── TrailEffect (ParticleSystem)
       └── DashEffect (ParticleSystem)
   ```

2. **Attach Scripts**:
   - PlayerController.cs
   - MomentumSystem.cs
   - Animator

3. **Configure**:
   - Layer: `Player`
   - Tag: `Player`

4. **Repeat for other eras**: Viking_Era2, Viking_Era3, etc.

### Enemy Prefab

Create in `Assets/VikingSiegeBreaker/Prefabs/Enemies/`:

```
Soldier_Tier1
├── Sprite (SpriteRenderer)
├── Collider (CapsuleCollider2D)
├── Rigidbody2D (Dynamic)
├── Animator
└── Scripts
    └── Enemy.cs
```

Configure:
- Layer: `Enemy`
- Tag: `Enemy`
- Assign EnemyData ScriptableObject

### Pickup Prefab

Create in `Assets/VikingSiegeBreaker/Prefabs/Pickups/`:

```
Meat
├── Sprite (SpriteRenderer)
├── Collider (CircleCollider2D)
│   └── Is Trigger: ✅
└── Scripts
    └── Pickup.cs
```

Configure:
- Layer: `Pickup`
- Tag: `Pickup`
- Assign PickupData ScriptableObject

---

## 6️⃣ ScriptableObject Creation

### Create ScriptableObject Assets

#### Game Settings

1. Create folder: `Assets/VikingSiegeBreaker/ScriptableObjects/Settings/`
2. Right-click > Create > VikingSiegeBreaker > Data > GameSettings
3. Name: `GameSettings`
4. Place in `Assets/VikingSiegeBreaker/Resources/Settings/GameSettings.asset`

#### Upgrades

Create in `Assets/VikingSiegeBreaker/ScriptableObjects/Upgrades/`:

1. Right-click > Create > VikingSiegeBreaker > Data > Upgrade
2. Create the following upgrades:
   - **LaunchPowerUpgrade**
     - Name: "LaunchPower"
     - Display Name: "Launch Power"
     - Base Cost: 100
     - Cost Multiplier: 1.15
     - Base Effect: 10
     - Effect Per Level: 5

   - **MaxHealthUpgrade**
     - Name: "MaxHealth"
     - Base Cost: 150
     - Base Effect: 20
     - Effect Per Level: 10

   - **MomentumDecayUpgrade**
     - Name: "MomentumDecay"
     - Base Cost: 200
     - Base Effect: 0.5
     - Effect Per Level: 0.2

   - **CritChanceUpgrade**
     - Name: "CritChance"
     - Base Cost: 300
     - Base Effect: 1
     - Effect Per Level: 0.5
     - Display As Percentage: ✅

   - **CoinMultiplierUpgrade**
     - Name: "CoinMultiplier"
     - Base Cost: 500
     - Base Effect: 5
     - Effect Per Level: 2
     - Display As Percentage: ✅

#### Enemies

Create in `Assets/VikingSiegeBreaker/ScriptableObjects/Enemies/`:

1. **BasicSoldier**
   - Name: "Soldier"
   - Tier: 1
   - Base Health: 50
   - Contact Damage: 10
   - Coin Reward: 5

2. **HeavyKnight**
   - Tier: 2
   - Base Health: 100
   - Contact Damage: 15
   - Coin Reward: 10

#### Pickups

Create in `Assets/VikingSiegeBreaker/ScriptableObjects/Pickups/`:

1. **MeatPickup**
   - Type: Meat
   - Value: 20 (momentum restore)

2. **ShieldPickup**
   - Type: Shield
   - Value: 5 (duration in seconds)

3. **CoinPickup**
   - Type: Coin
   - Value: 1

#### Evolutions

Create in `Assets/VikingSiegeBreaker/ScriptableObjects/Evolutions/`:

1. **VikingWarrior** (Era 0)
   - Display Name: "Viking Warrior"
   - XP Required: 0
   - Player Prefab: Viking_Era1

2. **BerserkerChief** (Era 1)
   - Display Name: "Berserker Chief"
   - XP Required: 1000
   - Health Bonus: 50
   - Damage Bonus: 10%
   - Player Prefab: Viking_Era2

---

## 7️⃣ Monetization Setup

### LevelPlay (ironSource) Integration

1. **Download SDK**:
   - Visit [ironSource Unity Plugin](https://developers.is.com/ironsource-mobile/unity/unity-plugin/)
   - Download latest SDK
   - Import into Unity

2. **Configure App Keys**:
   - Create app in [ironSource Dashboard](https://platform.ironsrc.com/)
   - Get Android App Key
   - Open `AdsManager.cs`
   - Uncomment LevelPlay code
   - Replace `YOUR_APP_KEY` with your key

3. **Test Ads**:
   - Use Test Suite: **ironSource > Integration Helper**
   - Enable test mode in AdsManager inspector

### Unity IAP Setup

1. **Enable Services**:
   - Window > Services
   - Select project
   - Enable "In-App Purchasing"

2. **Configure Products**:
   - Open `IAPManager.cs`
   - Uncomment Unity IAP code
   - Update product IDs

3. **Platform Setup**:
   - **Google Play**: Create products in Play Console
   - **App Store**: Create products in App Store Connect

4. **Test IAP**:
   - Use sandbox accounts
   - Test purchase flow
   - Verify offline-lock logic

---

## 8️⃣ Build Configuration

### Android Build Settings

1. **Switch Platform**:
   - File > Build Settings
   - Select Android
   - Click "Switch Platform"

2. **Build Settings**:
   - Texture Compression: ASTC
   - Export Project: ❌ (unless using Android Studio)

3. **Keystore**:
   - Create keystore: Edit > Project Settings > Player > Publishing Settings
   - Use for release builds

### Build & Run

1. **Development Build**:
   - ✅ Development Build
   - ✅ Script Debugging (for testing)
   - Build

2. **Release Build**:
   - ❌ Development Build
   - ✅ Split Application Binary (for large games)
   - Build

---

## 9️⃣ Testing Checklist

### Gameplay Tests

- [ ] Launch mechanic works (aim, charge, release)
- [ ] Player collides with enemies correctly
- [ ] Momentum depletes over time
- [ ] Game Over triggers when momentum = 0
- [ ] Pickups collect properly
- [ ] Damage numbers appear
- [ ] Distance tracking works

### UI Tests

- [ ] HUD displays correctly (momentum bar, speed, coins, etc.)
- [ ] Menu buttons work
- [ ] Upgrade panel shows upgrades correctly
- [ ] Purchase upgrades with coins
- [ ] Game Over screen shows stats

### Progression Tests

- [ ] Coins persist between runs
- [ ] Upgrades save/load properly
- [ ] XP accumulates
- [ ] Evolution triggers at correct XP thresholds
- [ ] Best distance is recorded

### Monetization Tests

- [ ] **Offline-Lock Logic**:
  - [ ] With internet: Game plays normally
  - [ ] Without internet + No IAP: Blocked, shows purchase popup
  - [ ] Without internet + IAP purchased: Game plays offline
- [ ] Rewarded ad shows and rewards player
- [ ] Interstitial shows after game over (every 3rd time)
- [ ] IAP "Remove Ads" disables ads
- [ ] IAP "Remove Ads" enables offline play

### Save System Tests

- [ ] Save game on app pause
- [ ] Load game on startup
- [ ] Export/import save data
- [ ] Delete save data works

### Performance Tests

- [ ] 60 FPS on mid-range device
- [ ] No memory leaks
- [ ] Object pooling works (enemies, pickups)
- [ ] Audio doesn't lag

---

## 🔟 Common Issues & Solutions

### Issue: "Input System not found"
**Solution**: Install Input System package, restart Unity

### Issue: "TMPro namespace not found"
**Solution**: Import TextMeshPro via Package Manager, restart IDE

### Issue: "IAP not working in Editor"
**Solution**: IAP only works on device/emulator, use test mode

### Issue: "Ads not showing"
**Solution**: Check AdsManager initialization, verify app key, use test mode

### Issue: "Player falls through ground"
**Solution**: Check Physics2D layer collision matrix, ensure Ground layer collides with Player

### Issue: "Catapult launch not working"
**Solution**: Verify Input Actions are generated, check LaunchPoint is assigned

### Issue: "Save not persisting"
**Solution**: Check PlayerPrefs location, ensure SaveGame is called, verify JSON serialization

---

## 📚 Next Steps

1. **Replace placeholder art** with final sprites/animations
2. **Add audio files** to Resources/Audio/
3. **Create particle effects** for hits, explosions, etc.
4. **Polish animations** (player, enemies, UI)
5. **Balance game economy** (upgrade costs, coin rewards)
6. **Test on multiple devices** (low-end to high-end)
7. **Implement analytics** (Unity Analytics or Firebase)
8. **Add leaderboards** (Google Play Games)
9. **Create promotional materials** (screenshots, video)
10. **Submit to Google Play** / App Store

---

## 🆘 Support & Resources

- **Unity Documentation**: https://docs.unity3d.com/
- **Input System Manual**: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- **ironSource Docs**: https://developers.is.com/ironsource-mobile/unity/
- **Unity IAP Docs**: https://docs.unity.com/ugs/en-us/manual/iap/manual/Overview

---

**Good luck with your Viking Siege Breaker development! 🎮⚔️**
