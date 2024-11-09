// Decompiled with JetBrains decompiler
// Type: WF.Harmony_Patches.SnowGrid_SetDepth
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace WF.Harmony_Patches
{
  internal class SnowGrid_SetDepth
  {
    internal static IEnumerable<CodeInstruction> Transpiler(
      IEnumerable<CodeInstruction> instructions)
    {
      List<CodeInstruction> instructionList = instructions.ToList<CodeInstruction>();
      for (int i = 0; i < instructionList.Count; ++i)
      {
        CodeInstruction instruction = instructionList[i];
        yield return instruction;
        if (instruction.opcode == OpCodes.Stloc_0)
        {
          yield return new CodeInstruction(OpCodes.Ldarg_2);
          yield return new CodeInstruction(OpCodes.Ldc_R4);
          yield return new CodeInstruction(OpCodes.Ceq);
          yield return new CodeInstruction(OpCodes.Brfalse_S, instructionList[i + 4].operand);
        }
        instruction = (CodeInstruction) null;
      }
    }
  }
}
