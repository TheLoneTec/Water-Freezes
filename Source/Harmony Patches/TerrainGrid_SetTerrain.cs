// Decompiled with JetBrains decompiler
// Type: WF.TerrainGrid_SetTerrain
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using Verse;

namespace WF
{
  [HarmonyPatch(typeof (TerrainGrid), "SetTerrain")]
  public class TerrainGrid_SetTerrain
  {
    internal static void Prefix(IntVec3 c, TerrainDef newTerr, Map ___map, ref TerrainDef __state) => __state = ___map.terrainGrid.TerrainAt(c);

    internal static void Postfix(
      IntVec3 c,
      TerrainDef newTerr,
      Map ___map,
      ref TerrainDef __state)
    {
      if (__state == newTerr || __state == null)
        return;
      int index = ___map.cellIndices.CellToIndex(c);
      if (__state.IsFreezableWater())
      {
        if (newTerr.IsFreezableWater())
          return;
        MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(___map);
        if (componentWaterFreezes == null || !componentWaterFreezes.Initialized)
          return;
        if (componentWaterFreezes.NaturalWaterTerrainGrid[index] == null && !newTerr.IsThawableIce())
        {
          componentWaterFreezes.AllWaterTerrainGrid[index] = (TerrainDef) null;
          componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
        }
        ___map.snowGrid.SetDepth(c, 0.0f);
        componentWaterFreezes.UpdatePseudoWaterElevationGridAtAndAroundCell(c);
      }
      else
      {
        MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(___map);
        if (componentWaterFreezes == null || !componentWaterFreezes.Initialized || __state.IsThawableIce() || !newTerr.IsFreezableWater())
          return;
        if (componentWaterFreezes.NaturalWaterTerrainGrid[index] == null && !newTerr.IsThawableIce())
        {
          componentWaterFreezes.AllWaterTerrainGrid[index] = newTerr;
          componentWaterFreezes.SetMaxWaterByDef(index, newTerr, false);
        }
        componentWaterFreezes.UpdatePseudoWaterElevationGridAtAndAroundCell(c);
      }
    }
  }
}
