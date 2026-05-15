# iCE KINGDOM FINAL - Progress Log

## Latest Session: Splitting Enemy Update (May 2026)

### Completed
- [x] Updated `SplittingEnemy.cs` - Added wandering movement when player out of range
- [x] Fixed Rigidbody issues - High mass (1000) prevents bullet push, uses gravity
- [x] Removed smallerEnemyPrefab dependency - Now spawns copies of itself (no prefab needed)
- [x] Fixed spawning bug - Sets `isDead = false` on spawned copies
- [x] Health progression: Size 1 (100 HP) → Size 2 (50 HP) → Size 4 (25 HP)
- [x] Each split: 2 copies, half scale, half health, 1.2x speed, half score
- [x] Removed individual `SPLITTING_ENEMY.md` - Consolidated into `GAME_SETUP.md`
- [x] Updated `progress.txt` and `PROJECT_SUMMARY.txt` with latest changes
- [x] SplittingEnemy documentation added to `GAME_SETUP.md`
- [x] Pushed changes to GitHub (latest commit: 82d7029)

### Previous Session: Portal Script & Documentation Update
- [x] Removed old `Shield.cs` and ShieldMaterial.mat
- [x] Created new `Shield.cs` - Working shield with Q key toggle, 5s cooldown
- [x] Created `TimeRewind.cs` - Global time rewind system (press R)
- [x] Created `Rewindable.cs` - Component for rewindable objects
- [x] Created `Portal.cs` - Teleport script with 3-second timer + manual E key activation
- [x] Combined all script documentation into `GAME_SETUP.md`
- [x] Moved all txt/md files to `Assets/Documentation/` folder

### How to Setup Portal (NOW WORKING)
1. Create Portal GameObject (Sphere/Cube) in scene
2. Add `Portal.cs` script to Portal
3. Add `Collider` → check `Is Trigger` (script auto-sets this)
4. Create empty GameObject as destination → name "PortalTarget"
5. Position "PortalTarget" where you want player to appear
6. Select Portal → drag "PortalTarget" to `teleportTarget` field
7. Tag Player GameObject as `Player` (CRITICAL!)
8. Test: Stand in portal for 3 seconds → player teleports!

**Portal Inspector Options:**
- `requiredTime`: 3 seconds (default)
- `useManualActivation`: true (player must press E instead of auto-teleport)
- `portalEffect`: Assign visual effect prefab (optional)

### Required Tags (MUST BE SET IN UNITY)
| Tag | Assign To | Required For |
|-----|---------|--------------|
| `Player` | Player GameObject | Portal.cs, Shield.cs, TimeRewind.cs |
| `Enemy` | All enemy GameObjects | Shield.cs (blocking) |
| `Rewindable` | Player, Enemies | TimeRewind.cs (rewind system) |

### Input Keys
| Key | Action | Script |
|-----|--------|--------|
| Q | Toggle Shield | Shield.cs |
| E | Activate Portal (manual mode) | Portal.cs |
| F | Slow Motion | SlowMotion.cs |
| R | Time Rewind | TimeRewind.cs |

### Pending / Issues
- [ ] Player script error: `UnassignedReferenceException: The variable controller of Player has not been assigned`
  - Fix: Select Player GameObject → In Inspector, drag CharacterController to `controller` field
- [ ] GitHub warning: `Tex_Particles_01.tga` (64MB) exceeds 50MB limit
  - Consider using Git LFS for large files

### Notes
- Shield: Press Q to toggle, blocks enemies/projectiles, 5s cooldown
- Portal: Auto-teleport after 3s OR press E for manual mode
- Time Rewind: Press R to rewind all "Rewindable" tagged objects
- All documentation in `Assets/Documentation/GAME_SETUP.md`
