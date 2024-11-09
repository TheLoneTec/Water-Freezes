// Decompiled with JetBrains decompiler
// Type: WF.TerrainExtension_WaterStats
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using Verse;

namespace WF
{
  public class TerrainExtension_WaterStats : DefModExtension
  {
    public float MaxWaterDepth;
    public float MaxIceDepth;
    public float FreezingPoint;
    public bool IsMoving;
    public TerrainDef ThinIceDef;
    public TerrainDef IceDef;
    public TerrainDef ThickIceDef;
  }
}
