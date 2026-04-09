using HarmonyLib;
using System;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.SaveMetadataSystem;

namespace Calloatti.LoadGameModValidator
{
  [HarmonyPatch]
  public static class ModPatches
  {
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