// Decompiled with JetBrains decompiler
// Type: WF.IToggleablePatch
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

namespace WF
{
  public interface IToggleablePatch
  {
    string Name { get; }

    bool Enabled { get; }

    bool Applied { get; }

    void Apply();

    void Remove();
  }
}
