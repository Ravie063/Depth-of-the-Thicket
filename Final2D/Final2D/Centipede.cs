using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class Centipede : Enemy
    {
        public Centipede(Texture2D charText, Rectangle charSource, Rectangle charDisplay,
            Texture2D hitBoxText, Rectangle hitBoxDisplay, Color charColor, int sourceWidthLimit,
            Texture2D attackHitBoxText, Rectangle attackHitBoxDisplay, int minPosX, int maxPosX,
            Texture2D rayCastHitBoxText, Rectangle rayCastHitBoxDisplay, int charLife, int chaseDistance)
            : base(charText, charSource, charDisplay, hitBoxText, hitBoxDisplay, charColor, sourceWidthLimit,
             attackHitBoxText, attackHitBoxDisplay, minPosX, maxPosX, rayCastHitBoxText, rayCastHitBoxDisplay, charLife, chaseDistance)
        {
            animSpeed = 12;
            initialAnimSpeed = animSpeed;

            charMoveSpeed = 1;
            charChaseSpeed = charMoveSpeed * 4;

            hitBoxPosX = 60;
            //rayHitBoxPosX = 40;
        }
        protected override void ChangeAnimationState(int spriteRow)
        {
            //ATTACK
            if (spriteRow == 2 || spriteRow == 7)
            {
                sourceWidthLimit = charText.Width - charSource.Width;
            }
            //HIT
            else if (spriteRow == 4 || spriteRow == 9)
            {
                sourceWidthLimit = charText.Width - charSource.Width * 5;
            }
            //REST THAT HAVE 4 FRAMES ANIMATION
            else
            {
                sourceWidthLimit = charText.Width - charSource.Width * 3;
            }
            charSource.Y = charSource.Height * spriteRow;
        }
    }
}
