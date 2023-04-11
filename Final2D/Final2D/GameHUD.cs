using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class GameHUD
    {
        Texture2D uiText;
        Rectangle uiDisplay;
        Color uiColor;
        int posX;
        public GameHUD(Texture2D uiText, Rectangle uiDisplay, Color uiColor)
        {
            this.uiText = uiText;
            this.uiDisplay = uiDisplay;
            this.uiColor = uiColor;
            posX = UIDisplay.X;
        }

        public Texture2D UIText { get => uiText; set => uiText = value; }
        public Rectangle UIDisplay { get => uiDisplay; }
        public Color UIColor { get => uiColor; }

        public virtual void Move(float cameraPosX)
        {
            uiDisplay.X = -(int)cameraPosX + posX;
        }
    }
}
