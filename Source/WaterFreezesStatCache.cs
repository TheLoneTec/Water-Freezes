// Decompiled with JetBrains decompiler
// Type: WF.WaterFreezesStatCache
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Verse;

namespace WF
{
  public static class WaterFreezesStatCache
  {
    private static Dictionary<TerrainDef, TerrainExtension_WaterStats> extensionPerDef = new Dictionary<TerrainDef, TerrainExtension_WaterStats>();
    public static HashSet<TerrainDef> FreezableWater = new HashSet<TerrainDef>();
    public static HashSet<TerrainDef> ThawableIce = new HashSet<TerrainDef>();

    public static void Initialize()
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      foreach (TerrainDef allDef in DefDatabase<TerrainDef>.AllDefs)
      {
        TerrainExtension_WaterStats modExtension = allDef.GetModExtension<TerrainExtension_WaterStats>();
        if (modExtension != null)
        {
          WaterFreezesStatCache.extensionPerDef.Add(allDef, modExtension);
          if (modExtension.ThinIceDef != null)
          {
            WaterFreezesStatCache.FreezableWater.Add(allDef);
            WaterFreezesStatCache.ThawableIce.Add(modExtension.ThinIceDef);
          }
          if (modExtension.IceDef != null)
            WaterFreezesStatCache.ThawableIce.Add(modExtension.IceDef);
          if (modExtension.ThickIceDef != null)
            WaterFreezesStatCache.ThawableIce.Add(modExtension.ThickIceDef);
        }
      }
      stopwatch.Stop();
      WaterFreezes.Log("Generated stat cache in " + (stopwatch.ElapsedMilliseconds > 0L ? stopwatch.ElapsedMilliseconds.ToString() + "ms" : ((double) stopwatch.ElapsedTicks / 10.0).ToString() + "μs"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TerrainExtension_WaterStats GetExtension(TerrainDef def) => WaterFreezesStatCache.extensionPerDef[def];
  }
}
