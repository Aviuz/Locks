using UnityEngine;

namespace Locks
{
    public static class RectUtility
    {
        public static Rect Indent(this Rect rect, float indent)
        {
            return new Rect(rect.x + indent, rect.y, rect.width - indent, rect.height);
        }

        public static Rect Row(this Rect rect, float row)
        {
            return new Rect(rect.x, rect.y + row, rect.width, rect.height);
        }
    }
}