// Decompiled with JetBrains decompiler
// Type: WF.WaterFreezesMod
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using UnityEngine;
using Verse;

namespace WF
{
  public class WaterFreezesMod : Mod
  {
    public WaterFreezesSettings Settings;
    private string _iceRateBuffer;
    private string _freezingMultiplierBuffer;
    private string _thawingDivisorBuffer;

    public WaterFreezesMod(ModContentPack content)
      : base(content)
    {
      this.Settings = this.GetSettings<WaterFreezesSettings>();
    }

    public override string SettingsCategory() => "Water Freezes";

    public override void DoSettingsWindowContents(Rect inRect)
    {
      Listing_Standard listingStandard = new Listing_Standard();
      listingStandard.Begin(inRect);
      listingStandard.Label("Ticks Per Simulation Update");
      listingStandard.Label("The higher this is the less frequently it will update, the less realistic it will be, and the less it will impact TPS.");
      listingStandard.Label("500 minimum, it's indistinguishable from updating every tick quality-wise but way better performance.");
      listingStandard.Label("Anything higher than 2500 (1 in-game hour) has not been tested. Default is 1000 for a good balance.");
      listingStandard.Label("Versions before this setting was introduced had it set to 2500.");
      listingStandard.TextFieldNumeric<int>(ref WaterFreezesSettings.IceRate, ref this._iceRateBuffer, 500f, 2500f);
      listingStandard.Label("Take care when modifying the multiplier and divisor, relatively small changes will produce big effects!");
      listingStandard.Label("The temperature is multiplied by this value before going into other calculations to produce the amount of freezing that occurs below freezing.");
      listingStandard.TextFieldNumericLabeled<float>("Freezing Factor", ref WaterFreezesSettings.FreezingFactor, ref this._freezingMultiplierBuffer, 1f);
      listingStandard.Label("The (negated) temperature is divided by this value before going into other calculations to produce the amount of thawing that occurs above freezing.");
      listingStandard.TextFieldNumericLabeled<float>("Thawing Factor", ref WaterFreezesSettings.ThawingFactor, ref this._thawingDivisorBuffer, 1f);
      listingStandard.CheckboxLabeled("Moisture Pump Clears Natural Water", ref WaterFreezesSettings.MoisturePumpClearsNaturalWater, "If enabled, using a moisture pump will prevent water you pumped from refilling. I recommend using Soil Relocation Framework's water filling instead!");
      listingStandard.End();
      base.DoSettingsWindowContents(inRect);
    }
  }
}
