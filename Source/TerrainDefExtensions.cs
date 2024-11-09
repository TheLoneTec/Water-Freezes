// Decompiled with JetBrains decompiler
// Type: WF.TerrainDefExtensions
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using RimWorld;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace WF
{
  public static class TerrainDefExtensions
  {
    private static Dictionary<TerrainDef, bool> bridgeCache = new Dictionary<TerrainDef, bool>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBridge(this TerrainDef def)
    {
      if (!TerrainDefExtensions.bridgeCache.ContainsKey(def))
        TerrainDefExtensions.bridgeCache[def] = def.bridge || def.label.ToLowerInvariant().Contains("bridge") || def.defName.ToLowerInvariant().Contains("bridge");
      return TerrainDefExtensions.bridgeCache[def];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFreezableWater(this TerrainDef def) => WaterFreezesStatCache.FreezableWater.Contains(def);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsThawableIce(this TerrainDef def) => WaterFreezesStatCache.ThawableIce.Contains(def);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShallowDepth(this TerrainDef def) => def == TerrainDefOf.WaterShallow || def == TerrainDefOf.WaterMovingShallow;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDeepDepth(this TerrainDef def) => def == TerrainDefOf.WaterDeep || def == TerrainDefOf.WaterMovingChestDeep;
  }
}
