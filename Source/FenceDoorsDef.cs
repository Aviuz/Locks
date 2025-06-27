using System;
using System.Collections.Generic;
using Verse;

namespace Locks
{
  public class FenceDoorsDef : Def
  {
    public List<String> fenceGatesDefNames;

    public bool IsFenceGate(String defName)
    {
      return fenceGatesDefNames?.Contains(defName) ?? false;
    }
  }
}