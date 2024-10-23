using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Locks
{
  public class FenceDoorsDef : Def
  {
    public HashSet<String> fenceGatesDefNames;

    public bool IsFenceGate(String defName)
    {
      return fenceGatesDefNames?.Contains(defName) ?? false;
    }
  }
}