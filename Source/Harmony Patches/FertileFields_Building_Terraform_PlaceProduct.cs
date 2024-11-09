// Decompiled with JetBrains decompiler
// Type: WF.TerrainGrid_SetTerrain
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using FertileFields;
using HarmonyLib;
using RimWorld;
using SK.Enlighten;
using System.Security.Cryptography;
using Verse;
using Verse.Noise;

namespace WF
{
  [HarmonyPatch(typeof (Building_Terraform), "PlaceProduct")]
  public class Building_Terraform_PlaceProduct
    {
    internal static bool Prefix(Building_Terraform __instance)
    {
            var terrain = __instance.def.GetModExtension<Terrain>();

            if (terrain == null)
            {
                Log.Error("FertileFields :: " + __instance + " lacks " + typeof(Terrain) + " ModExtension");
                return false;
            }

            if (terrain.result != null)
            {
                int index = __instance.Map.cellIndices.CellToIndex(__instance.Position);
                __instance.Map.terrainGrid.SetTerrain(__instance.Position, terrain.result);
                MapComponent_WaterFreezes componentWaterFreezes = WaterFreezesCompCache.GetFor(__instance.Map);
                if (terrain.result.IsWater)
                {
                    componentWaterFreezes.AllWaterTerrainGrid[index] = terrain.result;
                    componentWaterFreezes.NaturalWaterTerrainGrid[index] = terrain.result;
                    TerrainExtension_WaterStats extension = WaterFreezesStatCache.GetExtension(terrain.result);
                    componentWaterFreezes.WaterDepthGrid[index] = extension.MaxWaterDepth;
                }
                else
                {
                    componentWaterFreezes.AllWaterTerrainGrid[index] = null;
                    componentWaterFreezes.NaturalWaterTerrainGrid[index] = null;
                    componentWaterFreezes.WaterDepthGrid[index] = 0.0f;
                }

            }
            else if (terrain.resultSpecial == SpecialCategory.Natural)
            {
                var rock = RockAt(__instance.Map, __instance.Position).building.naturalTerrain;

                __instance.Map.terrainGrid.SetTerrain(__instance.Position, rock);
            }

            if (terrain.products != null)
            {
                Thing thing;

                foreach (var product in terrain.products)
                {
                    thing = ThingMaker.MakeThing(product.thingDef, null);
                    thing.stackCount = product.count;

                    GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Near, null);
                }
            }
            else if (terrain.productSpecial == SpecialCategory.Natural)
            {
                var rock = RockAt(__instance.Map, __instance.Position).building.mineableThing;

                var thing = ThingMaker.MakeThing(rock, null);

                GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Near, null);
            }
            return false;
        }

    static ThingDef RockAt(Map map, IntVec3 pos)
    {
        RockNoises.Init(map);
        var thing = GenStep_RocksFromGrid.RockDefAt(pos);
        RockNoises.Reset();

        return thing;
    }

    }
}
