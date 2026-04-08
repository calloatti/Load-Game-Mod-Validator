using HarmonyLib;
using Timberborn.ModManagerScene;
using Calloatti.Config;

namespace Calloatti.LoadGameModValidator
{
  public class ModStarter : IModStarter
  {
    public static SimpleConfig Config { get; private set; }

    public void StartMod(IModEnvironment modEnvironment)
    {
      Config = new SimpleConfig(modEnvironment.ModPath);
      
      // This tells Harmony to scan our mod's assembly and apply all [HarmonyPatch] attributes
      new Harmony("calloatti.loadgamemodvalidator").PatchAll();
    }
  }
}