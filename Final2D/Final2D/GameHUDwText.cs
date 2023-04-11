using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class GameHUDwText : GameHUD
    {
        Vector2 fontPos;
        Color fontColor;
        int fontPosX;
        public GameHUDwText(Texture2D uiText, Rectangle uiDisplay, Color uiColor, Vector2 fontPos, Color fontColor) 
            : base(uiText, uiDisplay, uiColor)
        {
            this.fontPos = fontPos;
            this.fontColor = fontColor;
            fontPosX = (int)FontPos.X;
        }
        public Vector2 FontPos { get => fontPos; }
        public Color FontColor { get => fontColor; }
        public override void Move(float cameraPosX)
        {
            base.Move(cameraPosX);
            fontPos.X = -(int)cameraPosX + fontPosX; 
        }
    }
}
