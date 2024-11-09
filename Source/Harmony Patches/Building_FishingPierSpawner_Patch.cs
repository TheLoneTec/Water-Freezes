// Decompiled with JetBrains decompiler
// Type: WF.CompTerrainPumpDry_AffectCell
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using RimWorld;
using Verse;
using SK;
using System.Collections.Generic;
using UnityEngine;
using PatchOperationWhatHappened;
using Verse.Noise;

namespace WF
{
  [HarmonyPatch(typeof (Building_FishingPier), "TickRare")]
  public class TickRare_Patch
    {
    internal static bool Prefix(Building_FishingPier __instance)
        {
            if (__instance.def.defName != "HSK_IceFishingSpot")
                return true;
            if (WaterFreezesCompCache.GetFor(__instance.Map).WaterDepthGrid[__instance.Map.cellIndices.CellToIndex(__instance.Position)] < 10)
            {
                __instance.Destroy();
                Messages.Message("WaterFreezes_IceFishingSpotFrozen".Translate(), (LookTargets)new TargetInfo(__instance.Position, __instance.Map), MessageTypeDefOf.NeutralEvent);
                return false;
            }
            return true;
        }
  }
    /*
    [HarmonyPatch(typeof(PlaceWorker_FishingPierSpawner), "AllowsPlacing", new System.Type[] { typeof(BuildableDef), typeof(IntVec3), typeof(Rot4), typeof(Map), typeof(Thing), typeof(Thing) })]
    public class AllowsPlacing_Patch
    {
        internal static bool Prefix(PlaceWorker_FishingPierSpawner __instance, AcceptanceReport __result, BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (checkingDef.defName != "HSK_IceFishingSpot")
                return true;
            Log.Message("Test AllowsPlacing_Patch 1");
            // Check this biome contains some fishes.
            if (Util_FishIndustry.GetFishSpeciesList(map.Biome).NullOrEmpty())
            {
                __result = new AcceptanceReport("FishIndustry.FishingPier_InvalidBiome".Translate());
                return false;
            }
            Log.Message("Test AllowsPlacing_Patch 2");
            if (IsNearIceFishingSpot(map, loc, Util_PlaceWorker.minDistanceBetweenTwoFishingSpots))
            {
                __result = new AcceptanceReport("FishIndustry.TooCloseFishingPier".Translate());
                return false;
            }
            Log.Message("Test AllowsPlacing_Patch 3");
            // Check if a fishing zone is not too close.
            *//*
            if (Util_PlaceWorker.IsNearFishingZone(map, loc, Util_PlaceWorker.minDistanceBetweenTwoFishingSpots))
            {
                return new AcceptanceReport("FishIndustry.TooCloseFishingZone".Translate());
            }
            *//*
            if (!SK.TerrainCheck.IsFrozenIce(loc, map))
            {
                __result = new AcceptanceReport("FishIndustry.FishingPier_PierMustBeOnWater".Translate());
                return false;
            }
            Log.Message("Test AllowsPlacing_Patch 4");
            // Check fishing zone is on water.
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = 3; yOffset <= 5; yOffset++)
                {
                    if (SK.TerrainCheck.IsFrozenIce(loc + new IntVec3(xOffset, 0, yOffset).RotatedBy(rot), map) == false)
                    {
                        __result = new AcceptanceReport("FishIndustry.FishingPier_ZoneMustBeOnWater".Translate());
                        return false;
                    }
                }
            }
            Log.Message("Test AllowsPlacing_Patch 5");
            __result = true;
            return false;
        }

        public static bool IsNearIceFishingSpot(Map map, IntVec3 position, int distance)
        {
            List<Thing> fishingPiers = new List<Thing>();
            fishingPiers.AddRange(map.listerThings.ThingsOfDef(BuildingDefs.IceFishingSpot));
            fishingPiers.AddRange(map.listerThings.ThingsOfDef(BuildingDefs.IceFishingSpot.blueprintDef));
            fishingPiers.AddRange(map.listerThings.ThingsOfDef(BuildingDefs.IceFishingSpot.frameDef));
            //fishingPiers.AddRange(map.listerThings.ThingsOfDef(Util_FishIndustry.FishingPierSpawnerOnMudDef.blueprintDef));
            //fishingPiers.AddRange(map.listerThings.ThingsOfDef(Util_FishIndustry.FishingPierSpawnerOnMudDef.frameDef));
            foreach (Thing thing in fishingPiers)
            {
                if (thing.Position.InHorDistOf(position, distance))
                {
                    return true;
                }
            }
            return false;
        }
    }*/
    /*
    [HarmonyPatch(typeof(WorkGiver_FishingPier), "PotentialWorkThingRequest")]
    public class PotentialWorkThingRequest_Patch
    {
        internal static bool Postfix(WorkGiver_FishingPier __instance, ThingRequest __result)
        {

            //Log.Message("ThingRequest is: " + __result.ToString());
            if (!__result.IsUndefined)
                return true;
            Log.Message("If passed");
            __result = ThingRequest.ForDef(BuildingDefs.IceFishingSpot);

            return false;
        }
    }*/

    [HarmonyPatch(typeof(Building_FishingPier), "UpdateAquaticCellsAround")]
    public class UpdateAquaticCellsAround_Patch
    {
        internal static bool Prefix(Building_FishingPier __instance)
        {
            //Verse.Log.Message("UpdateAquaticCellsAround_Patch 2");
            __instance.aquaticCells.Clear();
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(__instance.Position, Building_FishingPier.aquaticAreaRadius, false))
            {
                // Same room cannot be checked for deep water.
                if (cell.InBounds(__instance.Map) == false)
                {
                    continue;
                }

                if (__instance.def.defName == "HSK_IceFishingSpot")
                {
                    if (WaterFreezesCompCache.GetFor(__instance.Map).WaterDepthGrid[__instance.Map.cellIndices.CellToIndex(cell)] >= 10)
                    {
                        __instance.aquaticCells.Add(cell);
                    }
                }
                else
                {
                    if (!SK.TerrainCheck.IsFrozenIce(cell, __instance.Map))
                    {
                        //Verse.Log.Message("UpdateAquaticCellsAround_Patch 3");
                        __instance.aquaticCells.Add(cell);
                    }
                }
            }

            return false;
        }
    }
    
    [HarmonyPatch(typeof(WorkGiver_FishingPier), "HasJobOnThing", new System.Type[] {typeof (Pawn), typeof (Thing), typeof(bool) })]
    public class HasJobOnThing_Patch
    {
        internal static void Postfix(ref bool __result, Pawn pawn, Thing t, bool forced = false)
        {
            //Verse.Log.Message("HasJobOnThing_Patch");
            if (t.def.defName == "HSK_IceFishingSpot")
                return;
            //Verse.Log.Message("HasJobOnThing_Patch 2");
            if ((t is Building_FishingPier) == false)
            {
                //Verse.Log.Message("HasJobOnThing_Patch 3");
                return;
            }
            Building_FishingPier fishingPier = t as Building_FishingPier;

            if (SK.TerrainCheck.IsFrozenIce(fishingPier.fishingSpotCell, fishingPier.Map))
            {
                //Verse.Log.Message("HasJobOnThing_Patch 4");
                __result = false;
                return;
            }
            return;
        }
    }
}
