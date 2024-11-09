// Decompiled with JetBrains decompiler
// Type: WF.ToggleablePatch`1
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using System;
using System.Collections.Generic;
using Verse;

namespace WF
{
  public class ToggleablePatch<T> : IToggleablePatch where T : Def
  {
    protected T targetDef;
    protected bool? modInstalled;
    public string TargetModID;
    public string TargetDefName;
    public List<string> ConflictingModIDs = new List<string>();
    public Action<ToggleablePatch<T>, T> Patch;
    public Action<ToggleablePatch<T>, T> Unpatch;
    public object State;

    public string Name { get; set; }

    public bool Enabled { get; set; }

    public bool Applied { get; protected set; }

    public string TargetDescriptionString => (this.TargetModID != null ? this.TargetModID + "." : "") + this.TargetDefName + " (" + typeof (T).FullName + ")";

    public void Apply()
    {
      if (this.CanPatch)
      {
        if (!this.Applied)
        {
          ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] " + (this.Name != null ? "Applying patch \"" + this.Name + "\", patching " : "Patching ") + this.TargetDescriptionString + "..");
          if ((object) this.targetDef == null)
            this.targetDef = DefDatabase<T>.GetNamed(this.TargetDefName);
          try
          {
            this.Patch(this, this.targetDef);
          }
          catch (Exception ex)
          {
            ToggleablePatch.ErrorLoggingMethod("[ToggleablePatch] Error " + (this.Name != null ? "applying patch \"" + this.Name + "\"" : "patching ") + ". Most likely you have another mod that already patches " + this.TargetDescriptionString + ". Remove that mod or disable this patch in the mod options.\n\n" + ex.ToString());
          }
          this.Applied = true;
        }
        else
          ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping application of patch \"" + this.Name + "\" because it is already applied.");
      }
      else
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping application of patch \"" + this.Name + "\" because it cannot be applied.");
    }

    public void Remove()
    {
      if (this.Applied)
      {
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] " + (this.Name != null ? "Removing patch \"" + this.Name + "\", unpatching " : "Unpatching ") + this.TargetDescriptionString + "..");
        if ((object) this.targetDef == null)
          this.targetDef = DefDatabase<T>.GetNamed(this.TargetDefName);
        try
        {
          this.Unpatch(this, this.targetDef);
        }
        catch (Exception ex)
        {
          ToggleablePatch.ErrorLoggingMethod("[ToggleablePatch] Error " + (this.Name != null ? "removing patch \"" + this.Name + "\"" : "unpatching ") + ". Most likely you have another mod that already patches " + this.TargetDescriptionString + ", and it failed to patch in the first place. Remove that mod or disable this patch in the mod options.\n\n" + ex.ToString());
        }
        this.Applied = false;
      }
      else
        ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping removal of patch \"" + this.Name + "\" because it is not applied.");
    }

    public bool CanPatch
    {
      get
      {
        foreach (string conflictingModId in this.ConflictingModIDs)
        {
          if (ModLister.GetActiveModWithIdentifier(conflictingModId) != null)
          {
            ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Skipping patch \"" + this.Name + "\" because conflicting mod with ID \"" + conflictingModId + "\" was found.");
            return false;
          }
        }
        if (this.TargetModID == null)
          return true;
        if (!this.modInstalled.HasValue)
          this.modInstalled = new bool?(ModLister.GetActiveModWithIdentifier(this.TargetModID) != null);
        return this.modInstalled.Value;
      }
    }
  }
}
