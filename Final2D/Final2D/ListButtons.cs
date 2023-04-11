using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class ListButtons
    {
        Texture2D normalText, hoverText;
        Texture2D buttonText;
        Rectangle buttonRect, initialPos;
        Color buttonColor;
        Vector2 fontPosUnhover, fontPosHover;
        Vector2 fontPos;
        Color fontColor;
        string[] fontLabels = { "       START", "   CONTINUE", "   CONTROLS", "        EXIT", "        BACK",
            " MAIN MENU", "         QUIT" };
        string fontText;
        int fontPosX, buttonRectPosX;
        public ListButtons(Texture2D normalText, Texture2D hoverText, Rectangle buttonRect, Color buttonColor,
            Vector2 fontPosUnhover, Vector2 fontPosHover, Color fontColor, int i)
        {
            this.normalText = normalText;
            this.hoverText = hoverText;
            buttonText = normalText;
            this.buttonRect = buttonRect;
            this.buttonColor = buttonColor;
            this.fontPosUnhover = fontPosUnhover;
            this.fontPosHover = fontPosHover;
            fontPos = fontPosUnhover;
            this.fontColor = fontColor;
            fontText = fontLabels[i];

            fontPosX = (int)FontPos.X;
            buttonRectPosX = ButtonRect.X;
            initialPos = buttonRect;
        }
        public Texture2D ButtonText { get => buttonText; }
        public Rectangle ButtonRect { get => buttonRect; }
        public Color ButtonColor { get => buttonColor; }
        public string FontText { get => fontText; }
        public Vector2 FontPos { get => fontPos; }
        public Color FontColor { get => fontColor; }
        public Rectangle InitialPos { get => initialPos; }
        public void HoverButton()
        {
            buttonText = hoverText;
            fontPos.Y = fontPosHover.Y;
        }

        public void NormalButton()
        {
            buttonText = normalText;
            fontPos.Y = fontPosUnhover.Y;
        }
        public void Move(float cameraPosX)
        {
            fontPos.X = -(int)cameraPosX + fontPosX; 
            buttonRect.X = -(int)cameraPosX + buttonRectPosX;
        }
    }
}
