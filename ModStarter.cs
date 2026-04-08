using HarmonyLib;
using Timberborn.ModManagerScene;

namespace Calloatti.LoadGameModValidator
{
  public class ModStarter : IModStarter
  {
    public void StartMod(IModEnvironment modEnvironment)
    {
      // This tells Harmony to scan our mod's assembly and apply all [HarmonyPatch] attributes
      new Harmony("calloatti.loadgamemodvalidator").PatchAll();
    }
  }
}