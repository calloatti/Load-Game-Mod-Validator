using HarmonyLib;
using Timberborn.ModManagerScene;
using Calloatti.Config;
using Calloatti.Util; // To access ModCheck
using UnityEngine;

namespace Calloatti.LoadGameModValidator
{
  public class Log
  {
    public static readonly string Prefix = "[LoadGameModValidator]";
    public static void Info(string message) => Debug.Log($"{Prefix} {message}");
  }

  public class ModStarter : IModStarter
  {
    public static SimpleConfig Config { get; private set; }

    // The master flag for the configurator
    public static bool ShouldRun = true;

    public void StartMod(IModEnvironment modEnvironment)
    {
      // --- THE STEALTH CHECK ---
      // Pass the exact .dll assembly name of your new mod
      if (ModCheck.IsModEnabled("SyncModsPro"))
      {
        ShouldRun = false;
        Log.Info("Sync Mods Pro detected. Soft-disabling to prevent conflicts.");
        return; // Play dead! Skip Config load and Harmony patches entirely.
      }

      // --- NORMAL BOOT ---
      Config = new SimpleConfig(modEnvironment.ModPath);

      new Harmony("calloatti.loadgamemodvalidator").PatchAll();
      Log.Info("Harmony patches applied successfully.");
    }
  }
}