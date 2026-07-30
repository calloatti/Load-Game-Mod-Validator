Include ..\AGENTS.md

# Load Game Mod Validator — Mod-Specific Agent Instructions

## Identity
- **Assembly:** `loadgamemodvalidator`
- **Namespace:** `Calloatti.LoadGameModValidator`
- **Framework:** Harmony, Bindito DI
- **ModId:** `Calloatti.LoadGameModValidator`
- **Min Game Version:** 1.0.12.9 — uses `timberborn-decompiled-1.0.*`

## What This Mod Does
Validates mod configurations when loading saved games. Shows a unified mod list dialog warning about missing or mismatched mods, preventing load issues.

## Source Architecture (`Version-1.0/Source/`)

| File | Role |
|---|---|
| `ModStarter.cs` | Entry point — `IModStarter` |
| `ModConfigurator.cs` | DI configurator |
| `ModPatches.cs` | Harmony patches for load validation |
| `UnifiedModListDialog.cs` | Custom dialog UI for mod list display |
| `Calloatti.Util.cs` | Shared utility helpers |
