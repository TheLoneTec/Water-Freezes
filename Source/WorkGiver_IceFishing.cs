using RimWorld;
using SK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Verse.Noise;
using static System.Net.Mime.MediaTypeNames;

namespace WF
{
    internal class WorkGiver_IceFishing : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(BuildingDefs.IceFishingSpot);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if ((t is Building_FishingPier) == false)
            {
                return false;
            }
            Building_FishingPier fishingPier = t as Building_FishingPier;

            if (fishingPier.IsBurning()
                || (fishingPier.allowFishing == false))
            {
                return false;
            }

            if (TerrainCheck.IsFrozenIce(fishingPier.fishingSpotCell, fishingPier.Map) == false)
            {
                return false;
            }
            else if (WaterFreezesCompCache.GetFor(fishingPier.Map).WaterDepthGrid[fishingPier.Map.cellIndices.CellToIndex(t.Position)] < 10)
            {
                //t.Destroy();
                //Messages.Message("WaterFreezes_IceFishingSpotFrozen".Translate(), (LookTargets)new TargetInfo(t.Position, t.Map), MessageTypeDefOf.NeutralEvent);
                return false;
            }
            if (pawn.CanReserveAndReach(fishingPier, this.PathEndMode, Danger.Some) == false)
            {
                return false;
            }
            if (fishingPier.fishStock <= 0)
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = new Job();
            Building_FishingPier fishingPier = t as Building_FishingPier;

            if ((fishingPier.allowUsingCorn)
                && (HasFoodToAttractFishes(pawn) == false))
            {
                Predicate<Thing> predicate = delegate (Thing cornStack)
                {
                    return (cornStack.IsForbidden(pawn.Faction) == false)
                        && (cornStack.stackCount >= 4 * JobDriver_FishAtFishingPier.cornCountToAttractFishes);
                };
                TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false);
                // changed corn to bait. SK.
                Thing bait = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(Util_FishIndustry.FishingBaitDef), Verse.AI.PathEndMode.ClosestTouch, traverseParams, 9999f, predicate);
                if (bait != null)
                {
                    job = JobMaker.MakeJob(JobDefOf.TakeInventory, bait);
                    job.count = 4 * JobDriver_FishAtFishingPier.cornCountToAttractFishes;
                    return job;
                }
            }
            job = JobMaker.MakeJob(Util_FishIndustry.FishAtFishingPierJobDef, fishingPier, fishingPier.fishingSpotCell);

            return job;
        }

        public bool HasFoodToAttractFishes(Pawn fisher)
        {
            foreach (Thing thing in fisher.inventory.innerContainer)
            {
                // changed corn to bait
                if ((thing.def == Util_FishIndustry.FishingBaitDef)
                    && (thing.stackCount >= JobDriver_FishAtFishingPier.cornCountToAttractFishes))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
