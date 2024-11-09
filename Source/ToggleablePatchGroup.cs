// Decompiled with JetBrains decompiler
// Type: WF.ToggleablePatchGroup
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using System.Collections.Generic;

namespace WF
{
  public class ToggleablePatchGroup : IToggleablePatch
  {
    public List<IToggleablePatch> Patches;

    public string Name { get; set; }

    public bool Enabled { get; set; }

    public bool Applied { get; protected set; }

    public void Apply()
    {
      if (!this.Applied)
      {
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Applying patches in patch group \"" + this.Name + "\"..");
        foreach (IToggleablePatch patch in this.Patches)
          patch.Apply();
        this.Applied = true;
      }
      else
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping application of patch group \"" + this.Name + "\" because it is already applied.");
    }

    public void Remove()
    {
      if (this.Applied)
      {
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Removing patches in patch group \"" + this.Name + "\"..");
        foreach (IToggleablePatch patch in this.Patches)
          patch.Remove();
        this.Applied = false;
      }
      else
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping removal of patch group \"" + this.Name + "\" because it is not applied.");
    }
  }
}
