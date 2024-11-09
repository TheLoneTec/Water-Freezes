// Decompiled with JetBrains decompiler
// Type: WF.DebugActionsWaterFreezes
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using LudeonTK;
using RimWorld;
using System;
using System.Runtime.CompilerServices;
using Verse;

namespace WF
{
  public static class DebugActionsWaterFreezes
  {
    [DebugAction("Water Freezes", "Reinitialize MapComponent", false, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_ReinitializeMapComponent() => DebugActionsWaterFreezes.ReinitializeMapComponent(Find.CurrentMap);

    public static void ReinitializeMapComponent(Map map)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      for (int ind = 0; ind < componentWaterFreezes.AllWaterTerrainGrid.Length; ++ind)
      {
        TerrainDef newTerr = componentWaterFreezes.AllWaterTerrainGrid[ind];
        if (newTerr != null && map.terrainGrid.TerrainAt(ind) != newTerr)
        {
          componentWaterFreezes.IceDepthGrid[ind] = 0.0f;
          map.terrainGrid.SetTerrain(map.cellIndices.IndexToCell(ind), newTerr);
        }
      }
      componentWaterFreezes.AllWaterTerrainGrid = (TerrainDef[]) null;
      componentWaterFreezes.NaturalWaterTerrainGrid = (TerrainDef[]) null;
      componentWaterFreezes.WaterDepthGrid = (float[]) null;
      componentWaterFreezes.IceDepthGrid = (float[]) null;
      componentWaterFreezes.PseudoWaterElevationGrid = (float[]) null;
      componentWaterFreezes.Initialize();
      Messages.Message("Water Freezes MapComponent was reinitialized.", MessageTypeDefOf.TaskCompletion);
    }

    [DebugAction("Water Freezes", "Set As Natural Water", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetAsNaturalWater() => DebugActionsWaterFreezes.SetAsNaturalWater(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set As Natural Water", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetAsNaturalWater_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetAsNaturalWater));

    public static bool SetAsNaturalWater(Map map, IntVec3 cell, bool sendMessage = true)
    {
      int index = map.cellIndices.CellToIndex(cell);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      TerrainDef terrainDef = componentWaterFreezes.AllWaterTerrainGrid[index];
      if (terrainDef != null)
      {
        componentWaterFreezes.NaturalWaterTerrainGrid[index] = terrainDef;
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set natural water status for non-water (or unsupported water) terrain.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Clear Natural Water Status", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_ClearNaturalWaterStatus() => DebugActionsWaterFreezes.ClearNaturalWaterStatus(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Clear Natural Water Status", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_ClearNaturalWaterStatus_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.ClearNaturalWaterStatus));

    public static bool ClearNaturalWaterStatus(Map map, IntVec3 cell, bool sendMessage = true)
    {
      int index = map.cellIndices.CellToIndex(cell);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      if (componentWaterFreezes.NaturalWaterTerrainGrid[index] != null)
      {
        componentWaterFreezes.NaturalWaterTerrainGrid[index] = (TerrainDef) null;
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set natural water status to null where it was already null.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Clear Natural Water Status/Depth", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_ClearNaturalWaterStatusAndDepth() => DebugActionsWaterFreezes.ClearNaturalWaterStatusAndDepth(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Clear Natural Water Status/Depth", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_ClearNaturalWaterStatusAndDepth_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.ClearNaturalWaterStatusAndDepth));

    public static bool ClearNaturalWaterStatusAndDepth(Map map, IntVec3 cell, bool sendMessage = true)
    {
      int index = map.cellIndices.CellToIndex(cell);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      if (componentWaterFreezes.NaturalWaterTerrainGrid[index] != null)
      {
        componentWaterFreezes.NaturalWaterTerrainGrid[index] = (TerrainDef) null;
        componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
        componentWaterFreezes.UpdateIceStage(cell);
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set natural water status to null where it was already null.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Set Water Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetWaterDepthToMax() => DebugActionsWaterFreezes.SetWaterDepthToMax(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Water Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetWaterDepthToMax_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetWaterDepthToMax));

    public static bool SetWaterDepthToMax(Map map, IntVec3 cell, bool sendMessage = true)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      int index = map.cellIndices.CellToIndex(cell);
      TerrainDef water = componentWaterFreezes.AllWaterTerrainGrid[index];
      if (water == null)
      {
        if (sendMessage)
          Messages.Message("Attempted to set water depth to max for non-water (or unrecognized water) terrain.", MessageTypeDefOf.RejectInput);
        return false;
      }
      componentWaterFreezes.SetMaxWaterByDef(index, water);
      return true;
    }

    [DebugAction("Water Freezes", "Set Water Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetWaterDepthToZero() => DebugActionsWaterFreezes.SetWaterDepthToZero(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Water Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetWaterDepthToZero_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetWaterDepthToZero));

    public static bool SetWaterDepthToZero(Map map, IntVec3 cell, bool sendMessage = true)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      int index = map.cellIndices.CellToIndex(cell);
      if (componentWaterFreezes.AllWaterTerrainGrid[index] != null)
      {
        componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
        componentWaterFreezes.UpdateIceStage(cell);
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set water depth to zero for non-water (or unsupported water) terrain.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Set Ice Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceDepthToMax() => DebugActionsWaterFreezes.SetIceDepthToMax(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Ice Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceDepthToMax_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetIceDepthToMax));

    public static bool SetIceDepthToMax(Map map, IntVec3 cell, bool sendMessage = true)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      int index = map.cellIndices.CellToIndex(cell);
      TerrainDef def = componentWaterFreezes.AllWaterTerrainGrid[index];
      if (def == null)
      {
        if (sendMessage)
          Messages.Message("Attempted to set ice depth to max for non-water (or unsupported water) terrain.", MessageTypeDefOf.RejectInput);
        return false;
      }
      TerrainExtension_WaterStats extension = WaterFreezesStatCache.GetExtension(def);
      componentWaterFreezes.IceDepthGrid[index] = extension.MaxIceDepth;
      componentWaterFreezes.UpdateIceStage(cell, extension);
      return true;
    }

    [DebugAction("Water Freezes", "Set Ice Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceDepthToZero() => DebugActionsWaterFreezes.SetIceDepthToZero(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Ice Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceDepthToZero_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetIceDepthToZero));

    public static bool SetIceDepthToZero(Map map, IntVec3 cell, bool sendMessage = true)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      int index = map.cellIndices.CellToIndex(cell);
      if (componentWaterFreezes.AllWaterTerrainGrid[index] != null)
      {
        componentWaterFreezes.IceDepthGrid[index] = 0.0f;
        componentWaterFreezes.UpdateIceStage(cell);
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set ice depth to zero for non-water terrain.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Set Ice/Water Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceAndWaterDepthToZero() => DebugActionsWaterFreezes.SetIceAndWaterDepthToZero(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Ice/Water Depth To Zero", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetIceAndWaterDepthToZero_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetIceAndWaterDepthToZero));

    public static bool SetIceAndWaterDepthToZero(Map map, IntVec3 cell, bool sendMessage = true)
    {
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      int index = map.cellIndices.CellToIndex(cell);
      if (componentWaterFreezes.AllWaterTerrainGrid[index] != null)
      {
        componentWaterFreezes.IceDepthGrid[index] = 0.0f;
        componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
        componentWaterFreezes.UpdateIceStage(cell);
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set ice & water depth to zero for non-water terrain.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [DebugAction("Water Freezes", "Set Nat. Water/Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetNaturalWaterAndWaterDepthToMax() => DebugActionsWaterFreezes.SetNaturalWaterAndWaterDepthToMax(Find.CurrentMap, UI.MouseCell());

    [DebugAction("Water Freezes (Rect)", "Set Nat. Water/Depth To Max", false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DebugAction_SetNaturalWaterAndDepthToMax_Rect() => DebugActionsWaterFreezes.DoForRect(new Func<Map, IntVec3, bool, bool>(DebugActionsWaterFreezes.SetNaturalWaterAndWaterDepthToMax));

    private static bool SetNaturalWaterAndWaterDepthToMax(Map map, IntVec3 cell, bool sendMessage = true)
    {
      int index = map.cellIndices.CellToIndex(cell);
      MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(map);
      TerrainDef def = componentWaterFreezes.AllWaterTerrainGrid[index];
      if (def != null)
      {
        componentWaterFreezes.NaturalWaterTerrainGrid[index] = def;
        TerrainExtension_WaterStats extension = WaterFreezesStatCache.GetExtension(def);
        componentWaterFreezes.WaterDepthGrid[index] = extension.MaxWaterDepth;
        componentWaterFreezes.UpdateIceStage(cell, extension);
        return true;
      }
      if (sendMessage)
        Messages.Message("Attempted to set natural water and water depth to max for non-water (or unsupported water) terrain.", MessageTypeDefOf.RejectInput);
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DoForRect(Func<Map, IntVec3, bool, bool> action)
    {
      Map map = Find.CurrentMap;
      DebugTool tool = (DebugTool) null;
      IntVec3 firstCorner;
      tool = new DebugTool("first corner...", (Action) (() =>
      {
        firstCorner = UI.MouseCell();
        DebugTools.curTool = new DebugTool("second corner...", (Action) (() =>
        {
          int num3 = 0;
          int num4 = 0;
          CellRect cellRect = CellRect.FromLimits(firstCorner, UI.MouseCell());
          cellRect = cellRect.ClipInsideMap(map);
          foreach (IntVec3 intVec3 in cellRect)
          {
            if (!action(map, intVec3, false))
              ++num3;
            ++num4;
          }
          DebugTools.curTool = tool;
          if (num3 > 0)
            Messages.Message("There were " + num3.ToString() + " failures to perform the requested operation on " + num4.ToString() + " cells.", MessageTypeDefOf.TaskCompletion);
          else
            Messages.Message("Successfully performed the operation on " + num4.ToString() + " cells.", MessageTypeDefOf.TaskCompletion);
        }), firstCorner);
      }));
      DebugTools.curTool = tool;
    }
  }
}
