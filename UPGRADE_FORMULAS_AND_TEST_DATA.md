# Viking Siege Breaker - Upgrade Formulas & Test Data

Complete upgrade progression formulas with sample data for 100 levels.

---

## 📊 Upgrade Formula System

### Cost Formula (Exponential Growth)

```
Cost(level) = baseCost × (costMultiplier ^ (level - 1))
```

**Example**:
- Base Cost: 100
- Cost Multiplier: 1.15
- Level 1: 100 × (1.15^0) = **100**
- Level 10: 100 × (1.15^9) = **363**
- Level 50: 100 × (1.15^49) = **125,256**
- Level 100: 100 × (1.15^99) = **186,352,370**

### Effect Formula (Two Types)

#### Additive (Linear Growth)
```
Effect(level) = baseEffect + (effectPerLevel × level)
```

**Example** (Launch Power):
- Base Effect: 10
- Effect Per Level: 5
- Level 1: 10 + (5 × 1) = **15**
- Level 50: 10 + (5 × 50) = **260**
- Level 100: 10 + (5 × 100) = **510**

#### Multiplicative (Exponential Growth)
```
Effect(level) = baseEffect × ((1 + effectMultiplier) ^ level)
```

**Example** (Crit Chance):
- Base Effect: 1%
- Effect Multiplier: 0.02 (2% growth per level)
- Level 1: 1 × (1.02^1) = **1.02%**
- Level 50: 1 × (1.02^50) = **2.69%**
- Level 100: 1 × (1.02^100) = **7.24%**

---

## 🎯 Recommended Upgrade Configurations

### 1. Launch Power (Combat - Additive)
**Description**: Increases catapult launch force.

| Parameter | Value |
|-----------|-------|
| Base Cost | 100 |
| Cost Multiplier | 1.15 |
| Base Effect | 10 |
| Effect Per Level | 5 |
| Unit | "power" |

**Balance**: Linear growth keeps launches predictable while scaling steadily.

### 2. Max Health (Combat - Additive)
**Description**: Increases player maximum HP.

| Parameter | Value |
|-----------|-------|
| Base Cost | 150 |
| Cost Multiplier | 1.15 |
| Base Effect | 20 |
| Effect Per Level | 10 |
| Unit | "HP" |

**Balance**: Health grows significantly at higher levels for survivability.

### 3. Momentum Decay (Utility - Additive)
**Description**: Reduces momentum loss rate.

| Parameter | Value |
|-----------|-------|
| Base Cost | 200 |
| Cost Multiplier | 1.18 |
| Base Effect | 0.5 |
| Effect Per Level | 0.2 |
| Unit | "units/sec" |

**Balance**: Critical for long runs; cost increases faster.

### 4. Crit Chance (Combat - Multiplicative)
**Description**: Increases critical hit chance.

| Parameter | Value |
|-----------|-------|
| Base Cost | 300 |
| Cost Multiplier | 1.20 |
| Base Effect | 1 |
| Effect Multiplier | 0.02 |
| Display As Percentage | Yes |

**Balance**: Caps naturally around 20% at level 100 (balanced).

### 5. Crit Damage (Combat - Additive)
**Description**: Increases critical hit damage multiplier.

| Parameter | Value |
|-----------|-------|
| Base Cost | 400 |
| Cost Multiplier | 1.20 |
| Base Effect | 50 |
| Effect Per Level | 5 |
| Unit | "%" |

**Balance**: Linear growth to avoid one-shot mechanics.

### 6. Coin Multiplier (Economy - Additive)
**Description**: Increases coins earned from runs.

| Parameter | Value |
|-----------|-------|
| Base Cost | 500 |
| Cost Multiplier | 1.25 |
| Base Effect | 5 |
| Effect Per Level | 2 |
| Unit | "%" |

**Balance**: Expensive but accelerates progression significantly.

### 7. XP Multiplier (Economy - Additive)
**Description**: Increases XP earned from runs.

