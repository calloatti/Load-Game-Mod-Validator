using Bindito.Core;
using Timberborn.ModManagerScene;

namespace Calloatti.LoadGameModValidator
{
  [Context("MainMenu")]
  [Context("Game")]
  public class ModConfigurator : Configurator
  {
    protected override void Configure()
    {
      Bind<UnifiedModListDialog>().AsSingleton();
    }
  }
}