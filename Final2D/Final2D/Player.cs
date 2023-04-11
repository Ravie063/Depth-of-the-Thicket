using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class Player
    {
        public enum AnimState { Attacking, Moving, Idle, Hit, Death }
        public AnimState animState;
        public enum JumpState { Jumped, Falling, Ground}
        public JumpState jumpState;

        Texture2D playerText, playerHitBoxText, attackHitBoxText;
        Rectangle playerSource, playerDisplay, playerHitBoxDisplay, attackHitBoxDisplay;
        Color playerColor;
        int sourceWidthLimit;
        int spriteRow;
        Point checkPoint;
        int playerLife;

        int playerMoveSpeed, playerSprintSpeed;
        int delay, animSpeed;
        bool facingLeftRight, isHit;

        int gravity, force;

        public Player(Texture2D playerText, Rectangle playerSource, Rectangle playerDisplay,
            Texture2D playerHitBoxText, Rectangle playerHitBoxDisplay, Color playerColor, int sourceWidthLimit,
            Texture2D attackHitBoxText, Rectangle attackHitBoxDisplay)
        {
            animState = AnimState.Idle;

            this.playerText = playerText;
            this.playerSource = playerSource;
            this.playerDisplay = playerDisplay;
            this.playerHitBoxText = playerHitBoxText;
            this.playerHitBoxDisplay = playerHitBoxDisplay;
            this.playerColor = playerColor;
            this.sourceWidthLimit = sourceWidthLimit;
            this.attackHitBoxText = attackHitBoxText;
            this.attackHitBoxDisplay = attackHitBoxDisplay;
            spriteRow = 0;
            checkPoint = new Point(playerDisplay.X, playerDisplay.Y - 50);

            playerMoveSpeed = 2;
            playerSprintSpeed = 2;

            delay = 0;
            animSpeed = 8;

            facingLeftRight = true;
            playerLife = 3;

            gravity = 18;
        }

        public Texture2D PlayerText { get => playerText; }
        public Rectangle PlayerSource { get => playerSource; }
        public Rectangle PlayerDisplay { get => playerDisplay; set => playerDisplay = value; }
        public Texture2D PlayerHitBoxText { get => playerHitBoxText; }
        public Rectangle PlayerHitBoxDisplay { get => playerHitBoxDisplay; }
        public Color PlayerColor { get => playerColor; }
        public Texture2D AttackHitBoxText { get => attackHitBoxText; }
        public Rectangle AttackHitBoxDisplay { get => attackHitBoxDisplay; }
        public Point CheckPoint { get => checkPoint; set => checkPoint = value; }
        public int PlayerLife { get => playerLife; set => playerLife = value; }
        public void PlayerUpdate(List<Enemy> enemies, SoundEffectInstance sfxWalk, SoundEffectInstance sfxSlash, 
            SoundEffectInstance sfxRun, SoundEffectInstance sfxWhoosh,
            SoundEffectInstance sfxJump, SoundEffectInstance sfxHit, SoundEffectInstance sfxDeath)
        {
            if (animState != AnimState.Death)
            {
                //MOVEMENT
                if (animState == AnimState.Moving)
                {
                    PlayerMovement(sfxWalk, sfxRun);
                }
                //ATTACK
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && animState != AnimState.Attacking && jumpState == JumpState.Ground)
                {
                    sfxWhoosh.Play();
                    PlayerAttack();
                }
                //JUMP
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && jumpState == JumpState.Ground)
                {
                    sfxJump.Play();
                    jumpState = JumpState.Jumped;
                    playerSource.X = 0;
                    force = gravity;

                    animState = AnimState.Moving;
                }
                //HIT
                if (animState == AnimState.Hit && !isHit /*&& animState != AnimState.Death*/)
                {
                    isHit = true;
                    PlayerHit(sfxHit, sfxDeath);
                }
                //FALL FROM RAVINE 
                else if (PlayerDisplay.Top >= 544)
                {
                    PlayerHit(sfxHit, sfxDeath);
                    if (animState != AnimState.Death)
                    {
                        playerDisplay.Location = checkPoint;//new Point(100, 315); //EXCLUSIVE FOR LEVEL 1
                        animState = AnimState.Idle;
                    }
                }
                //IDLE
                if (animState == AnimState.Idle)
                {
                    sfxWalk.Stop();
                    sfxRun.Stop();
                    isHit = false;
                    animSpeed = 8;
                    //FACING RIGHT IDLE
                    if (facingLeftRight)
                    {
                        spriteRow = 0;
                    }
                    //FACING LEFT IDLE
                    else { spriteRow = 7; }
                    if (playerLife <= 0)
                    {
                        animState = AnimState.Death;
                    }
                }
                //CHECK HIT ATTACK
                foreach (Enemy en in enemies)
                {
                    if (attackHitBoxDisplay.Intersects(en.HitBoxDisplay) && animState == AnimState.Attacking)
                    {
                        sfxSlash.Play();
                        if (en.animState != Enemy.AnimState.Death)
                            en.animState = Enemy.AnimState.Hit;
                        break;
                    }
                }
                PlayerKeyState();
            }
            else
            {
                if (sfxDeath.State == SoundState.Stopped)
                {
                    Game1.hasPaused = true;
                }
                PlayerDeathStateAnim();
            }
            PlayerJumpState();
            AnimationDelay(animSpeed);
            ChangeAnimationState(spriteRow);
            playerHitBoxDisplay.Location = new Point(playerDisplay.X + 50,
                playerDisplay.Y + (playerDisplay.Height / 2));
        }
        void ChangeAnimationState(int spriteRow)
        {
            //WALKING
            if(spriteRow == 1 || spriteRow == 8)
            {
                sourceWidthLimit = playerText.Width - playerSource.Width * 3;
            }
            //JUMP
            else if (spriteRow == 3 || spriteRow == 10)
            {
                sourceWidthLimit = playerText.Width - playerSource.Width * 7;
            }
            //HIT
            else if (spriteRow == 5 || spriteRow == 12)
            {
                sourceWidthLimit = playerText.Width - playerSource.Width * 6;
            }
            //DEATH
            else if (spriteRow == 6 || spriteRow == 13)
            {
                sourceWidthLimit = playerText.Width - playerSource.Width * 4;
            }
            //REST THAT HAVE 8 FRAMES ANIMATION
            else
            {
                sourceWidthLimit = playerText.Width - playerSource.Width;
            }
            playerSource.Y = playerSource.Height * spriteRow;
        }
        void AnimationDelay(int animSpeed)
        {
            if (delay >= animSpeed)
            {
                PlayerAnimation();
                delay = 0;
            }
            delay++;
        }
        void PlayerAnimation()
        {
            if (jumpState != JumpState.Ground)
            {
                playerSource.X = 0;
            }
            else
            {
                if (playerSource.X < sourceWidthLimit)
                {
                    playerSource.X += playerSource.Width;
                }
                else if (animState != AnimState.Death)
                {
                    if (animState == AnimState.Attacking || animState == AnimState.Hit)
                    {
                        animState = AnimState.Idle;
                    }
                    playerSource.X = 0;
                }
            }
        }
        void PlayerMovement(SoundEffectInstance sfxWalk, SoundEffectInstance sfxRun)
        {
            //WALK LEFT                                       //LEFT BORDER
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && playerDisplay.X >= -30)
            {
                sfxWalk.Play();
                playerDisplay.X -= playerMoveSpeed;
                spriteRow = 8;
                facingLeftRight = false;
            }
            //WALK RIGHT                                       //RIGHT BORDER
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && playerDisplay.X <= 2200)
            {
                sfxWalk.Play();
                playerDisplay.X += playerMoveSpeed;
                spriteRow = 1;
                facingLeftRight = true;
            }
            //SPRINT
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && animState == AnimState.Moving)
            {
                sfxWalk.Stop();
                sfxRun.Play();
                if (facingLeftRight && playerDisplay.X <= 2200)
                {
                    playerDisplay.X += playerMoveSpeed * playerSprintSpeed;
                    spriteRow = 2;
                }
                else if(!facingLeftRight && playerDisplay.X >= -30)
                {
                    playerDisplay.X -= playerMoveSpeed * playerSprintSpeed;
                    spriteRow = 9;
                }
            }
        }
        void PlayerAttack()
        {
            animSpeed = 3;
            playerSource.X = 0;
            if (facingLeftRight)
            {
                attackHitBoxDisplay.Location = new Point(playerDisplay.X + 90,
                    playerDisplay.Y + (playerDisplay.Height / 2));
                spriteRow = 4;
            }
            else
            {
                attackHitBoxDisplay.Location = new Point(playerDisplay.X + 5,
                    playerDisplay.Y + (playerDisplay.Height / 2));
                spriteRow = 11;
            }
            animState = AnimState.Attacking;
        }
        void PlayerJumpState()
        {
            if (jumpState == JumpState.Jumped)
            {
                if (animState != AnimState.Hit)
                {
                    if (facingLeftRight)
                    {
                        spriteRow = 3;
                    }
                    else { spriteRow = 10; }
                }
                playerDisplay.Y -= force;
                force -= 1;
                if (force <= 0)
                {
                    jumpState = JumpState.Falling;
                }
            }
            if (jumpState == JumpState.Falling)
            {
                PlayerGravity();
            }
        }
        void PlayerGravity()
        {
            playerDisplay.Y -= force;
            force -= 1;
        }
        void PlayerKeyState()
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0 /*&& !isKeyPressed*/)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if ((keys[i] == Keys.Left || keys[i] == Keys.Right)
                        && animState == AnimState.Idle)
                    {
                        animSpeed = 8;
                        animState = AnimState.Moving;
                        playerSource.X = 0;
                    }
                    //PRESSING ONLY LEFTSHIFT
                    if (keys.Length == 1 && keys[i] == Keys.LeftShift && animState != AnimState.Attacking)
                    {
                        animState = AnimState.Idle;
                    }
                }
            }
            else if (keys.Length == 0 && animState != AnimState.Attacking && jumpState == JumpState.Ground && animState != AnimState.Hit
                && animState != AnimState.Death)
            {
                animState = AnimState.Idle;
            }
        }
        void PlayerHit(SoundEffectInstance sfxHit, SoundEffectInstance sfxDeath)
        {
            sfxHit.Play();
            if (animState != AnimState.Death && !(playerLife <= 0))
            {
                animSpeed = 5;
                playerSource.X = 0;
                //FACING RIGHT IDLE
                if (facingLeftRight)
                {
                    playerDisplay.X -= 40;
                    spriteRow = 5;
                }
                //FACING LEFT IDLE
                else
                {
                    spriteRow = 12;
                    playerDisplay.X += 40;
                }
                animState = AnimState.Hit;
                playerLife--;

                if (playerLife <= 0)
                {
                    PlayerDeath(sfxDeath);
                }
            }
        }
        void PlayerDeath(SoundEffectInstance sfxDeath)
        {
            sfxDeath.Play();
            animState = AnimState.Death;
            animSpeed = 12;
            playerSource.X = 0;
            PlayerDeathStateAnim();
        }
        void PlayerDeathStateAnim()
        {
            //FACING RIGHT IDLE
            if (facingLeftRight)
            {
                spriteRow = 6;
            }
            //FACING LEFT IDLE
            else { spriteRow = 13; }
        }
        public void PlayerDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerText, PlayerDisplay, PlayerSource, PlayerColor);
            spriteBatch.Draw(PlayerHitBoxText, PlayerHitBoxDisplay, Color.Transparent);
            if (animState == AnimState.Attacking)
            {
                spriteBatch.Draw(AttackHitBoxText, AttackHitBoxDisplay, Color.Transparent);
            }
        }
    }
}