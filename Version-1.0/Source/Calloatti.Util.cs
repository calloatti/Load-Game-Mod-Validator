using System;
using System.Linq;

namespace Calloatti.Util
{
  public static class ModCheck
  {
    public static bool IsModEnabled(string assemblyName)
    {
      if (string.IsNullOrEmpty(assemblyName)) return false;

      return AppDomain.CurrentDomain.GetAssemblies()
        .Any(a => a.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
    }
  }
}