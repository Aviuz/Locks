using System.Collections.Generic;

namespace Locks
{
  public static class Clipboard
  {
    private const int MaxItemCount = 10;

    private static readonly List<LockState> ClipboardStack = new List<LockState>();

    public static LockState? StoredState
    {
      get
      {
        if (ClipboardStack.Count > 0)
          return ClipboardStack[0];
        return null;
      }
      set
      {
        if (value.HasValue)
        {
          var copyState = LockState.DefaultConfiguration();
          copyState.CopyFrom(value.Value);
          ClipboardStack.Insert(0, value.Value);
        }

        if (ClipboardStack.Count > MaxItemCount)
          ClipboardStack.RemoveAt(MaxItemCount);
      }
    }
  }
}