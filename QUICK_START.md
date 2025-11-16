# ⚡ Viking Siege Breaker - Quick Start Guide

**Get your project ready for development in 60 seconds!**

---

## 🎯 Three Commands to Success

### 1️⃣ Build All Scenes (5 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → 1. Build All Scenes
```
**Keyboard Shortcut:** `Ctrl+Alt+B` (Windows) / `Cmd+Alt+B` (Mac)

**Creates:**
- ✓ MainMenu.unity
- ✓ Gameplay.unity
- ✓ GameOver.unity

---

### 2️⃣ Create Sample Data (5 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → Create ScriptableObjects
```

**Creates:**
- ✓ Upgrades (LaunchPower, MaxHealth, etc.)
- ✓ Enemies (BasicSoldier, HeavyKnight, etc.)
- ✓ Pickups (Meat, Shield, Coin, Gem)
- ✓ Evolutions (VikingWarrior, etc.)

---

### 3️⃣ Test It! (5 seconds)
```
Unity Menu → Tools → Viking Siege Breaker → Quick Build → 5. Play from MainMenu
```
**Keyboard Shortcut:** `Ctrl+Alt+P` (Windows) / `Cmd+Alt+P` (Mac)

---

## ✨ That's It!

Your project is now fully set up and ready for development!

---

## 🎹 Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+Alt+B` | Build all scenes |
| `Ctrl+Alt+1` | Open MainMenu scene |
| `Ctrl+Alt+2` | Open Gameplay scene |
| `Ctrl+Alt+3` | Open GameOver scene |
| `Ctrl+Alt+P` | Play from MainMenu |

---

## 📚 Next Steps

1. **Customize scenes** - Add UI elements, configure components
2. **Create prefabs** - Player, enemies, pickups
3. **Configure data** - Edit ScriptableObjects with real values
4. **Add art** - Import sprites, animations, VFX
5. **Test gameplay** - Play and iterate
6. **Build** - File → Build Settings → Build

---

## 🔧 Validation

Before building for release, run:
```
Tools → Viking Siege Breaker → Quick Build → Test Build (Validate Setup)
```

This checks:
- ✓ All scenes exist
- ✓ Build settings configured
- ✓ Scripts compile successfully

---

## 🆘 Need Help?

- **Scene Builder Guide:** `SCENE_BUILDER_GUIDE.md` (detailed documentation)
- **Project Structure:** `PROJECT_STRUCTURE.md`
- **Unity Setup:** `UNITY_SETUP_GUIDE.md`
- **Console errors?** Run validation test first

---

## 🎉 Pro Tips

### Rapid Iteration Workflow
```
1. Ctrl+Alt+2  (Open Gameplay scene)
2. Make changes
3. Ctrl+Alt+P  (Play from MainMenu)
4. Test
5. Repeat!
```

### Clean Reset
Need to start fresh?
```
Tools → Quick Build → Clean All (Reset Project)
```
Then rebuild with `Ctrl+Alt+B`

---

**Happy developing! ⚔️🛡️**

---

## 📊 What Gets Created

### Scenes
```
Assets/VikingSiegeBreaker/Scenes/
├── MainMenu.unity    ← Menu, settings, start
├── Gameplay.unity    ← Main game loop
└── GameOver.unity    ← Results, upgrades
```

### Data Assets
```
Assets/VikingSiegeBreaker/ScriptableObjects/
├── Upgrades/         ← 4 sample upgrades
├── Enemies/          ← 3 sample enemies
├── Pickups/          ← 4 sample pickups
├── Evolutions/       ← 3 sample evolutions
└── Settings/         ← Game settings
```

### Build Settings
All scenes automatically added and ready to build!

---

**Total Setup Time: ~15 seconds** ⚡

For detailed documentation, see **SCENE_BUILDER_GUIDE.md**
