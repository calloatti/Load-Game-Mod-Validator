using Bindito.Core;

namespace Calloatti.LoadGameModValidator
{
  [Context("MainMenu")]
  [Context("Game")]
  public class ModConfigurator : Configurator
  {
    protected override void Configure()
    {
      // Play dead if Sync Mods Pro is running
      if (!ModStarter.ShouldRun) return;

      Bind<UnifiedModListDialog>().AsSingleton();
    }
  }
}