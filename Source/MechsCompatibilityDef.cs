using System.Collections.Generic;
using Verse;

namespace Locks
{
  public class MechsCompatibilityDef : Def
  {
    public List<PawnKindDef> mechsFromMods;
  }

  public class MechCompatibleFleshTypeDef : Def
  {
    public List<string> fleshFromMods;
  }
}