| Parameter | Value |
|-----------|-------|
| Base Cost | 600 |
| Cost Multiplier | 1.25 |
| Base Effect | 5 |
| Effect Per Level | 1.5 |
| Unit | "%" |

**Balance**: Slightly weaker than coin multiplier but enables faster evolutions.

---

## 📈 Sample Data: Launch Power (100 Levels)

**Configuration**:
- Base Cost: 100
- Cost Multiplier: 1.15
- Base Effect: 10
- Effect Per Level: 5

| Level | Cost | Cumulative Cost | Effect | Total Power |
|-------|------|-----------------|--------|-------------|
| 1 | 100 | 100 | 15 | 15 |
| 5 | 131 | 572 | 35 | 35 |
| 10 | 363 | 2,030 | 60 | 60 |
| 15 | 814 | 5,474 | 85 | 85 |
| 20 | 1,823 | 14,645 | 110 | 110 |
| 25 | 4,083 | 39,016 | 135 | 135 |
| 30 | 9,147 | 103,608 | 160 | 160 |
| 35 | 20,496 | 274,524 | 185 | 185 |
| 40 | 45,925 | 726,083 | 210 | 210 |
| 45 | 102,899 | 1,918,070 | 235 | 235 |
| 50 | 230,586 | 5,064,709 | 260 | 260 |
| 60 | 1,173,909 | 36,458,799 | 310 | 310 |
| 70 | 5,974,312 | 262,343,423 | 360 | 360 |
| 80 | 30,399,152 | 1,887,466,937 | 410 | 410 |
| 90 | 154,705,527 | 13,578,960,433 | 460 | 460 |
| 100 | 787,024,649 | 97,672,527,146 | 510 | 510 |

**Analysis**:
- Level 50 is achievable for mid-game players (~5M coins total)
- Level 100 is endgame whale territory (~98B coins total)
- Power scales linearly, predictable for balancing

---

## 📊 Comparison Table: All Upgrades at Key Levels

| Upgrade | Lv 1 Cost | Lv 25 Effect | Lv 50 Effect | Lv 100 Effect |
|---------|-----------|--------------|--------------|---------------|
| Launch Power | 100 | 135 power | 260 power | 510 power |
| Max Health | 150 | 270 HP | 520 HP | 1,020 HP |
| Momentum Decay | 200 | 5.5 reduction | 10.5 reduction | 20.5 reduction |
| Crit Chance | 300 | 1.64% | 2.69% | 7.24% |
| Crit Damage | 400 | 175% | 300% | 550% |
| Coin Multiplier | 500 | +55% | +105% | +205% |
| XP Multiplier | 600 | +42.5% | +80% | +155% |

---

## 💰 Total Cost to Max All Upgrades

Assuming **7 upgrades** maxed to level 100:

| Upgrade | Total Cost to Lv 100 |
|---------|----------------------|
| Launch Power | ~98B coins |
| Max Health | ~147B coins |
| Momentum Decay | ~234B coins |
| Crit Chance | ~547B coins |
| Crit Damage | ~912B coins |
| Coin Multiplier | ~2.28T coins |
| XP Multiplier | ~3.42T coins |
| **TOTAL** | **~7.6 Trillion coins** |

**Reality Check**: This is balanced for a long-term idle/progression game. Players will naturally focus on a few upgrades, not max all.

---

## 🎮 Progression Pacing (Estimated)

### Early Game (Levels 1-20)
- **Time**: 1-2 weeks
- **Upgrades**: Focus on Launch Power, Max Health
- **Coins Needed**: ~50K total
- **Playstyle**: Learning mechanics, short runs

### Mid Game (Levels 20-50)
- **Time**: 2-4 weeks
- **Upgrades**: Add Coin Multiplier, Momentum Decay
- **Coins Needed**: ~5M total
- **Playstyle**: Optimizing runs, farming coins

### Late Game (Levels 50-75)
- **Time**: 1-2 months
- **Upgrades**: Balanced across all
- **Coins Needed**: ~500M total
- **Playstyle**: Pushing distance records, evolutions

