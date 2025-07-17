using System.Collections.Generic;
using Verse;

namespace Locks
{
  public class FenceDoorsDef : Def
  {
    public List<string> fenceGatesDefNames;

    public bool IsFenceGate(string defName)
    {
      return fenceGatesDefNames?.Contains(defName) ?? false;
    }
  }
}