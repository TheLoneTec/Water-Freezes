// Decompiled with JetBrains decompiler
// Type: WF.API
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using Verse;

namespace WF
{
  public static class API
  {
    public static bool IsThawableIce(TerrainDef def) => def.IsThawableIce();

    public static float TakeCellIce(Map map, IntVec3 cell)
    {
      int index = map.cellIndices.CellToIndex(cell);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      float cellIce = componentWaterFreezes.IceDepthGrid[index];
      componentWaterFreezes.IceDepthGrid[index] = 0.0f;
      componentWaterFreezes.UpdateIceStage(cell);
      return cellIce;
    }

    public static float QueryCellIce(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).IceDepthGrid[map.cellIndices.CellToIndex(cell)];

    public static float QueryCellWater(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).WaterDepthGrid[map.cellIndices.CellToIndex(cell)];

    public static TerrainDef QueryCellNaturalWater(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).NaturalWaterTerrainGrid[map.cellIndices.CellToIndex(cell)];

    public static TerrainDef QueryCellAllWater(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).AllWaterTerrainGrid[map.cellIndices.CellToIndex(cell)];

    public static void ClearCellNaturalWater(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).NaturalWaterTerrainGrid[map.cellIndices.CellToIndex(cell)] = (TerrainDef) null;

    public static void ClearCellWater(Map map, IntVec3 cell) => WaterFreezesCompCache.GetFor(map).WaterDepthGrid[map.cellIndices.CellToIndex(cell)] = 0.0f;
  }
}
