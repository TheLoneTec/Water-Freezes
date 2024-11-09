// Decompiled with JetBrains decompiler
// Type: WF.MouseoverReadout_MouseoverReadoutOnGUI
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace WF
{
  [HarmonyPatch(typeof (MouseoverReadout), "MouseoverReadoutOnGUI")]
  public class MouseoverReadout_MouseoverReadoutOnGUI
  {
    internal static IEnumerable<CodeInstruction> Transpiler(
      IEnumerable<CodeInstruction> instructions)
    {
      MethodInfo labelMaker = AccessTools.Method(typeof (MouseoverReadout_MouseoverReadoutOnGUI), "MakeLabelIfRequired");
      FieldInfo BotLeft = AccessTools.Field(typeof (MouseoverReadout), "BotLeft");
      List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
      int num = 0;
      bool skip = true;
      for (int i = 0; i < codes.Count; ++i)
      {
        if (num == 7 & skip)
        {
          yield return new CodeInstruction(OpCodes.Ldloc_0).WithLabels((IEnumerable<Label>) codes[i].labels);
          yield return new CodeInstruction(OpCodes.Ldarg_0);
          yield return new CodeInstruction(OpCodes.Ldfld, (object) BotLeft);
          yield return new CodeInstruction(OpCodes.Ldloc_1);
          yield return new CodeInstruction(OpCodes.Call, (object) labelMaker);
          yield return new CodeInstruction(OpCodes.Stloc_1);
          skip = false;
          yield return codes[i].WithLabels();
        }
        else
        {
          yield return codes[i];
          if (codes[i].opcode == OpCodes.Stloc_1)
            ++num;
        }
      }
    }

    public static float MakeLabelIfRequired(IntVec3 cell, Vector2 BotLeft, float num)
    {
      MapComponent_WaterFreezes component = Find.CurrentMap.GetComponent<MapComponent_WaterFreezes>();
      if (component == null)
        return num;
      float num1 = num;
      int index = component.map.cellIndices.CellToIndex(cell);
      float num2 = component.IceDepthGrid[index];
      float num3 = component.WaterDepthGrid[index];
      bool flag = component.NaturalWaterTerrainGrid[index] != null;
      if ((double) num2 > 0.0)
      {
        Widgets.Label(new Rect(BotLeft.x, (float) UI.screenHeight - BotLeft.y - num1, 999f, 999f), "Ice depth " + Math.Round((double) num2, 4).ToString());
        num1 += 19f;
      }
      if ((double) num3 > 0.0)
      {
        Widgets.Label(new Rect(BotLeft.x, (float) UI.screenHeight - BotLeft.y - num1, 999f, 999f), "Water depth " + Math.Round((double) num3, 4).ToString());
        num1 += 19f;
      }
      if (flag)
      {
        Widgets.Label(new Rect(BotLeft.x, (float) UI.screenHeight - BotLeft.y - num1, 999f, 999f), "Natural Water");
        num1 += 19f;
      }
      return num1;
    }
  }
}
