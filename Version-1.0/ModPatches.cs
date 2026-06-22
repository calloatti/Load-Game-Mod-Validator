using HarmonyLib;
using System;
using System.Collections.Generic;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.Modding;
using Timberborn.SaveMetadataSystem;

namespace Calloatti.LoadGameModValidator
{
  [HarmonyPatch]
  public static class ModPatches
  {
    [HarmonyPatch(typeof(SaveModsValidator), "ModsAreCompatible")]
    [HarmonyPostfix]
    public static void ModsAreCompatible_Postfix(SaveModsValidator __instance, SaveMetadata metadata, ref bool __result)
    {
      // If vanilla already decided they aren't compatible, or there is no metadata, do nothing.
      if (!__result || metadata == null) return;

      // Extract all the Mod IDs that are present in the save file
      HashSet<string> savedModIds = new HashSet<string>();
      foreach (var savedMod in metadata.Mods)
      {
        savedModIds.Add(savedMod.Id);
      }

      // Because the DLL is publicized, we can access _modRepository directly from __instance
      foreach (var enabledMod in __instance._modRepository.EnabledMods)
      {
        if (!savedModIds.Contains(enabledMod.Manifest.Id))
        {
          // We found a newly enabled mod that wasn't in the save file. Force the dialog.
          __result = false;
          break;
        }
      }
    }

    [HarmonyPatch(typeof(SaveModsValidator), "ShowModsIncompatibilityDialog")]
    [HarmonyPrefix]
    public static bool ShowModsIncompatibilityDialog_Prefix(SaveMetadata metadata, Action continueCallback)
    {
      // Route to our custom UI instead
      if (UnifiedModListDialog.Instance != null)
      {
        UnifiedModListDialog.Instance.ShowDialog(metadata, continueCallback);
        return false; // Skip the vanilla UI entirely
      }

      // Fallback: If our UI failed to load, return true to let the vanilla warning dialog show up!
      return true;
    }
  }
}