### Endgame (Levels 75-100)
- **Time**: 3-6+ months (or IAP)
- **Upgrades**: Min-maxing final levels
- **Coins Needed**: Trillions
- **Playstyle**: Completionist, leaderboards

---

## 🔧 Balancing Recommendations

### For Faster Progression
- Lower cost multipliers (1.12-1.13 instead of 1.15)
- Increase base effect values
- Add daily login bonuses
- Increase coin/XP rewards from runs

### For Slower Progression (Retention)
- Increase cost multipliers (1.18-1.20)
- Decrease effect per level
- Introduce soft caps (diminishing returns after Lv 50)
- Add prestige/reset mechanics

### For Monetization
- Offer "Double Coins" IAP (1 week duration)
- Sell "Upgrade Boost" packs (instantly gain 10 levels)
- "Starter Pack" (gems + coin boost + no ads)
- Event-exclusive upgrades

---

## 📉 CSV Export (for spreadsheet import)

```csv
Level,LaunchPower_Cost,LaunchPower_Effect,MaxHealth_Cost,MaxHealth_Effect,CritChance_Cost,CritChance_Effect
1,100,15,150,30,300,1.02
2,115,20,173,40,360,1.04
3,132,25,198,50,432,1.06
4,152,30,228,60,518,1.08
5,175,35,262,70,622,1.10
10,363,60,544,120,1552,1.22
15,814,85,1221,170,4460,1.35
20,1823,110,2735,220,12823,1.49
25,4083,135,6125,270,36866,1.64
30,9147,160,13721,320,106000,1.81
35,20496,185,30739,370,304809,2.00
40,45925,210,68881,420,876382,2.21
45,102899,235,154349,470,2519605,2.44
50,230586,260,345878,520,7240557,2.69
60,1173909,310,1760864,620,41163647,3.28
70,5974312,360,8961468,720,234071901,4.00
80,30399152,410,45598825,820,1331235529,4.88
90,154705527,460,232058109,920,7571711040,5.94
100,787024649,510,1180536174,1020,43070703864,7.24
```

---

## 🧪 Testing Commands (DebugTools)

Use these commands in Unity Console for testing:

```csharp
// Set specific upgrade level
DebugTools.SetCoins(1000000); // Set 1M coins
UpgradeManager.Instance.CheatMaxAllUpgrades(); // Max all upgrades

// Test specific level costs
var upgrade = UpgradeManager.Instance.GetUpgradeData("LaunchPower");
int cost = UpgradeManager.Instance.CalculateCost(upgrade, 50);
Debug.Log($"Level 50 cost: {cost}");

// Test effect calculation
float effect = UpgradeManager.Instance.CalculateEffect(upgrade, 50);
Debug.Log($"Level 50 effect: {effect}");
```

---

## 📊 Analytics Metrics to Track

Monitor these for balancing:

1. **Average Upgrade Level** (per upgrade, per player cohort)
2. **Time to Reach Level 50** (for each upgrade)
3. **Coins Earned Per Run** (average, median, top 10%)
4. **Upgrade Purchase Frequency** (which upgrades are most popular)
5. **Retention by Upgrade Level** (do players with higher upgrades stick around?)
6. **IAP Conversion** (do players buy coin packs? At what upgrade level?)

---

## 🎯 Recommended Tweaks After Launch

Based on analytics:

### If Progression Too Slow
- Reduce cost multipliers by 0.02-0.03
- Add 2x coin events on weekends
- Increase run rewards by 20-30%

### If Progression Too Fast
- Increase cost multipliers by 0.02-0.03
- Add more upgrade tiers (levels 101-150)
- Introduce prestige system

### If IAP Not Converting
- Lower "Remove Ads" price
- Add more valuable gem packs
- Offer limited-time bundles

---

**Use this data as a starting point. Balance is an iterative process!** 🎮⚔️
