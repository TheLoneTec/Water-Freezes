// Decompiled with JetBrains decompiler
// Type: WF.WaterFreezesCompCache
// Assembly: UdderlyEvelyn.WaterFreezes, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 37B82161-8370-4565-B1CB-EC87BADC0A9E
// Assembly location: C:\Users\kacey\Downloads\Water-Freezes-main\Water-Freezes-main\1.3\Assemblies\UdderlyEvelyn.WaterFreezes.dll

using System.Collections.Generic;
using Verse;

namespace WF
{
  public static class WaterFreezesCompCache
  {
    public static Dictionary<int, MapComponent_WaterFreezes> compCachePerMap = new Dictionary<int, MapComponent_WaterFreezes>();

    public static MapComponent_WaterFreezes GetFor(Map map)
    {
      MapComponent_WaterFreezes componentWaterFreezes;
      if (!WaterFreezesCompCache.compCachePerMap.ContainsKey(map.uniqueID))
        WaterFreezesCompCache.compCachePerMap.Add(map.uniqueID, componentWaterFreezes = map.GetComponent<MapComponent_WaterFreezes>());
      else
        componentWaterFreezes = WaterFreezesCompCache.compCachePerMap[map.uniqueID];
      return componentWaterFreezes;
    }

    public static void SetFor(Map map, MapComponent_WaterFreezes comp)
    {
      if (!WaterFreezesCompCache.compCachePerMap.ContainsKey(map.uniqueID))
        WaterFreezesCompCache.compCachePerMap.Add(map.uniqueID, comp);
      else
        WaterFreezesCompCache.compCachePerMap[map.uniqueID] = comp;
    }
  }
}
