using RimWorld;
using SK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WF
{
    internal class PlaceWorker_IceFishingSpot : PlaceWorker
    {
        public static int lastMoteUpdateSecond = 0;
        public static int lastMoteUpdateTick = 0;
        public static IntVec3 lastMotePosition = IntVec3.Invalid;
        public static float waterVolume;

        public override AcceptanceReport AllowsPlacing(
            BuildableDef checkingDef,
            IntVec3 loc,
            Rot4 rot,
            Map map,
            Thing thingToIgnore = null,
            Thing thing = null)
        {
            // Remove old fish stock respawn rate text mote.
            if (lastMotePosition.IsValid
                && (loc != lastMotePosition))
            {
                RemoveLastRespawnRateMote(map);
            }

            if (Util_FishIndustry.GetFishSpeciesList(map.Biome).NullOrEmpty())
            {
                return new AcceptanceReport("FishIndustry.FishingPier_InvalidBiome".Translate());
            }
            //Log.Message("Test AllowsPlacing_Patch 2");
            if (IsNearIceFishingSpot(map, loc, Util_PlaceWorker.minDistanceBetweenTwoFishingSpots))
            {
                return new AcceptanceReport("FishIndustry.TooCloseFishingPier".Translate());
            }
            //Log.Message("Test AllowsPlacing_Patch 3");
            // Check if a fishing zone is not too close.
            /*
            if (Util_PlaceWorker.IsNearFishingZone(map, loc, Util_PlaceWorker.minDistanceBetweenTwoFishingSpots))
            {
                return new AcceptanceReport("FishIndustry.TooCloseFishingZone".Translate());
            }
            */
            if (!SK.TerrainCheck.IsFrozenIce(loc, map))
            {
                return new AcceptanceReport("FishIndustry.FishingPier_PierMustBeOnWater".Translate());
            }
            else if (WaterFreezesCompCache.GetFor(map).WaterDepthGrid[map.cellIndices.CellToIndex(loc)] < 10)
            {
                return new AcceptanceReport("FishIndustry.FishingPier_TooFewWater".Translate());
            }
            //Log.Message("Test AllowsPlacing_Patch 4");
            // Check fishing zone is on water.
            //float waterVolume = 0;
            /*
            for (int xOffset = -2; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset <= 2; yOffset++)
                {
                    if (SK.TerrainCheck.IsFrozenIce(loc + new IntVec3(xOffset, 0, yOffset).RotatedBy(rot), map) == false)
                    {
                        return new AcceptanceReport("FishIndustry.FishingPier_ZoneMustBeOnWater".Translate() + " Test 4");
                    }
                    //GetWaterVolumeCellsInRadius(map, loc + new IntVec3(xOffset, 0, yOffset).RotatedBy(rot), Building_FishingPier.aquaticAreaRadius, out waterVolume);
                }
            }*/
            //Log.Message("Test AllowsPlacing_Patch 5");

            //if (waterVolume < 250)
            //return new AcceptanceReport("FishIndustry.FishingPier_IceTooThick".Translate());

            // Display fish stock respawn rate.
            if ((lastMotePosition.IsValid == false)
                || (Find.TickManager.Paused
                    && (DateTime.Now.Second != lastMoteUpdateSecond))
                || ((Find.TickManager.Paused == false)
                    && (Find.TickManager.TicksGame >= lastMoteUpdateTick + GenTicks.TicksPerRealSecond)))
            {
                lastMoteUpdateSecond = DateTime.Now.Second;
                lastMoteUpdateTick = Find.TickManager.TicksGame;
                RemoveLastRespawnRateMote(map);
                DisplayMaxFishStockMoteAt(map, loc, rot, checkingDef, out waterVolume);

                //Log.Message("Result Water Volume is: " + waterVolume);
            }
            if (waterVolume < 250)
                return new AcceptanceReport("FishIndustry.FishingPier_TooFewWater".Translate());

            return true;
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

        public void RemoveLastRespawnRateMote(Map map)
        {
            if (lastMotePosition.IsValid
                && lastMotePosition.InBounds(map))
            {
                foreach (Thing thing in lastMotePosition.GetThingList(map))
                {
                    if (thing is MoteText)
                    {
                        thing.Destroy();
                        break;
                    }
                }
            }
            lastMotePosition = IntVec3.Invalid;
        }

        public void DisplayMaxFishStockMoteAt(Map map, IntVec3 position, Rot4 rotation, BuildableDef checkingDef, out float waterVolume)
        {
            waterVolume = 0;
            if (position.InBounds(map) == false)
            {
                return;
            }
            int aquaticCellsAround = MapComponent_WaterFreezes.GetWaterVolumeCellsInRadius(map, position + checkingDef.blueprintDef.interactionCellOffset.RotatedBy(rotation), Building_FishingPier.aquaticAreaRadius,out waterVolume);
            int aquaticCellsProportionInPercent = Mathf.RoundToInt(((float)aquaticCellsAround / (float)(GenRadial.NumCellsInRadius(Building_FishingPier.aquaticAreaRadius) - 3)) * 100f); // 3 cells will actually be occupied by the pier.
            //Log.Message("water Volume is: " + waterVolume);
            Color textColor = Color.red;
            string aquaticCellsProportionAsText = "";
            if (aquaticCellsAround < Util_Zone_Fishing.minCellsToSpawnFish || waterVolume < 250)
            {
                //if (water < 10000)
                    //aquaticCellsProportionAsText = "FishIndustry.FishingPier_IceTooThick".Translate();
                //else
                    aquaticCellsProportionAsText = "FishIndustry.FishingPier_TooFewWater".Translate();
            }
            else
            {
                aquaticCellsProportionAsText = aquaticCellsProportionInPercent + "%";
                if (aquaticCellsProportionInPercent >= 75)
                {
                    textColor = Color.green;
                }
                else if (aquaticCellsProportionInPercent >= 50)
                {
                    textColor = Color.yellow;
                }
                else if (aquaticCellsProportionInPercent >= 25)
                {
                    textColor = new Color(255, 165, 0); // Orange color.
                }
            }

            MoteText moteText = (MoteText)ThingMaker.MakeThing(ThingDefOf.Mote_Text, null);
            moteText.exactPosition = position.ToVector3Shifted();
            moteText.text = aquaticCellsProportionAsText;
            moteText.textColor = textColor;
            moteText.overrideTimeBeforeStartFadeout = 1f;
            GenSpawn.Spawn(moteText, position, map);
            lastMotePosition = position;
        }

    }
}
