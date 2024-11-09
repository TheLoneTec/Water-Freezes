// Decompiled with JetBrains decompiler
// Type: WF.TerrainGrid_DoTerrainChangedEffects
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace WF
{
  [HarmonyPatch(typeof (TerrainGrid), "DoTerrainChangedEffects")]
  public class TerrainGrid_DoTerrainChangedEffects
  {
    public static MethodInfo ListThing_get_Item = typeof (List<Thing>).GetMethod("get_Item", new System.Type[1]
    {
      typeof (int)
    });

    internal static IEnumerable<CodeInstruction> Transpiler(
      IEnumerable<CodeInstruction> instructions)
    {
      foreach (CodeInstruction instruction in instructions)
      {
        if (instruction.opcode == OpCodes.Ldnull)
        {
          yield return new CodeInstruction(OpCodes.Ldloc_0);
          yield return new CodeInstruction(OpCodes.Ldloc_2);
          yield return new CodeInstruction(OpCodes.Callvirt, (object) TerrainGrid_DoTerrainChangedEffects.ListThing_get_Item);
        }
        else
          yield return instruction;
      }
    }
  }
}
