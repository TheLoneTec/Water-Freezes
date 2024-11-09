// Decompiled with JetBrains decompiler
// Type: WF.ToggleablePatchExtensions
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

namespace WF
{
  public static class ToggleablePatchExtensions
  {
    public static void Process(this IToggleablePatch patch)
    {
      if (patch.Enabled)
        patch.Apply();
      else if (patch.Applied)
        patch.Remove();
      else
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping patch " + (patch is ToggleablePatchGroup ? "group" : "") + "\"" + patch.Name + "\" because it is disabled.");
    }
  }
}
