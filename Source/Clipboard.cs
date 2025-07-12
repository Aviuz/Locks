using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locks
{
    public static class Clipboard
    {
        public const int MaxItemCount = 10;

        private static List<LockState> clipboardStack = new List<LockState>();

        public static LockState? StoredState
        {
            get
            {
                if (clipboardStack.Count > 0)
                    return clipboardStack[0];
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                {
                    var copyState = LockState.DefaultConfiguration();
                    copyState.CopyFrom(value.Value);
                    clipboardStack.Insert(0, value.Value);
                }
                if (clipboardStack.Count > MaxItemCount)
                    clipboardStack.RemoveAt(MaxItemCount);
            }
        }
    }
}
