// Decompiled with JetBrains decompiler
// Type: WF.MapComponent_WaterFreezes
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using RimWorld;
using SK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace WF
{
  public class MapComponent_WaterFreezes : MapComponent
  {
    public bool Initialized;
    public TerrainDef[] NaturalWaterTerrainGrid;
    public TerrainDef[] AllWaterTerrainGrid;
    public float[] IceDepthGrid;
    public float[] WaterDepthGrid;
    public float[] PseudoWaterElevationGrid;
    public float ThresholdThinIce = 0.15f;
    public float ThresholdIce = 50f;
    public float ThresholdThickIce = 110f;
    private Season season;
    private int seasonLastUpdated = 0;
    public List<string> BreakdownOrDestroyExceptedDefNames = new List<string>()
    {
      "ludeon.rimworld.royalty.Shuttle",
      "ludeon.rimworld.royalty.ShuttleCrashed",
      "dubwise.dubsbadhygiene.sewagePipeStuff",
      "aelanna.arimreborn.core.ARR_AetherSpotWater",
      "owlchemist.invisiblewalls.Owl_InvisibleWall",
      "somewhereoutinspace.spaceports.Spaceports_FuelProcessor",
      "skyarkhangel.hsk.FishingPier",
      "skyarkhangel.hsk.CrashedShipWreck",
      "skyarkhangel.hsk.ShipWreck",
      "skyarkhangel.hsk.ShipChunk",
      "skyarkhangel.hsk.ShipMechChunk",
      "skyarkhangel.hsk.DropPodLanded",
      "skyarkhangel.hsk.PsychicDronerShipPart",
      "skyarkhangel.hsk.DefoliatorShipPart"
    };
    public List<string> BreakdownOrDestroyExceptedPlaceWorkerTypeStrings = new List<string>()
    {
      "RimWorld.PlaceWorker_Conduit"
    };
    public List<string> BreakdownOrDestroyExceptedPlaceWorkerFailureReasons = new List<string>()
    {
      (string) "VPE_NeedsDistance".Translate(),
      (string) "WFFT_NeedsDistance".Translate(),
      (string) "RBB.TrapTooClose".Translate(),
      (string) "RBB.FSTooClose".Translate(),
      (string) "VME_NeedsDistance".Translate()
    };

    public MapComponent_WaterFreezes(Map map)
      : base(map)
    {
      WaterFreezes.Log("New MapComponent constructed (for map " + map.uniqueID.ToString() + ") adding it to the cache.");
      WaterFreezesCompCache.SetFor(map, this);
    }

    public override void MapGenerated() => this.Initialize();

    public override void MapRemoved()
    {
      WaterFreezes.Log("Removing MapComponent from cache due to map removal (for map " + this.map.uniqueID.ToString() + ").");
      WaterFreezesCompCache.compCachePerMap.Remove(this.map.uniqueID);
    }

    public void Initialize()
    {
      WaterFreezes.Log("MapComponent Initializing (for map " + this.map.uniqueID.ToString() + ")..");
      if (this.WaterDepthGrid == null)
      {
        WaterFreezes.Log("Instantiating water depth grid..");
        this.WaterDepthGrid = new float[this.map.cellIndices.NumGridCells];
      }
      if (this.NaturalWaterTerrainGrid == null)
      {
        WaterFreezes.Log("Generating natural water grid and populating water depth grid..");
        this.NaturalWaterTerrainGrid = new TerrainDef[this.map.cellIndices.NumGridCells];
        for (int ind = 0; ind < this.map.cellIndices.NumGridCells; ++ind)
        {
          TerrainDef def1 = this.map.terrainGrid.TerrainAt(ind);
          if (def1.IsFreezableWater())
          {
            this.NaturalWaterTerrainGrid[ind] = def1;
            this.WaterDepthGrid[ind] = WaterFreezesStatCache.GetExtension(def1).MaxWaterDepth;
          }
          else if (def1.IsBridge())
          {
            TerrainDef def2 = this.map.terrainGrid.UnderTerrainAt(ind);
            if (def2.IsFreezableWater())
            {
              this.NaturalWaterTerrainGrid[ind] = def2;
              this.WaterDepthGrid[ind] = WaterFreezesStatCache.GetExtension(def2).MaxWaterDepth;
            }
          }
        }
      }
      if (this.AllWaterTerrainGrid == null)
      {
        WaterFreezes.Log("Cloning natural water grid into all water grid..");
        this.AllWaterTerrainGrid = (TerrainDef[]) this.NaturalWaterTerrainGrid.Clone();
      }
      if (this.IceDepthGrid == null)
      {
        WaterFreezes.Log("Instantiating ice depth grid..");
        this.IceDepthGrid = new float[this.map.cellIndices.NumGridCells];
      }
      if (this.PseudoWaterElevationGrid == null)
      {
        this.PseudoWaterElevationGrid = new float[this.map.cellIndices.NumGridCells];
        this.UpdatePseudoWaterElevationGrid();
      }
      this.Initialized = true;
    }

    public override void MapComponentTick()
    {
      if (!this.Initialized)
        this.Initialize();
      if ((uint) (Find.TickManager.TicksGame % WaterFreezesSettings.IceRate) > 0U)
        return;
      for (int ind = 0; ind < this.AllWaterTerrainGrid.Length; ++ind)
      {
        IntVec3 cell = this.map.cellIndices.IndexToCell(ind);
        TerrainDef def = this.AllWaterTerrainGrid[ind];
        if (def != null)
        {
          TerrainExtension_WaterStats extension = WaterFreezesStatCache.GetExtension(def);
          this.UpdateIceForTemperature(cell, extension);
          this.UpdateIceStage(cell, extension);
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdatePseudoWaterElevationGrid()
    {
      WaterFreezes.Log("Updating pseudo water elevation grid..");
      for (int ind = 0; ind < this.AllWaterTerrainGrid.Length; ++ind)
      {
        if (this.AllWaterTerrainGrid[ind] != null)
          this.UpdatePseudoWaterElevationGridForCell(this.map.cellIndices.IndexToCell(ind));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdatePseudoWaterElevationGridForCell(IntVec3 cell)
    {
      int index1 = this.map.cellIndices.CellToIndex(cell);
      List<IntVec3> intVec3List = GenAdjFast.AdjacentCells8Way(cell);
      float num = 0.0f;
      for (int index2 = 0; index2 < intVec3List.Count; ++index2)
      {
        int index3 = this.map.cellIndices.CellToIndex(intVec3List[index2]);
        if (index3 >= 0 && index3 < this.map.terrainGrid.topGrid.Length && this.AllWaterTerrainGrid[index3] == null)
          ++num;
      }
      this.PseudoWaterElevationGrid[index1] = num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdatePseudoWaterElevationGridAtAndAroundCell(IntVec3 cell)
    {
      int index1 = this.map.cellIndices.CellToIndex(cell);
      List<IntVec3> intVec3List = GenAdjFast.AdjacentCells8Way(cell);
      float num = 0.0f;
      for (int index2 = 0; index2 < intVec3List.Count; ++index2)
      {
        IntVec3 intVec3 = intVec3List[index2];
        int index3 = this.map.cellIndices.CellToIndex(intVec3);
        if (index3 >= 0 && index3 < this.map.terrainGrid.topGrid.Length)
        {
          if (this.AllWaterTerrainGrid[index3] == null)
            ++num;
          else
            this.UpdatePseudoWaterElevationGridForCell(intVec3);
        }
      }
      this.PseudoWaterElevationGrid[index1] = num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TerrainExtension_WaterStats SetMaxWaterByDef(
      int i,
      TerrainDef water = null,
      bool updateIceStage = true)
    {
      if (water == null)
        water = this.AllWaterTerrainGrid[i];
      TerrainExtension_WaterStats extension = WaterFreezesStatCache.GetExtension(water);
      this.WaterDepthGrid[i] = extension.MaxWaterDepth;
      if (updateIceStage)
        this.UpdateIceStage(this.map.cellIndices.IndexToCell(i));
      return extension;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateIceForTemperature(IntVec3 cell, TerrainExtension_WaterStats extension = null)
    {
      float temperatureForCell = GenTemperature.GetTemperatureForCell(cell, this.map);
      int index1 = this.map.cellIndices.CellToIndex(cell);
      TerrainDef def = this.AllWaterTerrainGrid[index1];
      if (extension == null)
        extension = WaterFreezesStatCache.GetExtension(def);
      float num1 = temperatureForCell - extension.FreezingPoint;
      if ((double) num1 == 0.0)
        return;
      float num2 = this.IceDepthGrid[index1];
      float num3 = this.WaterDepthGrid[index1];
      if ((double) num1 < 0.0)
      {
        if ((double) num3 <= 0.0 || (double) num2 >= (double) extension.MaxIceDepth)
          return;
        float num4 = this.PseudoWaterElevationGrid[index1];
        if ((double) num4 == 0.0)
        {
          List<IntVec3> intVec3List = GenAdjFast.AdjacentCells8Way(cell);
          bool flag = false;
          for (int index2 = 0; index2 < intVec3List.Count; ++index2)
          {
            int index3 = this.map.cellIndices.CellToIndex(intVec3List[index2]);
            if (index3 >= 0 && index3 < this.map.terrainGrid.topGrid.Length && this.map.terrainGrid.TerrainAt(index3).IsThawableIce())
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return;
        }
        float num5 = (float) (-(double) num1 * ((double) WaterFreezesSettings.FreezingFactor + (double) num4) / 2500.0) * (float) WaterFreezesSettings.IceRate;
        ref float local = ref this.IceDepthGrid[index1];
        double num6 = (double) local;
        double num7;
        float num8 = (float) (num7 = (double) num5 < (double) num3 ? (double) num5 : (double) num3);
        double num9;
        float num10 = (float) (num9 = num6 + num7);
        local = (float) num9;
        if ((double) num10 > (double) extension.MaxIceDepth)
        {
          this.IceDepthGrid[index1] = extension.MaxIceDepth;
          num8 = extension.MaxIceDepth - num2;
        }
        float num11 = num3 - num8;
        this.WaterDepthGrid[index1] = (double) num11 > 0.0 ? num11 : 0.0f;
      }
      else
      {
        if ((double) num1 <= 0.0 || (double) num2 <= 0.0)
          return;
        float num12 = this.PseudoWaterElevationGrid[index1];
        float num13 = WaterFreezesSettings.ThawingFactor + (extension.IsMoving ? num12 : -num12);
        float num14 = (float) ((double) num1 / ((double) num13 > 1.0 ? (double) num13 : 1.0) / ((double) num2 / 100.0) / 2500.0) * (float) WaterFreezesSettings.IceRate;
        float num15;
        this.IceDepthGrid[index1] -= num15 = (double) num14 < (double) num2 ? num14 : num2;
        float num16 = num3 + num15;
        this.WaterDepthGrid[index1] = (double) num16 > (double) extension.MaxWaterDepth ? extension.MaxWaterDepth : num16;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateIceStage(
      IntVec3 cell,
      TerrainExtension_WaterStats extension = null,
      TerrainDef currentTerrain = null,
      TerrainDef underTerrain = null)
    {
      int index = this.map.cellIndices.CellToIndex(cell);
      float iceDepth = this.IceDepthGrid[index];
      float waterDepth = this.WaterDepthGrid[index];
      TerrainDef waterTerrain = this.AllWaterTerrainGrid[index];
      if (currentTerrain == null)
        currentTerrain = this.map.terrainGrid.TerrainAt(index);
      if (underTerrain == null)
        underTerrain = this.map.terrainGrid.UnderTerrainAt(index);
      TerrainDef appropriateTerrainFor = this.GetAppropriateTerrainFor(waterTerrain, waterDepth, iceDepth, extension);
      if (appropriateTerrainFor != null)
      {
        if (underTerrain.IsThawableIce() || currentTerrain.IsBridge())
        {
          if (underTerrain != appropriateTerrainFor)
            this.map.terrainGrid.SetUnderTerrain(cell, appropriateTerrainFor);
        }
        else if (currentTerrain != appropriateTerrainFor)
          this.map.terrainGrid.SetTerrain(cell, appropriateTerrainFor);
      }
      this.CheckAndRefillCell(cell, extension);
      if (currentTerrain.IsBridge())
        return;
      this.BreakdownOrDestroyBuildingsInCellIfInvalid(cell);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TerrainDef GetAppropriateTerrainFor(
      TerrainDef waterTerrain,
      float waterDepth,
      float iceDepth,
      TerrainExtension_WaterStats extension = null)
    {
      float num = iceDepth / (iceDepth + waterDepth);
      if ((double) iceDepth == 0.0 || (double) num < (double) this.ThresholdThinIce)
        return (double) waterDepth > 0.0 ? waterTerrain : (TerrainDef) null;
      if ((double) iceDepth < (double) this.ThresholdIce)
        return extension.ThinIceDef;
      if (extension.IceDef != null && (double) iceDepth < (double) this.ThresholdThickIce)
        return extension.IceDef;
      if (extension.ThickIceDef != null)
        return extension.ThickIceDef;
      string message = "GetAppropriateTerrainFor failed to find appropriate terrain for \"" + waterTerrain.defName + "\" with depth " + waterDepth.ToString() + " and ice depth " + iceDepth.ToString() + ".";
      WaterFreezes.Log(message, ErrorLevel.Error);
      throw new InvalidOperationException(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CheckAndRefillCell(IntVec3 cell, TerrainExtension_WaterStats extension = null)
    {
      if ((double) cell.GetTemperature(this.map) <= 0.0)
        return;
      int index = this.map.cellIndices.CellToIndex(cell);
      TerrainDef def = this.NaturalWaterTerrainGrid[index];
      float num1 = this.WaterDepthGrid[index];
      if (extension == null)
        extension = WaterFreezesStatCache.GetExtension(def);
      if (def == null || (double) num1 >= (double) extension.MaxWaterDepth)
        return;
      if (Find.TickManager.TicksGame - this.seasonLastUpdated > 60000)
        this.season = GenLocalDate.Season(this.map);
      if (this.season == Season.Spring || this.season == Season.Summer || this.season == Season.PermanentSummer)
      {
        float num2 = num1 + 0.0004f * (float) WaterFreezesSettings.IceRate;
        this.WaterDepthGrid[index] = (double) num2 < (double) extension.MaxWaterDepth ? num2 : extension.MaxWaterDepth;
      }
    }

    public void BreakdownOrDestroyBuildingsInCellIfInvalid(IntVec3 cell)
    {
      TerrainDef terrain = cell.GetTerrain(this.map);
      List<Thing> thingList = cell.GetThingList(this.map);
      for (int index = 0; index < thingList.Count; ++index)
      {
        Thing thing = thingList[index];
        if (thing != null)
        {
          bool flag1 = false;
          bool flag2 = false;

          if (thing.def.IsFrame)
              return;

          if (thing is Building && thing.def.destroyable && (thing.questTags == null || thing.questTags.Count <= 0) && !thing.def.defName.StartsWith("Ancient")
                        && !thing.def.defName.StartsWith("VFEA_") && (thing.def.modContentPack == null
                        || !this.BreakdownOrDestroyExceptedDefNames.Contains(thing.def.modContentPack.PackageId + "." + thing.def.defName))
                        && (thing.def.building == null || thing.def.building.buildingTags == null || !thing.def.building.buildingTags.Contains("DoesNotSink")))
          {
            if (thing.def.PlaceWorkers != null)
            {
              foreach (PlaceWorker placeWorker in thing.def.PlaceWorkers)
              {
                if (!this.BreakdownOrDestroyExceptedPlaceWorkerTypeStrings.Contains(placeWorker.ToString()))
                {
                  AcceptanceReport acceptanceReport = placeWorker.AllowsPlacing((BuildableDef) thing.def, thing.Position, thing.Rotation, this.map);
                  if (!(bool) acceptanceReport && !this.BreakdownOrDestroyExceptedPlaceWorkerFailureReasons.Contains(acceptanceReport.Reason))
                  {
                    flag2 = true;
                    break;
                  }
                }
              }
            }
            if (thing.TerrainAffordanceNeeded != null && thing.TerrainAffordanceNeeded.defName != "" && terrain.affordances != null && !terrain.affordances.Contains(thing.TerrainAffordanceNeeded))
            {
              flag2 = true;
              flag1 = true;
            }
            else
            {
              flag2 = false;
            }
            if (flag2)
            {
              if (thing is ThingWithComps thingWithComps)
              {
                CompFlickable comp1 = thingWithComps.GetComp<CompFlickable>();
                CompBreakdownable comp2 = thingWithComps.GetComp<CompBreakdownable>();
                if (comp1 != null && comp2 != null)
                {
                  if (comp1.SwitchIsOn && !comp2.BrokenDown)
                  {
                    comp2.DoBreakdown();
                    comp1.DoFlick();
                  }
                }
                else if ((!flag1 || !terrain.IsWater) && comp2 != null)
                {
                  if (!comp2.BrokenDown)
                    comp2.DoBreakdown();
                }
                else
                {
                    Log.Message("Destroying: " + thing.def.defName);
                    thing.Destroy(DestroyMode.FailConstruction);
                }

              }
            else
            {
                Log.Message("Destroying: " + thing.def.defName);
                thing.Destroy(DestroyMode.FailConstruction);
            }

            }
          }
        }
      }
    }

    public static int GetWaterVolumeCellsInRadius(Map map, IntVec3 position, float radius, out float currentVolume)
    {
        int aquaticCellsNumber = 0;
        currentVolume = 0;
        float[] waterGrid = WaterFreezesCompCache.GetFor(map).WaterDepthGrid;
        //float[] iceGrid = WaterFreezesCompCache.GetFor(map).IceDepthGrid;

        if (radius <= 0)
        {
            return 0;
        }
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(position, radius, true))
        {
            if (cell.InBounds(map) == false)
            {
                continue;
            }
            currentVolume += waterGrid[map.cellIndices.CellToIndex(cell)];
            //Log.Message("WaterGrid: " + waterGrid[map.cellIndices.CellToIndex(cell)]);
            //Log.Message("IceGrid: " + iceGrid[map.cellIndices.CellToIndex(cell)]);
            if (Util_Zone_Fishing.IsAquaticTerrain(map, cell) && waterGrid[map.cellIndices.CellToIndex(cell)] > 50)
            {
                aquaticCellsNumber++;
            }
        }
        return aquaticCellsNumber;
    }
    public override void ExposeData()
    {
      List<float> list1 = new List<float>();
      List<float> list2 = new List<float>();
      List<float> list3 = new List<float>();
      if (Scribe.mode == LoadSaveMode.Saving)
      {
        List<string> list4 = new List<string>();
        List<string> list5 = new List<string>();
        if (this.AllWaterTerrainGrid != null)
          list5 = ((IEnumerable<TerrainDef>) this.AllWaterTerrainGrid).Select<TerrainDef, string>((Func<TerrainDef, string>) (def => def != null ? def.defName : "null")).ToList<string>();
        if (this.NaturalWaterTerrainGrid != null)
          list4 = ((IEnumerable<TerrainDef>) this.NaturalWaterTerrainGrid).Select<TerrainDef, string>((Func<TerrainDef, string>) (def => def != null ? def.defName : "null")).ToList<string>();
        if (this.IceDepthGrid != null)
          list1 = ((IEnumerable<float>) this.IceDepthGrid).ToList<float>();
        if (this.WaterDepthGrid != null)
          list2 = ((IEnumerable<float>) this.WaterDepthGrid).ToList<float>();
        if (this.PseudoWaterElevationGrid != null)
          list3 = ((IEnumerable<float>) this.PseudoWaterElevationGrid).ToList<float>();
        Scribe_Collections.Look<string>(ref list4, "NaturalWaterTerrainGrid", LookMode.Undefined);
        Scribe_Collections.Look<string>(ref list5, "AllWaterTerrainGrid", LookMode.Undefined);
      }
      Scribe_Collections.Look<float>(ref list1, "IceDepthGrid", LookMode.Undefined);
      Scribe_Collections.Look<float>(ref list2, "WaterDepthGrid", LookMode.Undefined);
      Scribe_Collections.Look<float>(ref list3, "PseudoElevationGrid", LookMode.Undefined);
      if (Scribe.mode == LoadSaveMode.LoadingVars)
      {
        List<TerrainDef> list6 = new List<TerrainDef>();
        List<TerrainDef> list7 = new List<TerrainDef>();
        Scribe_Collections.Look<TerrainDef>(ref list6, "NaturalWaterTerrainGrid", LookMode.Undefined);
        Scribe_Collections.Look<TerrainDef>(ref list7, "AllWaterTerrainGrid", LookMode.Undefined);
        this.NaturalWaterTerrainGrid = list6.ToArray();
        this.AllWaterTerrainGrid = list7.ToArray();
        this.IceDepthGrid = list1.ToArray();
        this.WaterDepthGrid = list2.ToArray();
        this.PseudoWaterElevationGrid = list3.ToArray();
      }
      base.ExposeData();
    }
  }
}
