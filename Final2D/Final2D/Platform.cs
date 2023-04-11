using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class Platform
    {
        Texture2D platText, hitBoxText;
        Rectangle platSource, platDisplay, hitBoxDisplay;
        Color platColor;
        public Platform(Texture2D platText, Rectangle platSource, Rectangle platDisplay, Color platColor,
            Texture2D hitBoxText, Rectangle hitBoxDisplay)
        {
            this.platText = platText;
            this.platSource = platSource;
            this.platDisplay = platDisplay;
            this.platColor = platColor;
            this.hitBoxText = hitBoxText;
            this.hitBoxDisplay = hitBoxDisplay;
        }

        public Texture2D PlatText { get => platText; set => platText = value; }
        public Rectangle PlatDisplay { get => platDisplay; }
        public Rectangle PlatSource { get => platSource; }
        public Color PlatColor { get => platColor; }
        public Texture2D HitBoxText { get => hitBoxText; }
        public Rectangle HitBoxDisplay { get => hitBoxDisplay; }
    }
}
