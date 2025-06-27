using UnityEngine;
using Verse;

namespace Locks
{
    [StaticConstructorOnStartup]
    public class CustomRow
    {
        private readonly Rect initRect;
        private static readonly Texture2D RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
        private static readonly Texture2D RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
        private const float Spacing = 4f;
        private float actualX;

        public CustomRow(Rect initRect)
        {
            this.initRect = initRect;
            actualX = initRect.x;
        }

        public bool ButtonInvisible()
        {
            return Widgets.ButtonInvisible(initRect);
        }

        public bool DrawButtonInvisible(ref bool checkout)
        {
            var clicked = Widgets.ButtonInvisible(initRect);
            if (clicked)
            {
                checkout = !checkout;
            }
            return clicked;
        }

        public void DrawRadioBox(bool checkedOn)
        {
            var checkTexture = checkedOn ? RadioButOnTex : RadioButOffTex;
            var rect = new Rect(actualX, initRect.y + (initRect.height / 2 - Widgets.RadioButtonSize / 2),
                Widgets.RadioButtonSize,
                Widgets.RadioButtonSize);
            Widgets.DrawTextureFitted(rect, checkTexture, 1);
            actualX += Widgets.RadioButtonSize + Spacing;
        }

        public void DrawTexture(Texture texture, Vector2 textureSize)
        {
            Widgets.DrawTextureFitted(
                new Rect(actualX, initRect.y, textureSize.x, textureSize.y),
                texture, 1f);
            actualX += textureSize.x + Spacing;
        }

        public void DrawLabel(string text)
        {
            var rect = new Rect(actualX, initRect.y,  initRect.width - actualX, initRect.height);
            Widgets.Label(rect, text);
            actualX += rect.width + Spacing;
        }
    }
}