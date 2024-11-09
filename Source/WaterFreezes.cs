// Decompiled with JetBrains decompiler
// Type: WF.WaterFreezes
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace WF
{
  [StaticConstructorOnStartup]
  public static class WaterFreezes
  {
    [ToggleablePatch]
    public static ToggleablePatch<ThingDef> WatermillIsFlickablePatch = new ToggleablePatch<ThingDef>()
    {
      Name = "Watermill Is Flickable",
      Enabled = true,
      TargetDefName = "WatermillGenerator",
      Patch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.Add((CompProperties) new CompProperties_Flickable())),
      Unpatch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.RemoveAll((Predicate<CompProperties>) (comp => comp is CompProperties_Flickable)))
    };
    [ToggleablePatch]
    public static ToggleablePatch<ThingDef> VPEAdvancedWatermillIsFlickablePatch = new ToggleablePatch<ThingDef>()
    {
      Name = "VPE Advanced Watermill Is Flickable",
      Enabled = true,
      TargetModID = "VanillaExpanded.VFEPower",
      TargetDefName = "VFE_AdvancedWatermillGenerator",
      Patch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.Add((CompProperties) new CompProperties_Flickable())),
      Unpatch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.RemoveAll((Predicate<CompProperties>) (c => c is CompProperties_Flickable)))
    };
    [ToggleablePatch]
    public static ToggleablePatch<ThingDef> VPETidalGeneratorIsFlickablePatch = new ToggleablePatch<ThingDef>()
    {
      Name = "VPE Tidal Generator Is Flickable",
      Enabled = true,
      TargetModID = "VanillaExpanded.VFEPower",
      TargetDefName = "VFE_TidalGenerator",
      Patch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.Add((CompProperties) new CompProperties_Flickable())),
      Unpatch = (Action<ToggleablePatch<ThingDef>, ThingDef>) ((patch, def) => def.comps.RemoveAll((Predicate<CompProperties>) (c => c is CompProperties_Flickable)))
    };
    private static System.Version version = Assembly.GetAssembly(typeof (WaterFreezes)).GetName().Version;
    public static string Version = WaterFreezes.version.Major.ToString() + "." + WaterFreezes.version.Minor.ToString() + "." + WaterFreezes.version.Build.ToString();

    static WaterFreezes()
    {
      WaterFreezes.Log("Initializing..");
      ToggleablePatch.ProcessPatches("UdderlyEvelyn.WaterFreezes");
      new Harmony("UdderlyEvelyn.WaterFreezes").PatchAll();
      WaterFreezesStatCache.Initialize();
    }

    public static void Log(
      string message,
      ErrorLevel errorLevel = ErrorLevel.Message,
      int errorOnceKey = 0,
      bool ignoreStopLoggingLimit = false)
    {
      if (ignoreStopLoggingLimit)
        Verse.Log.ResetMessageCount();
      string text = "[Water Freezes " + WaterFreezes.Version + "] " + message;
      switch (errorLevel)
      {
        case ErrorLevel.Message:
          Verse.Log.Message(text);
          break;
        case ErrorLevel.Warning:
          Verse.Log.Warning(text);
          break;
        case ErrorLevel.Error:
          Verse.Log.Error(text);
          break;
        case ErrorLevel.ErrorOnce:
          Verse.Log.ErrorOnce(text, errorOnceKey);
          break;
      }
    }
  }
}
