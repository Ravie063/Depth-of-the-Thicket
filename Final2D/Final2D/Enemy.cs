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
    class Enemy
    {
        public enum AnimState { Attacking, Patrolling, Chasing, Death, Hit, Idle }
        public AnimState animState;

        protected Texture2D charText, hitBoxText, attackHitBoxText, rayCastHitBoxText;
        protected Rectangle charSource, charDisplay, hitBoxDisplay, attackHitBoxDisplay, rayCastHitBoxDisplay;
        protected Color charColor;
        protected int sourceWidthLimit;
        protected int spriteRow;

        protected int hitBoxPosX, rayHitBoxPosX;
        protected int charMoveSpeed, charChaseSpeed;
        protected int delay, attackDelay, animSpeed, initialAnimSpeed;
        protected float distanceToPlayer;
        protected int chaseDistance;
        protected int charLife;
        protected bool facingLeftRight, isPlayerInFront, isRay, isHit, isDead;
        protected int minPosX, maxPosX;

        public Enemy(Texture2D charText, Rectangle charSource, Rectangle charDisplay,
            Texture2D hitBoxText, Rectangle hitBoxDisplay, Color charColor, int sourceWidthLimit,
            Texture2D attackHitBoxText, Rectangle attackHitBoxDisplay, int minPosX, int maxPosX,
            Texture2D rayCastHitBoxText, Rectangle rayCastHitBoxDisplay, int charLife, int chaseDistance)
        {
            animState = AnimState.Patrolling;

            this.charText = charText;
            this.charSource = charSource;
            this.charDisplay = charDisplay;
            this.hitBoxText = hitBoxText;
            this.hitBoxDisplay = hitBoxDisplay;
            this.charColor = charColor;
            this.sourceWidthLimit = sourceWidthLimit;
            this.attackHitBoxText = attackHitBoxText;
            this.attackHitBoxDisplay = attackHitBoxDisplay;
            this.minPosX = minPosX;
            this.maxPosX = maxPosX;
            this.rayCastHitBoxText = rayCastHitBoxText;
            this.rayCastHitBoxDisplay = rayCastHitBoxDisplay;
            this.charLife = charLife;
            this.chaseDistance = chaseDistance;
            spriteRow = 1;
            animSpeed = 24;
            initialAnimSpeed = animSpeed;
            charMoveSpeed = 1;
            charChaseSpeed = charMoveSpeed * 3;

            delay = 0;
            attackDelay = 0;
            hitBoxPosX = 60;
            //rayHitBoxPosX = 40;
            facingLeftRight = true;
        }

        public Texture2D CharText { get => charText; }
        public Rectangle CharSource { get => charSource; }
        public Rectangle CharDisplay { get => charDisplay; }
        public Texture2D HitBoxText { get => hitBoxText; }
        public Rectangle HitBoxDisplay { get => hitBoxDisplay; }
        public Color CharColor { get => charColor; }
        public Texture2D AttackHitBoxText { get => attackHitBoxText; }
        public Rectangle AttackHitBoxDisplay { get => attackHitBoxDisplay; }
        public Texture2D RayCastHitBoxText { get => rayCastHitBoxText; }
        public Rectangle RayCastHitBoxDisplay { get => rayCastHitBoxDisplay; }
        public int CharLife { set => charLife = value; }

        public void EnemyUpdate(Player player, SoundEffectInstance sfxAttack, SoundEffectInstance sfxDeath)
        {
            if (animState != AnimState.Death)
            {
                Vector2 movePos = new Vector2(charDisplay.X, charDisplay.Y);
                Vector2 playerVec = new Vector2(player.PlayerDisplay.X, player.PlayerDisplay.Y);
                distanceToPlayer = Vector2.Distance(movePos, playerVec);
                if (animState != AnimState.Idle && animState != AnimState.Attacking && animState != AnimState.Hit)
                {
                    if ((distanceToPlayer < chaseDistance) && (charDisplay.X >= minPosX -50 && charDisplay.X <= maxPosX +50))
                    {
                        animState = AnimState.Chasing;
                    }
                    else
                    {
                        animState = AnimState.Patrolling;
                    }
                }
                switch (animState)
                {
                    case AnimState.Patrolling:
                        Patrol();
                        break;
                    case AnimState.Chasing:
                        Chase(player);
                        break;
                    case AnimState.Idle:
                        isHit = false;
                        IdleState(player);
                        break;
                    case AnimState.Attacking:
                        sfxAttack.Play();
                        Attack();
                        isPlayerInFront = false;
                        break;
                    case AnimState.Hit:
                        if (!isHit && animState != AnimState.Death)
                        {
                            isHit = true;
                            EnemyHit();
                        }
                        break;
                }
            }
            else if (!isDead)
            {
                isDead = !isDead;
                sfxDeath.Play();
                Game1.currentEnemyCount--;
                EnemyDeath();
            }
            AnimationDelay(animSpeed);
            ChangeAnimationState(spriteRow);
            hitBoxDisplay.Location = new Point(charDisplay.X + hitBoxPosX,
                   charDisplay.Y + (charDisplay.Height / 2));
            rayCastHitBoxDisplay.Location = new Point(charDisplay.X + rayHitBoxPosX,
                   charDisplay.Y + 75);
        }
        protected virtual void ChangeAnimationState(int spriteRow)
        {
            //WALK
            if (spriteRow == 0 || spriteRow == 5)
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
        void AnimationDelay(int animSpeed)
        {
            if (delay >= animSpeed)
            {
                CharAnimation();
                delay = 0;
            }
            delay++;
        }
        void CharAnimation()
        {
            if (charSource.X < sourceWidthLimit)
            {
                charSource.X += charSource.Width;
            }
            else if (animState != AnimState.Death)
            {
                if (animState == AnimState.Attacking || animState == AnimState.Hit)
                {
                    animState = AnimState.Idle;
                }
                charSource.X = 0;
            }
        }
        void Patrol()
        {
            //Vector2 movePos = new Vector2(charDisplay.X, charDisplay.Y);
            //movePos = Vector2.Clamp(movePos, new Vector2(200, charDisplay.Y), new Vector2(400, charDisplay.Y));
  
            animSpeed = initialAnimSpeed;
            //TURN LEFT
            if (charDisplay.X >= maxPosX)
            {
                facingLeftRight = true;
            }
            //TURN RIGHT
            else if (charDisplay.X <= minPosX)
            {
                facingLeftRight = false;
            }
            if (facingLeftRight)
            {
                charDisplay.X -= charMoveSpeed;
                spriteRow = 0;
                hitBoxPosX = 60;
                rayHitBoxPosX = -220;
            }
            else
            {
                charDisplay.X += charMoveSpeed;
                spriteRow = 5;
                hitBoxPosX = 40;
                rayHitBoxPosX = 85;
            }
        }
        void Chase(Player player)
        {
            if (distanceToPlayer <= 60 && distanceToPlayer >= 55 && RayCastHitBoxDisplay.Intersects(player.PlayerDisplay))
            {
                isPlayerInFront = true;
            }
            else
            {
                isPlayerInFront = false;
            }

            if (!isPlayerInFront)
            {
                if (!(RayCastHitBoxDisplay.Intersects(player.PlayerDisplay)) && isRay != true)
                {
                    isRay = true;
                    facingLeftRight = !facingLeftRight;
                }
                if (facingLeftRight)
                {
                    spriteRow = 0;
                    hitBoxPosX = 60;
                    charDisplay.X -= charMoveSpeed * charChaseSpeed;
                    rayHitBoxPosX = -220;
                }
                else
                {
                    spriteRow = 5;
                    hitBoxPosX = 40;
                    charDisplay.X += charMoveSpeed * charChaseSpeed;
                    rayHitBoxPosX = 85;
                }
            }
            else
            {
                animState = AnimState.Idle;
                charSource.X = 0;
                isRay = false;
            }
        }
        void Attack()
        {
            animSpeed = 7;
            if (facingLeftRight)
            {
                attackHitBoxDisplay.Location = new Point(charDisplay.X,
                    charDisplay.Y + (charDisplay.Height / 2));
                spriteRow = 2;
            }
            else
            {
                attackHitBoxDisplay.Location = new Point(charDisplay.X + 100,
                    charDisplay.Y + (charDisplay.Height / 2));
                spriteRow = 7;
            }
        }
        void IdleState(Player player)
        {
            //FACING RIGHT IDLE
            if (facingLeftRight)
            {
                spriteRow = 1;
            }
            //FACING LEFT IDLE
            else { spriteRow = 6; }
            if (player.animState != Player.AnimState.Death)
            {
                if (distanceToPlayer >= 60)
                {
                    isPlayerInFront = false;
                }
                if (attackDelay >= 5)
                {
                    animState = AnimState.Attacking;
                    attackDelay = 0;

                    if (attackHitBoxDisplay.Intersects(player.PlayerHitBoxDisplay))
                    {
                        player.animState = Player.AnimState.Hit;
                    }
                }
                attackDelay++;
            }
            else { isPlayerInFront = false; }
            if (!isPlayerInFront)
            {
                animState = AnimState.Patrolling;
                attackDelay = 0;
            }
        }
        void EnemyHit()
        {
            if (animState != AnimState.Death)
            {
                animSpeed = 5;
                charSource.X = 0;
                //FACING RIGHT IDLE
                if (facingLeftRight)
                {
                    charDisplay.X += 40;
                    spriteRow = 4;
                }
                //FACING LEFT IDLE
                else
                {
                    spriteRow = 9;
                    charDisplay.X -= 40;
                }
                charLife--;

                if (charLife <= 0)
                {
                    //Console.WriteLine(charLife + " HEALTH");
                    animState = AnimState.Death;
                }
            }
        }
        void EnemyDeath()
        {
            animSpeed = 12;
            charSource.X = 0;
            EnemyDeathStateAnim();
        }
        void EnemyDeathStateAnim()
        {
            //FACING RIGHT IDLE
            if (facingLeftRight)
            {
                spriteRow = 3;
            }
            //FACING LEFT IDLE
            else { spriteRow = 8; }
        }
        public void EnemyDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CharText, CharDisplay, CharSource, CharColor);
            if (animState != AnimState.Death)
            {
                spriteBatch.Draw(HitBoxText, HitBoxDisplay, Color.Transparent);
                spriteBatch.Draw(RayCastHitBoxText, RayCastHitBoxDisplay, Color.Transparent);
                if (animState == AnimState.Attacking)
                {
                    spriteBatch.Draw(AttackHitBoxText, AttackHitBoxDisplay, Color.Transparent);
                }
            }
        }
    }
}
