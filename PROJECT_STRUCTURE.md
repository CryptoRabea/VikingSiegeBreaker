# Viking Siege Breaker - Project Structure

## 📁 Folder Organization

```
Assets/
├── VikingSiegeBreaker/
│   ├── Scripts/
│   │   ├── Core/                      # Core game systems
│   │   │   ├── GameManager.cs
│   │   │   ├── SaveSystem.cs
│   │   │   └── NetworkCheck.cs
│   │   ├── Player/                    # Player-related scripts
│   │   │   ├── PlayerController.cs
│   │   │   ├── MomentumSystem.cs
│   │   │   └── CatapultController.cs
│   │   ├── Entities/                  # Game entities
│   │   │   ├── Enemy.cs
│   │   │   ├── Pickup.cs
│   │   │   └── Destructible.cs
│   │   ├── Systems/                   # Game systems
│   │   │   ├── SpawnManager.cs
│   │   │   ├── UpgradeManager.cs
│   │   │   ├── EvolutionManager.cs
│   │   │   └── CurrencyManager.cs
│   │   ├── Managers/                  # Service managers
│   │   │   ├── UIManager.cs
│   │   │   ├── AdsManager.cs
│   │   │   ├── IAPManager.cs
│   │   │   └── AudioManager.cs
│   │   ├── Data/                      # ScriptableObject definitions
│   │   │   ├── UpgradeData.cs
│   │   │   ├── EnemyData.cs
│   │   │   ├── PickupData.cs
│   │   │   ├── EvolutionData.cs
│   │   │   └── GameSettings.cs
│   │   ├── UI/                        # UI-specific scripts
│   │   │   ├── HUDController.cs
│   │   │   ├── MenuController.cs
│   │   │   ├── ShopPanel.cs
│   │   │   ├── GameOverPanel.cs
│   │   │   └── UpgradePanel.cs
│   │   └── Utilities/                 # Helper scripts
│   │       ├── DebugTools.cs
│   │       ├── ObjectPooler.cs
│   │       └── Extensions.cs
│   ├── ScriptableObjects/             # SO asset files
│   │   ├── Upgrades/
│   │   │   ├── LaunchPowerUpgrade.asset
│   │   │   ├── MaxHealthUpgrade.asset
│   │   │   └── ...
│   │   ├── Enemies/
│   │   │   ├── BasicSoldier.asset
│   │   │   ├── HeavyKnight.asset
│   │   │   └── ...
│   │   ├── Pickups/
│   │   │   ├── MeatPickup.asset
│   │   │   ├── ShieldPickup.asset
│   │   │   └── ...
│   │   ├── Evolutions/
│   │   │   ├── VikingWarrior.asset
│   │   │   ├── BerserkerChief.asset
│   │   │   └── ...
│   │   └── Settings/
│   │       └── GameSettings.asset
│   ├── Prefabs/
│   │   ├── Player/
│   │   │   ├── Viking_Era1.prefab
│   │   │   ├── Viking_Era2.prefab
│   │   │   └── Viking_Era3.prefab
│   │   ├── Enemies/
│   │   │   ├── Soldier_Tier1.prefab
│   │   │   ├── Knight_Tier2.prefab
│   │   │   └── ...
│   │   ├── Pickups/
│   │   │   ├── Meat.prefab
│   │   │   ├── Shield.prefab
│   │   │   └── Coin.prefab
│   │   ├── Environment/
│   │   │   ├── Castle_Wall.prefab
│   │   │   ├── Tower.prefab
│   │   │   └── Obstacle.prefab
│   │   ├── VFX/
│   │   │   ├── HitEffect.prefab
│   │   │   ├── DeathExplosion.prefab
│   │   │   └── LaunchTrail.prefab
│   │   └── UI/
│   │       ├── DamageNumber.prefab
│   │       └── CoinPopup.prefab
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── Gameplay.unity
│   │   └── GameOver.unity
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Animations/
│   │   └── UI/
│   ├── Audio/
│   │   ├── SFX/
│   │   └── Music/
│   └── Resources/
│       └── Data/
└── Plugins/                           # Third-party plugins
    ├── Unity.Services.Core/
    ├── UnityPurchasing/
    └── IronSource/                    # LevelPlay SDK

```

## 🎯 Quick Navigation

### Core Gameplay Loop
1. **MainMenu** → Player launches → **Gameplay** → Momentum depletes → **GameOver** → Loop

### Key Dependencies
- Unity 6.2+
- Input System (com.unity.inputsystem)
- TextMeshPro (com.unity.textmeshpro)
- Unity IAP (com.unity.purchasing)
- LevelPlay SDK (ironSource)

### PlayerPrefs Keys Used
See `SaveSystem.cs` for complete list. Main keys:
- `VSB_SaveData` - JSON save data
- `NoAdsPurchased` - IAP flag (0/1)
- `FirstLaunch` - Tutorial flag
- `MusicVolume`, `SFXVolume` - Audio settings

## 📋 Setup Checklist

- [ ] Import Input System package
- [ ] Import TextMeshPro
- [ ] Import Unity IAP
- [ ] Import LevelPlay SDK
- [ ] Create all ScriptableObject assets
- [ ] Set up scenes (MainMenu, Gameplay, GameOver)
- [ ] Configure Input Actions
- [ ] Set up Physics2D layers
- [ ] Configure Android build settings
- [ ] Test monetization flows
