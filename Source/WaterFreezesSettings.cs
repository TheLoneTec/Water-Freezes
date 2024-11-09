// Decompiled with JetBrains decompiler
// Type: WF.WaterFreezesSettings
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using Verse;

namespace WF
{
  public class WaterFreezesSettings : ModSettings
  {
    public static int IceRate = 1000;
    public static float FreezingFactor = 4f;
    public static float ThawingFactor = 2f;
    public static bool MoisturePumpClearsNaturalWater = false;

    public override void ExposeData()
    {
      Scribe_Values.Look<int>(ref WaterFreezesSettings.IceRate, "IceRate", 1000);
      Scribe_Values.Look<float>(ref WaterFreezesSettings.FreezingFactor, "FreezingFactor", 4f);
      Scribe_Values.Look<float>(ref WaterFreezesSettings.ThawingFactor, "ThawingFactor", 2f);
      Scribe_Values.Look<bool>(ref WaterFreezesSettings.MoisturePumpClearsNaturalWater, "MoisturePumpClearsNaturalWater");
      base.ExposeData();
    }
  }
}
