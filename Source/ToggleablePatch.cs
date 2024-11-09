// Decompiled with JetBrains decompiler
// Type: WF.ToggleablePatch
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace WF
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class ToggleablePatch : Attribute
  {
    public static bool AutoScan = true;
    protected static bool _performedPatchScan = false;
    public static Action<string> MessageLoggingMethod = new Action<string>(Log.Message);
    public static Action<string> WarningLoggingMethod = new Action<string>(Log.Warning);
    public static Action<string> ErrorLoggingMethod = new Action<string>(Log.Error);
    public static List<IToggleablePatch> Patches = new List<IToggleablePatch>();

    public static void ScanForPatches()
    {
      if (ToggleablePatch._performedPatchScan)
        return;
      IEnumerable<MemberInfo> memberInfos = ((IEnumerable<System.Type>) Assembly.GetExecutingAssembly().GetTypes()).SelectMany<System.Type, MemberInfo>((Func<System.Type, IEnumerable<MemberInfo>>) (type => (IEnumerable<MemberInfo>) type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
      System.Type type1 = typeof (IToggleablePatch);
      foreach (MemberInfo memberInfo in memberInfos)
      {
        if (memberInfo.HasAttribute<ToggleablePatch>())
        {
          if (memberInfo is FieldInfo fieldInfo)
          {
            if (((IEnumerable<System.Type>) fieldInfo.FieldType.GetInterfaces()).Contains<System.Type>(type1))
              ToggleablePatch.Patches.Add((IToggleablePatch) fieldInfo.GetValue((object) null));
            else
              ToggleablePatch.ErrorLoggingMethod("[ToggleablePatch] Field \"" + fieldInfo.Name + "\" is marked with ToggleablePatch attribute but does not implement IToggleablePatch.");
          }
          else if (memberInfo is PropertyInfo propertyInfo)
          {
            if (((IEnumerable<System.Type>) propertyInfo.PropertyType.GetInterfaces()).Contains<System.Type>(type1))
              ToggleablePatch.Patches.Add((IToggleablePatch) propertyInfo.GetValue((object) null));
            else
              ToggleablePatch.ErrorLoggingMethod("[ToggleablePatch] Property \"" + propertyInfo.Name + "\" is marked with ToggleablePatch attribute but does not implement IToggleablePatch.");
          }
        }
      }
      ToggleablePatch._performedPatchScan = true;
    }

    public static void AddPatches(params IToggleablePatch[] patches) => ToggleablePatch.Patches.AddRange((IEnumerable<IToggleablePatch>) patches);

    public static void ProcessPatches(string modID, string reason = null)
    {
      if (ToggleablePatch.AutoScan)
        ToggleablePatch.ScanForPatches();
      ToggleablePatch.MessageLoggingMethod("[ToggleablePatch] Processing " + ToggleablePatch.Patches.Count.ToString() + " patches" + (reason != null ? " because " + reason : "") + " for \"" + modID + "\"..");
      foreach (IToggleablePatch patch in ToggleablePatch.Patches)
        patch.Process();
    }
  }
}
