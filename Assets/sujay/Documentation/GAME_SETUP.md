# Game Setup Documentation - All Scripts

## Table of Contents
1. [Player Setup](#player-setup)
2. [Enemy Setup](#enemy-setup)
3. [Global/Manager Scripts](#globalmanager-scripts)
4. [Other Scripts](#other-scripts)
5. [Portal Script](#portal-script)
6. [Shield Script](#shield-script)
7. [Required Tags](#required-tags)
8. [Input Keys](#input-keys)
9. [Quick Setup Checklist](#quick-setup-checklist)

---

## Player Setup
**GameObject:** Player (or whatever your player is named)

**Required Scripts to Attach:**
1. `Player.cs` - Main player controller
   - Requires: `CharacterController` component
   - Requires: `controller` variable assigned (drag CharacterController to field)
   - Handles: Movement, sprint, death, sounds

2. `SlowMotion.cs` - Slow motion effect
   - Press **F** to toggle slow motion
   - Adjust `slowFactor` (default 0.3) and `smoothTransition` in Inspector

3. `TimeRewind.cs` - Global time rewind system
   - Attach to: GameManager or any manager object (NOT Player)
   - Press **R** to start/stop rewinding ALL "Rewindable" tagged objects
   - `maxRewindTime` = 5 seconds (how far back to record)
   - `rewindableTag` = "Rewindable" (tag objects to rewind)

4. `Shield.cs` - Player shield (attached to Shield child object)
   - Attach to: Shield GameObject (child of Player)
   - Press **Q** to toggle shield
   - Requires: Collider (Is Trigger = true), Renderer

**Player Hierarchy:**
```
Player
├── Camera
├── Gun (if applicable)
└── Shield (child - for shield visual)
```

---

## Enemy Setup

### Flying Enemy
**GameObject:** Any flying enemy

**Required Scripts:**
1. `FlyingEnemy.cs`
   - Stats: health, speed, detection range, score value
   - Flying: hover amplitude, hover speed
   - Combat: attack range, cooldown, projectile prefab, fire point

**Required Components:**
- `CharacterController` or `Rigidbody`
- Tag: `Enemy`

**Setup:**
1. Create enemy GameObject (or use existing enemy)
2. Add `FlyingEnemy.cs` script
3. Assign `projectilePrefab` (bullet prefab from Assets/SCRIPTS/prefabs/)
4. Create empty GameObject as child → name "FirePoint" → position where projectiles spawn
5. Drag FirePoint to `firePoint` field in FlyingEnemy script
6. Tag enemy GameObject as `Enemy` (REQUIRED for shield blocking)
7. If using Rigidbody: set `Is Kinematic = false`, `Use Gravity = true`
8. If using CharacterController: adjust `Center` and `Radius` to fit enemy model

---

## Global/Manager Scripts

### GameManager
**GameObject:** GameManager (persistent)

**Script:** `GameManager.cs`
- Manages score, game state
- `scoreText` - UI Text component to display score
- Uses `Time.timeScale` for pause (0 = paused, 1 = normal)

### TimeRewind Manager
**GameObject:** GameManager or dedicated manager

**Script:** `TimeRewind.cs`
- Finds all objects with tag "Rewindable"
- Automatically adds `Rewindable.cs` component if missing
- Press **R** to toggle rewind

**Objects to Tag as "Rewindable":**
- Player
- All Enemies
- Projectiles (if you want them to rewind)
- Any moving object

### Rewindable Component
**Script:** `Rewindable.cs`
- Automatically added by TimeRewind to "Rewindable" tagged objects
- Records position, rotation, velocity
- Disables other scripts during rewind

---

## Other Scripts

### Projectile
**Script:** `Projectile.cs`
- Damages player on hit
- Requires: Collider (Is Trigger)

### EnemyBullet
**Script:** `EnemyBullet.cs`
- Damages player on hit
- Tag: `EnemyProjectile`

### FrozenNPC
**Script:** `FrozenNPC.cs`
- NPCs that can be rescued
- Requires: Animation component with "talking(1)" animation in Resources

### MeltingIce
**Script:** `MeltingIce.cs`
- Ice blocks that melt over time
- Adjust `meltDuration` in Inspector

### Target
**Script:** `Target.cs`
- Destructible targets
- Has health, adds score on death

### MenuManager
**Script:** `MenuManager.cs`
- Main menu functionality
- `bgMusic` - Audio clip for background music

### Gun
**Script:** `Gun.cs`
- Player's weapon
- Handles shooting, reloading

### ElaraFollow
**Script:** `ElaraFollow.cs`
- Follower NPC (if applicable)

### Splitting Enemy
**Script:** `SplittingEnemy.cs`
- Enemy that splits into 2 smaller copies when killed
- No prefab needed - spawns copies of itself

**Size Progression:**
1. Size 1 (Large, 100 HP) → Dies → Spawns 2x Size 2 (Medium, 50 HP)
2. Size 2 (Medium, 50 HP) → Dies → Spawns 2x Size 4 (Small, 25 HP)
3. Size 4 (Small, 25 HP) → Dies → No more splitting

**Inspector Settings:**
| Setting | Default | Description |
|---------|---------|-------------|
| health | 100 | Enemy health (auto-adjusts by size) |
| moveSpeed | 3 | Movement speed |
| detectionRange | 20 | Detection range for player |
| scoreValue | 200 | Score when killed |
| currentSize | 1 | Size (1=large, 2=medium, 4=small) |
| splitCount | 2 | How many smaller enemies spawn |
| touchDamage | 15 | Damage on player touch |
| wanderRadius | 10 | How far it wanders |
| wanderTimer | 3 | Time between wander target changes |

**How It Works:**
1. Wanders randomly when player is out of range
2. Chases player when within detection range
3. When health reaches 0:
   - Adds score
   - Spawns 2 copies with half scale, half health, 1.2x speed
   - Each copy has `isDead = false` so they can be killed too
4. Destroys itself after spawning copies (or immediately if size 4)

**Setup:**
1. Create enemy GameObject
2. Add `Rigidbody` (Mass = 1000, Use Gravity = true, Freeze Rotation = true)
3. Add `Collider`
4. Add `SplittingEnemy.cs` script
5. Tag as `Enemy`
6. Health auto-adjusts based on `currentSize`:
   - Size 1: 100 HP
   - Size 2: 50 HP
   - Size 4: 25 HP

**Required Tags:**
- Enemy GameObject → Tag as `Enemy`

---

## Portal Script

### Script Name
`Portal.cs`

### Description
A portal that teleports the player to a target location after standing in it for 3 seconds (or by pressing E for manual activation).

### What to Attach To
- Attach to a **Portal GameObject** (Sphere, Cube, or prefab)
- The Portal should have a **Collider** (set as Trigger)

### Required Components on Portal GameObject
1. **Collider** (Box Collider, Sphere Collider, etc.)
   - `Is Trigger = true` (script auto-sets this if not set)

2. **Optional: Renderer** (Mesh Renderer)
   - For visual appearance (glowing portal effect)
   - Apply a transparent/distortive material

### Inspector Settings
| Setting | Default | Description |
|---------|---------|-------------|
| teleportTarget | None | Transform where player will be teleported (REQUIRED) |
| requiredTime | 3 seconds | Time player must stand in portal before teleport |
| manualActivateKey | KeyCode.E | Key to press for manual activation |
| useManualActivation | false | If true, player must press E instead of auto-teleport |
| portalEffect | None | Visual effect prefab to spawn at destination |

### How It Works
**Auto Mode (default):**
1. Player enters portal trigger zone
2. Timer starts counting up to 3 seconds
3. Player must stay in portal for full 3 seconds
4. Player is teleported to `teleportTarget` position/rotation
5. If player leaves early, timer resets

**Manual Mode (`useManualActivation = true`):**
1. Player enters portal trigger zone
2. Player presses **E** to teleport immediately
3. No waiting required

### Setup Steps
1. Create Portal GameObject (Sphere/Cube) in scene
2. Add `Portal.cs` script to Portal
3. Create an empty GameObject as teleport destination → name "PortalTarget"
4. Position "PortalTarget" where you want player to appear
5. Select Portal → drag "PortalTarget" to `teleportTarget` field
6. Add Collider to Portal → check `Is Trigger`
7. (Optional) Assign `portalEffect` prefab for visual feedback
8. (Optional) Set `useManualActivation = true` if you want E key activation

### Player Requirements
- Player GameObject must be tagged as `Player`
- Player should have `CharacterController` component (script handles enabling/disabling)

### Teleport Process
1. Disables player's CharacterController (to avoid collision issues)
2. Sets player position to `teleportTarget.position`
3. Sets player rotation to `teleportTarget.rotation`
4. Re-enables CharacterController
5. Spawns `portalEffect` at destination (if assigned)

### Example Usage (Two-Way Portal):
1. Create "PortalA" with `Portal.cs`
2. Create "PortalB" with `Portal.cs`
3. Set PortalA's `teleportTarget` to PortalB
4. Set PortalB's `teleportTarget` to PortalA
5. Now players can travel back and forth!

---

## Shield Script

### Script Name
`Shield.cs`

### Description
A toggleable shield that protects the player from enemy projectiles and enemies. Activated by pressing Q key.

### What to Attach To
- Attach to a **Shield GameObject** (Sphere or prefab)
- The Shield GameObject should be a **child of the Player** in Hierarchy
- Position: `(0, 0, 0)` relative to Player (centered)
- Scale: `(2, 2, 2)` or adjust to surround player

### Required Components on Shield GameObject
1. **Collider** (Sphere Collider recommended)
   - Used to block projectiles
   - `Is Trigger = true` (to detect hits without physics push)

2. **Renderer** (Mesh Renderer)
   - For visual appearance
   - Apply a transparent material (Rendering Mode: Transparent, Alpha ~30-40%)

3. **Optional: Double-Sided Shader or Duplicate**
   - To see shield from inside, either:
     - Use a double-sided shader
     - OR duplicate shield, set scale to negative values `(-2, -2, -2)`

### Inspector Settings
| Setting | Default | Description |
|---------|---------|-------------|
| activateKey | KeyCode.Q | Key to toggle shield on/off |
| cooldown | 5 seconds | Time before shield can be used again |

### How It Works
1. Press **Q** to activate shield
2. Shield becomes visible and collider activates
3. Blocks objects with tags: `EnemyProjectile`, `Bullet`, `Enemy`
4. Press **Q** again to deactivate
5. Enters cooldown period (5 seconds) before can be used again

### Setup Steps
1. Create Sphere in Hierarchy → rename to "Shield"
2. Make it child of Player GameObject
3. Set Position to `(0, 0, 0)`, Scale to `(2, 2, 2)`
4. Add `Shield.cs` script to Shield
5. Add `Sphere Collider` → check `Is Trigger`
6. Create transparent material → apply to Shield
7. (Optional) Duplicate shield → set scale to `(-2, -2, -2)` for inside visibility

### Debug Output
- `[Shield] Initialized` - When script starts
- `[Shield] Activated!` - When shield turns on
- `[Shield] Deactivated! Cooldown: Xs` - When shield turns off
- `[Shield] Blocked: [object name]` - When something is blocked
- Warnings if Collider or Renderer missing

---

## Required Tags in Project
**MUST BE ADDED IN UNITY INSPECTOR → TAG DROPDOWN**

| Tag | Used By | Required For |
|-----|---------|--------------|
| `Player` | Player GameObject | Player.cs, SlowMotion.cs, TimeRewind.cs |
| `Enemy` | All enemy GameObjects | Shield.cs (blocking), Target.cs (score) |
| `EnemyProjectile` | Enemy bullet prefabs | Shield.cs (blocking) |
| `Bullet` | Player bullet prefabs | Shield.cs (blocking) |
| `Rewindable` | Player, Enemies, Moving Objects | TimeRewind.cs (rewind system) |

### How to Add Tags:
1. Select GameObject in Hierarchy
2. Click "Tag" dropdown at top of Inspector
3. Click "Add Tag..." if tag doesn't exist
4. Create tags: `Player`, `Enemy`, `EnemyProjectile`, `Bullet`, `Rewindable`
5. Select GameObject again → apply the appropriate tag

### Critical Tag Setup:
- **Player GameObject** → Tag as `Player`
- **All Enemy GameObjects** → Tag as `Enemy`
- **Enemy Projectile Prefabs** → Tag as `EnemyProjectile` (if using shield blocking)
- **Player Bullet Prefabs** → Tag as `Bullet`
- **Objects to Rewind** (Player, enemies) → Tag as `Rewindable`

---

## Input Keys
| Key | Action | Script |
|-----|--------|--------|
| WASD | Move | Player.cs |
| Mouse | Look | Player.cs |
| Left Click | Shoot | Gun.cs |
| Left Shift | Sprint | Player.cs |
| Q | Toggle Shield | Shield.cs |
| E | Activate Portal | Portal.cs (manual mode) |
| F | Slow Motion | SlowMotion.cs |
| R | Time Rewind | TimeRewind.cs |

---

## Prefabs Needed
- **Player prefab** (with CharacterController, Player.cs, SlowMotion.cs)
- **Shield prefab** (with Shield.cs, Collider, transparent material)
- **Enemy prefab** (with FlyingEnemy.cs, Enemy tag)
- **Projectile prefab** (with Projectile.cs or EnemyBullet.cs)
- **FirePoint** (empty GameObject as child of enemy)
- **Portal prefab** (with Portal.cs, Collider)

---

## Quick Setup Checklist

### Player Setup:
- [ ] Player GameObject tagged as `Player`
- [ ] Player has `CharacterController` component
- [ ] `controller` variable assigned in Player.cs (drag CharacterController to field)
- [ ] Player has `Player.cs`, `SlowMotion.cs` scripts
- [ ] Shield is child of Player with `Shield.cs`
- [ ] Shield has `Collider` (Is Trigger = true) + `Renderer`
- [ ] Shield GameObject tagged as `Rewindable` (for time rewind)

### Enemy Setup:
- [ ] Enemy GameObject tagged as `Enemy` (CRITICAL for shield)
- [ ] Enemy has `FlyingEnemy.cs` script
- [ ] `projectilePrefab` assigned in FlyingEnemy.cs
- [ ] `firePoint` (empty child object) assigned in FlyingEnemy.cs
- [ ] Enemy tagged as `Rewindable` (for time rewind)

### Manager Setup:
- [ ] GameManager has `GameManager.cs`, `TimeRewind.cs` scripts
- [ ] `scoreText` assigned in GameManager.cs (if using UI)

### Portal Setup:
- [ ] Portal has `Portal.cs` script
- [ ] `teleportTarget` assigned (destination object)
- [ ] Portal has Collider with `Is Trigger = true`
- [ ] Test: Stand in portal for 3 seconds → teleports to target

### Shield Testing:
- [ ] Press **Q** to activate shield
- [ ] Shield appears (renderer enabled)
- [ ] Shield blocks enemies/projectiles
- [ ] Press **Q** again to deactivate
- [ ] 5 second cooldown before can use again
