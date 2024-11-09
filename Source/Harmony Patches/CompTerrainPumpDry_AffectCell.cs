// Decompiled with JetBrains decompiler
// Type: WF.CompTerrainPumpDry_AffectCell
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using RimWorld;
using Verse;

namespace WF
{
  [HarmonyPatch(typeof (CompTerrainPumpDry), "AffectCell", new System.Type[] {typeof (Map), typeof (IntVec3)})]
  public class CompTerrainPumpDry_AffectCell
  {
    internal static void Postfix(Map map, IntVec3 c)
    {
      TerrainDef terrain = c.GetTerrain(map);
      if (terrain != TerrainDefOf.WaterDeep && terrain != TerrainDefOf.WaterShallow && terrain != WaterDefs.Marsh && terrain != TerrainDefOf.WaterMovingShallow && terrain != TerrainDefOf.WaterMovingChestDeep)
        return;
      int index = map.cellIndices.CellToIndex(c);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      if (WaterFreezesSettings.MoisturePumpClearsNaturalWater)
        componentWaterFreezes.NaturalWaterTerrainGrid[index] = (TerrainDef) null;
      componentWaterFreezes.AllWaterTerrainGrid[index] = (TerrainDef) null;
      componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
    }
  }
}
