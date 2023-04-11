using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Final2D
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameManager gameManager;
        Matrix camera;
        SaveLoadSystem saveSystem;

        Player player;
        List<Enemy> bloatEnemies1A;
        List<Enemy> bloatEnemies1B;
        List<Enemy> bloatEnemies1C;
        List<Enemy> centipedeEnemies2A;
        List<Enemy> centipedeEnemies2B;
        List<Enemy> centipedeEnemies2C;

        List<Platform> currentPlatforms;
        Platform currentBorder;
        Texture2D currentBG;
        List<Enemy> currentEnemies;
        SoundEffectInstance currentBGM;
        public static int currentEnemyCount;

        List<Platform> platforms1A;
        List<Platform> platforms1B;
        List<Platform> platforms1C;
        List<Platform> platforms2A; 
        List<Platform> platforms2B;
        List<Platform> platforms2C;

        Texture2D jungleBG;
        Texture2D jungleBorder;
        Texture2D deepBorder;
        Texture2D deepJungleBG;

        //MAIN MENU
        SpriteFont gameFont;
        MainMenu mainMenuUI;
        List<ListButtons> buttonsUI;
        Texture2D title;
        Texture2D boardUI;
        Texture2D controls;
        //HUD
        List<GameHUD> heart;
        GameHUD currentEnemyIcon;
        GameHUDwText objIcon;
        Texture2D bloatedIcon;
        Texture2D centipedeIcon;
        //PAUSE UI
        GameHUDwText pauseUI;
        List<ListButtons> gameOverUIButtons;
        #region SFX
        //BUTTON SFX UI - BGM
        SoundEffect bgmMenu, sfxClick, sfxCancel, sfxAreaClear, sfxCheckPoint;
        SoundEffectInstance bgmMenuInstance, sfxClickInstance, sfxCancelInstance, sfxAreaClearInst, sfxCheckPInst;

        SoundEffect bgmJungle, bgmJungleBoss, bgmDeepJungle, bgmDeepBoss, sfxLevelCmpt;
        SoundEffectInstance bgmJungleInstance, bgmJungleBossInstance, bgmDeepJungleInstance, bgmDeepBossInstance, sfxLevelCmptInstance;
        //Player sfx
        SoundEffect sfxWalk, sfxSlash, sfxRun, sfxWhoosh, sfxJump, sfxHit, sfxGameOver;
        SoundEffectInstance sfxWalkInstance, sfxSlashInstance, sfxRunInstance, sfxWhooshInstance, sfxJumpInstance, sfxHitInstance, sfxGameOverInstance;
        //Enemy Bloated
        SoundEffect sfxbloatAtk, sfxbloatDeath;
        SoundEffectInstance sfxbloatAtkInstance, sfxbloatDeathInstance;
        #endregion
        public static bool hasPaused, hasPressed;
        bool hasCheckPointReached, hasGameLoaded, isAreaClear;
        int delay;
        float cameraPosX;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Depth of the Thicket";
            //graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 544;
            //graphics.IsFullScreen = true;
            IsMouseVisible = true;
            graphics.ApplyChanges();
            //800x544
        }

        protected override void Initialize()
        {
            gameManager = new GameManager(GameManager.GameStateEnum.MENU, GameManager.GameLevel.LEVEL1A);
            hasPressed = false;
            hasPaused = false;
            camera = Matrix.Identity;
            saveSystem = new SaveLoadSystem();
            #region MAIN MENU
            Texture2D menuBG = Content.Load<Texture2D>("BGMENU");
            mainMenuUI = new MainMenu(menuBG, new Rectangle(0, 0, Window.ClientBounds.Width,
                Window.ClientBounds.Height), Color.White);
            buttonsUI = new List<ListButtons>();
            int buttonY = 70;
            int buttonX = Window.ClientBounds.Width / 2 - 90;
            for (int i = 0; i < 5; i++)
            {
                Point buttonPoint = new Point(buttonX, Window.ClientBounds.Height / 2 - buttonY);
                ListButtons menuButtons = new ListButtons(Content.Load<Texture2D>("buttonN"), Content.Load<Texture2D>("buttonP"), 
                    new Rectangle(buttonPoint.X, buttonPoint.Y, 169, 56), Color.White,
                    new Vector2(buttonPoint.X + 17, buttonPoint.Y+ 13), new Vector2(buttonPoint.X + 17, buttonPoint.Y + 23), Color.White, i);
                if (i == 3)
                {
                    //buttonX = Window.ClientBounds.Width - 200;
                    buttonY = -140;
                }
                else
                {
                    buttonY -= 70;
                }
                buttonsUI.Add(menuButtons);
            }
            #endregion

            #region PLAYER
            Texture2D playerTexture = Content.Load<Texture2D>("ConceptMeAnimation");
            Rectangle playerDisplay = new Rectangle(100, 315, 128, 128);
            Rectangle playerSource = new Rectangle(0, 0, playerTexture.Width / 8, playerTexture.Height / 14);
            player = new Player(playerTexture, playerSource, playerDisplay, 
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(playerDisplay.X, playerDisplay.Y, playerDisplay.Width / 4, playerDisplay.Height / 2),
                Color.White, playerTexture.Width - playerSource.Width,
                new Texture2D(GraphicsDevice, 1, 1), 
                new Rectangle(playerDisplay.X, playerDisplay.Y, playerDisplay.Width / 4, playerDisplay.Height / 2));
            player.PlayerHitBoxText.SetData(new Color[] { Color.White });
            player.AttackHitBoxText.SetData(new Color[] { Color.White });
            #endregion

            #region GAME HUD
            heart = new List<GameHUD>();
            Texture2D heartText = Content.Load<Texture2D>("heart");
            int posX = 0;
            for (int i = 0; i < player.PlayerLife; i++)
            {
                GameHUD heartTemp = new GameHUD(heartText, new Rectangle(posX, 0, 64, 64), Color.White);
                heart.Add(heartTemp);
                posX += 64;
            }

            bloatedIcon = Content.Load<Texture2D>("bloatedIcon");
            centipedeIcon = Content.Load<Texture2D>("centipedeIcon");
            currentEnemyIcon = new GameHUD(bloatedIcon, new Rectangle(Window.ClientBounds.Width - 100, 10, 64, 64), Color.White);
            objIcon = new GameHUDwText(Content.Load<Texture2D>("parchmentUI"), new Rectangle(Window.ClientBounds.Width - 280, 20, 234, 64), Color.White,
                new Vector2(Window.ClientBounds.Width - 260, 30), Color.Black);
            #endregion

            #region PAUSE UI
            pauseUI = new GameHUDwText(Content.Load<Texture2D>("bParchmentUI"), 
                new Rectangle(Window.ClientBounds.Width / 2 - 125, Window.ClientBounds.Height / 2 - 50, 240, 96), Color.White,
                new Vector2(Window.ClientBounds.Width / 2 - 60, Window.ClientBounds.Height / 2 - 20), Color.Black);
            gameOverUIButtons = new List<ListButtons>();
            int pausePosX = Window.ClientBounds.Width /2 - 300;
            for (int i = 5; i < 7; i++)
            {
                Point buttonPoint = new Point(pausePosX, Window.ClientBounds.Height / 2 + 100);
                ListButtons pauseButton = new ListButtons(Content.Load<Texture2D>("buttonN"), Content.Load<Texture2D>("buttonP"),
                    new Rectangle(buttonPoint.X, buttonPoint.Y, 169, 56), Color.White,
                    new Vector2(buttonPoint.X + 17, buttonPoint.Y + 13), new Vector2(buttonPoint.X + 17, buttonPoint.Y + 23), Color.White, i);
                gameOverUIButtons.Add(pauseButton); 
                pausePosX = Window.ClientBounds.Width / 2+ 130 ;
            }
            #endregion

            #region Enemy Bloat
            Texture2D enemyTexture = Content.Load<Texture2D>("big bloated");
            Rectangle enemySource = new Rectangle(0, 0, enemyTexture.Width / 6, enemyTexture.Height / 10);
            int sourceWidthLimit = enemyTexture.Width - enemySource.Width;
            #region BLOAT1A
            bloatEnemies1A = new List<Enemy>();
            Rectangle enemyDisplay = new Rectangle(400, 305, 144, 144);
            Enemy enemy = new Enemy(enemyTexture, enemySource, enemyDisplay,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay.X, enemyDisplay.Y, enemyDisplay.Width / 3, enemyDisplay.Height / 2),
                Color.White, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay.X, enemyDisplay.Y, enemyDisplay.Width / 4, enemyDisplay.Height / 2),
                400, 700,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay.X, enemyDisplay.Y, enemyDisplay.Width *2, enemyDisplay.Height / 4),
                7, 200);
            enemy.HitBoxText.SetData(new Color[] { Color.White });
            enemy.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy.RayCastHitBoxText.SetData(new Color[] { Color.White });
            bloatEnemies1A.Add(enemy);

            Rectangle enemyDisplay1 = new Rectangle(1400, 220, 144, 144);
            Enemy enemy1 = new Enemy(enemyTexture, enemySource, enemyDisplay1,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay1.X, enemyDisplay1.Y, enemyDisplay1.Width / 3, enemyDisplay1.Height / 2),
                Color.White, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay1.X, enemyDisplay1.Y, enemyDisplay1.Width / 4, enemyDisplay1.Height / 2),
                1400, 1800,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay1.X, enemyDisplay1.Y, enemyDisplay1.Width * 2, enemyDisplay1.Height / 4),
                7,200);
            enemy1.HitBoxText.SetData(new Color[] { Color.White });
            enemy1.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy1.RayCastHitBoxText.SetData(new Color[] { Color.White });
            bloatEnemies1A.Add(enemy1);
            #endregion
            #region BLOAT1B
            bloatEnemies1B = new List<Enemy>();
            Rectangle enemyDisplay2 = new Rectangle(200, 225, 144, 144);
            Enemy enemy2 = new Enemy(enemyTexture, enemySource, enemyDisplay2,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay2.X, enemyDisplay2.Y, enemyDisplay2.Width / 3, enemyDisplay2.Height / 2),
                Color.White, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay2.X, enemyDisplay2.Y, enemyDisplay2.Width / 4, enemyDisplay2.Height / 2),
                200, 500,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay2.X, enemyDisplay2.Y, enemyDisplay2.Width * 2, enemyDisplay2.Height / 4),
                7, 200);
            enemy2.HitBoxText.SetData(new Color[] { Color.White });
            enemy2.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy2.RayCastHitBoxText.SetData(new Color[] { Color.White });
            bloatEnemies1B.Add(enemy2);

            Rectangle enemyDisplay3 = new Rectangle(1800, 270, 144, 144);
            Enemy enemy3 = new Enemy(enemyTexture, enemySource, enemyDisplay3,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay3.X, enemyDisplay3.Y, enemyDisplay3.Width / 3, enemyDisplay3.Height / 2),
                Color.PaleVioletRed, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay3.X, enemyDisplay3.Y, enemyDisplay3.Width / 4, enemyDisplay3.Height / 2),
                1800, 2100,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay3.X, enemyDisplay3.Y, enemyDisplay3.Width * 2, enemyDisplay3.Height / 4),
                12,200);
            enemy3.HitBoxText.SetData(new Color[] { Color.White });
            enemy3.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy3.RayCastHitBoxText.SetData(new Color[] { Color.White });
            bloatEnemies1B.Add(enemy3);
            #endregion
            #region BLOAT1C
            bloatEnemies1C = new List<Enemy>();
            Rectangle enemyDisplay4 = new Rectangle(1500, 220, 144, 144);
            Enemy enemy4 = new Enemy(enemyTexture, enemySource, enemyDisplay4,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay4.X, enemyDisplay4.Y, enemyDisplay4.Width / 3, enemyDisplay4.Height / 2),
                Color.MediumVioletRed, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay4.X, enemyDisplay4.Y, enemyDisplay4.Width / 4, enemyDisplay4.Height / 2),
                1500, 2100,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay4.X, enemyDisplay4.Y, enemyDisplay4.Width * 2, enemyDisplay4.Height / 4),
                30, 300);
            enemy4.HitBoxText.SetData(new Color[] { Color.White });
            enemy4.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy4.RayCastHitBoxText.SetData(new Color[] { Color.White });
            bloatEnemies1C.Add(enemy4);
            #endregion
            #endregion

            #region Enemy Centipede
            Texture2D enemy2Texture = Content.Load<Texture2D>("centipede");
            Rectangle enemySource5 = new Rectangle(0, 0, enemy2Texture.Width / 6, enemy2Texture.Height / 10);
            int sourceWidthLimit2 = enemy2Texture.Width - enemySource5.Width;
            #region CENT2A
            centipedeEnemies2A = new List<Enemy>();
            int centPosX = 400;
            int centPosMax = 700;
            for (int i = 0; i < 3; i++)
            {
                Rectangle enemyDisplay5 = new Rectangle(centPosX, 305, 144, 144);
                Centipede enemy5 = new Centipede(enemy2Texture, enemySource5, enemyDisplay5,
                    new Texture2D(GraphicsDevice, 1, 1),
                    new Rectangle(enemyDisplay5.X, enemyDisplay5.Y, enemyDisplay5.Width / 3, enemyDisplay5.Height / 2),
                    Color.White, sourceWidthLimit2,
                    new Texture2D(GraphicsDevice, 1, 1),
                    new Rectangle(enemyDisplay5.X, enemyDisplay5.Y, enemyDisplay5.Width / 4, enemyDisplay5.Height / 2),
                    centPosX, centPosMax,
                    new Texture2D(GraphicsDevice, 1, 1),
                    new Rectangle(enemyDisplay5.X, enemyDisplay5.Y, enemyDisplay5.Width * 2, enemyDisplay5.Height / 4),
                    10, 200);
                enemy5.HitBoxText.SetData(new Color[] { Color.White });
                enemy5.AttackHitBoxText.SetData(new Color[] { Color.White });
                enemy5.RayCastHitBoxText.SetData(new Color[] { Color.White });
                centipedeEnemies2A.Add(enemy5);

                centPosX += 450;
                centPosMax += 450;
            }
            #endregion
            #region CENT2B
            centipedeEnemies2B = new List<Enemy>();
            Rectangle enemyDisplay6 = new Rectangle(1600, 180, 144, 144);
            Centipede enemy6 = new Centipede(enemy2Texture, enemySource5, enemyDisplay6,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay6.X, enemyDisplay6.Y, enemyDisplay6.Width / 3, enemyDisplay6.Height / 2),
                Color.BlueViolet, sourceWidthLimit2,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay6.X, enemyDisplay6.Y, enemyDisplay6.Width / 4, enemyDisplay6.Height / 2),
                1300, 1600,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay6.X, enemyDisplay6.Y, enemyDisplay6.Width * 2, enemyDisplay6.Height / 4),
                20, 200);
            enemy6.HitBoxText.SetData(new Color[] { Color.White });
            enemy6.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy6.RayCastHitBoxText.SetData(new Color[] { Color.White });
            centipedeEnemies2B.Add(enemy6);
            #endregion
            #region LASTLEVEL
            centipedeEnemies2C = new List<Enemy>();
            Rectangle enemyDisplay7 = new Rectangle(1300, 300, 144, 144);
            Centipede enemy7 = new Centipede(enemy2Texture, enemySource5, enemyDisplay7,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay7.X, enemyDisplay7.Y, enemyDisplay7.Width / 3, enemyDisplay7.Height / 2),
                Color.PaleVioletRed, sourceWidthLimit2,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay7.X, enemyDisplay7.Y, enemyDisplay7.Width / 4, enemyDisplay7.Height / 2),
                1300, 1600,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay7.X, enemyDisplay7.Y, enemyDisplay7.Width * 2, enemyDisplay7.Height / 4),
                30,400);
            enemy7.HitBoxText.SetData(new Color[] { Color.White });
            enemy7.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy7.RayCastHitBoxText.SetData(new Color[] { Color.White });
            centipedeEnemies2C.Add(enemy7);

            Texture2D enemyTexture2C = Content.Load<Texture2D>("big bloated");
            Rectangle enemyDisplay8 = new Rectangle(1400, 300, 144, 144);
            Enemy enemy8 = new Enemy(enemyTexture2C, enemySource5, enemyDisplay8,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay8.X, enemyDisplay8.Y, enemyDisplay8.Width / 3, enemyDisplay8.Height / 2),
                Color.MediumVioletRed, sourceWidthLimit,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay8.X, enemyDisplay8.Y, enemyDisplay8.Width / 4, enemyDisplay8.Height / 2),
                1400, 1800,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(enemyDisplay8.X, enemyDisplay8.Y, enemyDisplay8.Width * 2, enemyDisplay8.Height / 4),
                30,400);
            enemy8.HitBoxText.SetData(new Color[] { Color.White });
            enemy8.AttackHitBoxText.SetData(new Color[] { Color.White });
            enemy8.RayCastHitBoxText.SetData(new Color[] { Color.White });
            centipedeEnemies2C.Add(enemy8);
            #endregion
            #endregion

            #region LEVEL 1 PLATFORM
            Texture2D platformTexture = Content.Load<Texture2D>("platform");
            #region LEVEL 1A
            platforms1A = new List<Platform>();
            Rectangle platSource = new Rectangle(0, 0, platformTexture.Width/2, platformTexture.Height);
            Rectangle platformDis = new Rectangle(0, Window.ClientBounds.Height - 112, 1152,/*2304,*/ 128);
            Platform platform = new Platform(platformTexture, platSource, platformDis, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis.X, platformDis.Y + 7, platformDis.Width, platformDis.Height / 2));
            platform.HitBoxText.SetData(new Color[] { Color.White });
            platforms1A.Add(platform);

            Rectangle platSource1 = new Rectangle(platformTexture.Width / 2, 0, platformTexture.Width / 2, platformTexture.Height);
            Rectangle platformDis1 = new Rectangle((platformTexture.Width / 2) + 100, Window.ClientBounds.Height - 192, 1152/*2304*/, 128);
            Platform platform1 = new Platform(platformTexture, platSource1, platformDis1, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis1.X, platformDis1.Y + 7, platformDis1.Width, platformDis1.Height / 2));
            platform1.HitBoxText.SetData(new Color[] { Color.White });
            platforms1A.Add(platform1);
            #endregion
            #region LEVEL 1B
            platforms1B = new List<Platform>();
            Rectangle platSource2 = new Rectangle(platformTexture.Width / 2, 0, platformTexture.Width / 3, platformTexture.Height);
            Rectangle platformDis2 = new Rectangle(0, Window.ClientBounds.Height - 192, 768/*2304*/, 128);
            Platform platform2 = new Platform(platformTexture, platSource2, platformDis2, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis2.X, platformDis2.Y + 7, platformDis2.Width, platformDis2.Height / 2));
            platform2.HitBoxText.SetData(new Color[] { Color.White });
            platforms1B.Add(platform2);

            int platX1B = 900;
            int platY1B = 90;
            for (int i = 0; i < 3; i++)
            {
                Rectangle platSource3 = new Rectangle(0, 0, 128, platformTexture.Height);
                Rectangle platformDis3 = new Rectangle(platX1B, Window.ClientBounds.Height - platY1B, 128,/*2304,*/ 128);
                Platform platform3 = new Platform(platformTexture, platSource3, platformDis3, Color.White,
                    new Texture2D(GraphicsDevice, 1, 1),
                    new Rectangle(platformDis3.X, platformDis3.Y + 7, platformDis3.Width, platformDis3.Height / 2));
                platform3.HitBoxText.SetData(new Color[] { Color.White });
                platforms1B.Add(platform3);
                platX1B += 268;
                if(i == 0) { platY1B = 130; }else { platY1B = 90; }
            }

            Rectangle platSource4 = new Rectangle(platformTexture.Width / 2, 0, platformTexture.Width / 3, platformTexture.Height);
            Rectangle platformDis4 = new Rectangle(1684, Window.ClientBounds.Height - 142, 768/*2304*/, 128);
            Platform platform4 = new Platform(platformTexture, platSource4, platformDis4, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis4.X, platformDis4.Y + 7, platformDis4.Width, platformDis4.Height / 2));
            platform4.HitBoxText.SetData(new Color[] { Color.White });
            platforms1B.Add(platform4);
            #endregion
            #region LEVEL 1C
            platforms1C = new List<Platform>();
            Rectangle platSource5 = new Rectangle(0, 0, 512, platformTexture.Height);
            Rectangle platformDis5 = new Rectangle(0, Window.ClientBounds.Height - 142, 512/*2304*/, 128);
            Platform platform5 = new Platform(platformTexture, platSource5, platformDis5, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis5.X, platformDis5.Y + 7, platformDis5.Width, platformDis5.Height / 2));
            platform5.HitBoxText.SetData(new Color[] { Color.White });
            platforms1C.Add(platform5);

            Rectangle platSource52 = new Rectangle(128, 0, 128, platformTexture.Height);
            Rectangle platformDis52 = new Rectangle(655, Window.ClientBounds.Height - 142, 128/*2304*/, 128);
            Platform platform52 = new Platform(platformTexture, platSource52, platformDis52, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis52.X, platformDis52.Y + 7, platformDis52.Width, platformDis52.Height / 2));
            platform52.HitBoxText.SetData(new Color[] { Color.White });
            platforms1C.Add(platform52);

            Rectangle platSource6 = new Rectangle(platformTexture.Width / 2, 0, 256, platformTexture.Height);
            Rectangle platformDis6 = new Rectangle(900, Window.ClientBounds.Height - 142, 256/*2304*/, 128);
            Platform platform6 = new Platform(platformTexture, platSource6, platformDis6, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis6.X, platformDis6.Y + 7, platformDis6.Width, platformDis6.Height / 2));
            platform6.HitBoxText.SetData(new Color[] { Color.White });
            platforms1C.Add(platform6);

            Rectangle platSource7 = new Rectangle(0, 0, platformTexture.Width/2, platformTexture.Height);
            Rectangle platformDis7 = new Rectangle(1256, Window.ClientBounds.Height - 200, platformTexture.Width / 2/*2304*/, 128);
            Platform platform7 = new Platform(platformTexture, platSource7, platformDis7, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis7.X, platformDis7.Y + 7, platformDis7.Width, platformDis7.Height / 2));
            platform7.HitBoxText.SetData(new Color[] { Color.White });
            platforms1C.Add(platform7);
            #endregion
            #endregion

            #region LEVEL 2 PLATFORM
            Texture2D platform2Texture = Content.Load<Texture2D>("platformDeep");
            #region LEVEL 2A
            platforms2A = new List<Platform>();
            Rectangle platSource8 = new Rectangle(0, 0, platform2Texture.Width , platform2Texture.Height);
            Rectangle platformDis8 = new Rectangle(0, Window.ClientBounds.Height - 140, 2304,/*2304,*/ 128);
            Platform platform8 = new Platform(platform2Texture, platSource8, platformDis8, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis8.X, platformDis8.Y + 35, platformDis8.Width, platformDis8.Height / 2));
            platform8.HitBoxText.SetData(new Color[] { Color.White });
            platforms2A.Add(platform8);
            #endregion
            #region LEVEL 2B
            platforms2B = new List<Platform>();
            Rectangle platSource9 = new Rectangle(0, 0, 256, platform2Texture.Height);
            Rectangle platformDis9 = new Rectangle(0, Window.ClientBounds.Height - 140, 256,/*2304,*/ 128);
            Platform platform9 = new Platform(platform2Texture, platSource9, platformDis9, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis9.X, platformDis9.Y + 35, platformDis9.Width, platformDis9.Height / 2));
            platform9.HitBoxText.SetData(new Color[] { Color.White });
            platforms2B.Add(platform9);

            int platform10X = 356;
            int platform10Y = Window.ClientBounds.Height - 140;
            int platSource10X = 0;
            for (int i = 0; i < 3; i++)
            {
                Rectangle platSource10 = new Rectangle(platSource10X, 0, 128, platform2Texture.Height);
                Rectangle platformDis10 = new Rectangle(platform10X, platform10Y, 128,/*2304,*/ 128);
                Platform platform10 = new Platform(platform2Texture, platSource10, platformDis10, Color.White,
                    new Texture2D(GraphicsDevice, 1, 1),
                    new Rectangle(platformDis10.X, platformDis10.Y + 35, platformDis10.Width, platformDis10.Height / 2));
                platform10.HitBoxText.SetData(new Color[] { Color.White });
                platforms2B.Add(platform10);
                platform10X += 278;
                platform10Y -= 40;
                platSource10X += 128;
            }
            Rectangle platSource11 = new Rectangle(0, 0, 512, platform2Texture.Height);
            Rectangle platformDis11 = new Rectangle(platform10X + 50, platform10Y, 512,/*2304,*/ 128);
            Platform platform11 = new Platform(platform2Texture, platSource11, platformDis11, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis11.X, platformDis11.Y + 35, platformDis11.Width, platformDis11.Height / 2));
            platform11.HitBoxText.SetData(new Color[] { Color.White });
            platforms2B.Add(platform11);

            Rectangle platSource12 = new Rectangle(1280, 0, 128, platform2Texture.Height);
            Rectangle platformDis12 = new Rectangle(1900, Window.ClientBounds.Height/2, 128,/*2304,*/ 128);
            Platform platform12 = new Platform(platform2Texture, platSource12, platformDis12, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis12.X, platformDis12.Y + 35, platformDis12.Width, platformDis12.Height / 2));
            platform12.HitBoxText.SetData(new Color[] { Color.White });
            platforms2B.Add(platform12);

            Rectangle platSource13 = new Rectangle(1408, 0, 128, platform2Texture.Height);
            Rectangle platformDis13 = new Rectangle(2200, platform10Y + 40, 128,/*2304,*/ 128);
            Platform platform13 = new Platform(platform2Texture, platSource13, platformDis13, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis13.X, platformDis13.Y + 35, platformDis13.Width, platformDis13.Height / 2));
            platform13.HitBoxText.SetData(new Color[] { Color.White });
            platforms2B.Add(platform13);
            #endregion
            #region Level 2C
            platforms2C = new List<Platform>();
            Rectangle platSource14 = new Rectangle(1408, 0, 256, platform2Texture.Height);
            Rectangle platformDis14 = new Rectangle(0, platform10Y + 40, 256,/*2304,*/ 128);
            Platform platform14 = new Platform(platform2Texture, platSource14, platformDis14, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis14.X, platformDis14.Y + 35, platformDis14.Width, platformDis14.Height / 2));
            platform14.HitBoxText.SetData(new Color[] { Color.White });
            platforms2C.Add(platform14);

            Rectangle platSource15 = new Rectangle(0, 0, platform2Texture.Width, platform2Texture.Height);
            Rectangle platformDis15 = new Rectangle(462, Window.ClientBounds.Height/2 +120, platform2Texture.Width,/*2304,*/ 128);
            Platform platform15 = new Platform(platform2Texture, platSource15, platformDis15, Color.White,
                new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(platformDis15.X, platformDis15.Y + 35, platformDis15.Width, platformDis15.Height / 2));
            platform15.HitBoxText.SetData(new Color[] { Color.White });
            platforms2C.Add(platform15);
            #endregion
            #endregion

            jungleBorder = Content.Load<Texture2D>("jungleBorder");
            deepBorder = Content.Load<Texture2D>("deepBorder");
            currentBorder = new Platform(jungleBorder, Rectangle.Empty,
                new Rectangle(0, 0, 2304, 544), Color.White, new Texture2D(GraphicsDevice, 1, 1),
                new Rectangle(2280, 0, 43, 544));
            currentBorder.HitBoxText.SetData(new Color[] { Color.White });

            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameFont = Content.Load<SpriteFont>("GameFont");
            title = Content.Load<Texture2D>("title");
            boardUI = Content.Load<Texture2D>("boardUI");
            controls = Content.Load<Texture2D>("controls");
            #region UI SFX
            sfxClick = Content.Load<SoundEffect>("buttonConfirm");
            sfxClickInstance = sfxClick.CreateInstance();
            sfxClickInstance.Volume = .2f;
            sfxCancel = Content.Load<SoundEffect>("buttonCancel");
            sfxCancelInstance = sfxCancel.CreateInstance();
            sfxCancelInstance.Volume = .3f;
            sfxAreaClear = Content.Load<SoundEffect>("clearAreaSFX");
            sfxAreaClearInst = sfxAreaClear.CreateInstance();
            sfxAreaClearInst.Volume = 1;
            sfxCheckPoint = Content.Load<SoundEffect>("checkPointSFX");
            sfxCheckPInst = sfxCheckPoint.CreateInstance();
            sfxCheckPInst.Volume = .1f;
            sfxLevelCmpt = Content.Load<SoundEffect>("levelComplete");
            sfxLevelCmptInstance = sfxLevelCmpt.CreateInstance();
            sfxLevelCmptInstance.Volume = .3f;
            #endregion

            #region BGM
            bgmMenu = Content.Load<SoundEffect>("heroBG");
            bgmMenuInstance = bgmMenu.CreateInstance();
            bgmMenuInstance.IsLooped = true;
            bgmMenuInstance.Volume = .3f;
            bgmJungle = Content.Load<SoundEffect>("jungleBGM");
            bgmJungleInstance = bgmJungle.CreateInstance();
            bgmJungleInstance.IsLooped = true;
            bgmJungleInstance.Volume = .1f;
            bgmJungleBoss = Content.Load<SoundEffect>("jungleBossBGM");
            bgmJungleBossInstance = bgmJungleBoss.CreateInstance();
            bgmJungleBossInstance.IsLooped = true;
            bgmJungleBossInstance.Volume = .1f;
            bgmDeepJungle = Content.Load<SoundEffect>("deepJungleBGM");
            bgmDeepJungleInstance = bgmDeepJungle.CreateInstance();
            bgmDeepJungleInstance.IsLooped = true;
            bgmDeepJungleInstance.Volume = .1f;
            bgmDeepBoss = Content.Load<SoundEffect>("deepJungleBossBGM");
            bgmDeepBossInstance = bgmDeepBoss.CreateInstance();
            bgmDeepBossInstance.IsLooped = true;
            bgmDeepBossInstance.Volume = .1f;
            #endregion

            #region PLAYER SFX
            sfxWalk = Content.Load<SoundEffect>("walking");
            sfxWalkInstance = sfxWalk.CreateInstance();
            sfxWalkInstance.Volume = .3f;
            sfxRun = Content.Load<SoundEffect>("run");
            sfxRunInstance = sfxRun.CreateInstance();
            sfxRunInstance.Volume = .3f;
            sfxSlash = Content.Load<SoundEffect>("slash");
            sfxSlashInstance = sfxSlash.CreateInstance();
            sfxSlashInstance.Volume = .1f;
            sfxWhoosh = Content.Load<SoundEffect>("whoosh");
            sfxWhooshInstance = sfxWhoosh.CreateInstance();
            sfxWhooshInstance.Volume = .4f;
            sfxJump = Content.Load<SoundEffect>("jump");
            sfxJumpInstance = sfxJump.CreateInstance();
            sfxJumpInstance.Volume = .1f;
            sfxHit = Content.Load<SoundEffect>("hit");
            sfxHitInstance = sfxHit.CreateInstance();
            sfxHitInstance.Volume = .3f;
            sfxGameOver = Content.Load<SoundEffect>("gameOver");
            sfxGameOverInstance = sfxGameOver.CreateInstance();
            sfxGameOverInstance.Volume = .3f;
            #endregion

            #region BLOAT SFX
            sfxbloatAtk = Content.Load<SoundEffect>("bloatAttack");
            sfxbloatAtkInstance = sfxbloatAtk.CreateInstance();
            sfxbloatAtkInstance.Volume = .1f;
            sfxbloatDeath = Content.Load<SoundEffect>("bloatDeath");
            sfxbloatDeathInstance = sfxbloatDeath.CreateInstance();
            sfxbloatDeathInstance.Volume = .7f;
            #endregion

            jungleBG = Content.Load<Texture2D>("jungleBG");
            deepJungleBG = Content.Load<Texture2D>("deepForestBG");

            //INITIAL VALUE WHEN PRESSED START
            SetLevel(platforms1A, bloatEnemies1A, bloatEnemies1A.Count, bloatedIcon, jungleBorder, jungleBG, bgmJungleInstance);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            switch (gameManager.gameState)
            {
                case GameManager.GameStateEnum.MENU:
                    mainMenuUI.Update(buttonsUI, gameManager, bgmMenuInstance, sfxClickInstance, sfxCancelInstance);
                    break;
                case GameManager.GameStateEnum.CONTINUE:
                    LoadLevel();
                    break;
                #region CASE START
                case GameManager.GameStateEnum.START:
                    #region CAMERA
                    cameraPosX = -player.PlayerDisplay.X + Window.ClientBounds.Width / 3;
                    if (cameraPosX >= 0)
                    {
                        cameraPosX = 0;
                    }
                    else if (cameraPosX <= -1500) { cameraPosX = -1500; }
                    camera = Matrix.CreateTranslation(cameraPosX, camera.Translation.Y, camera.Translation.Z);
                    //HUD
                    for (int i = 0; i < heart.Count; i++)
                    {
                        heart[i].Move(cameraPosX);
                    }
                    currentEnemyIcon.Move(cameraPosX);
                    objIcon.Move(cameraPosX);
                    //GAME UI
                    pauseUI.Move(cameraPosX);
                    foreach (ListButtons lb in gameOverUIButtons)
                    {
                        lb.Move(cameraPosX);
                    }
                    #endregion
                    if (!hasPaused)
                    {
                        player.PlayerUpdate(currentEnemies, sfxWalkInstance, sfxSlashInstance, sfxRunInstance,
                            sfxWhooshInstance, sfxJumpInstance, sfxHitInstance, sfxGameOverInstance);
                        #region PLAYER-PLATFORM COLLISION
                        foreach (Platform p in currentPlatforms)
                        {
                            if (p.HitBoxDisplay.Intersects(player.PlayerHitBoxDisplay))
                            {
                                if (player.PlayerHitBoxDisplay.Bottom >= p.HitBoxDisplay.Bottom)
                                {
                                    player.jumpState = Player.JumpState.Falling;
                                    break;
                                }
                                else
                                {
                                    player.jumpState = Player.JumpState.Ground;
                                    break;
                                }
                            }
                            else if (player.jumpState != Player.JumpState.Jumped)
                            {
                                player.jumpState = Player.JumpState.Falling;
                            }
                        }
                        #endregion

                        currentBGM.Play();
                        #region NEXT LEVEL
                        if (currentEnemyCount == 0 && !isAreaClear) { sfxAreaClearInst.Play(); isAreaClear = true; }
                        if (currentBorder.HitBoxDisplay.Intersects(player.PlayerHitBoxDisplay) && currentEnemyCount == 0)
                        {
                            isAreaClear = false;
                            player.PlayerDisplay = new Rectangle(-32, 
                                player.PlayerDisplay.Y, player.PlayerDisplay.Width, player.PlayerDisplay.Height);
                            //CHECK WHAT CURRENT LEVEL
                            switch (gameManager.gameLevel)
                            {
                                case GameManager.GameLevel.LEVEL1A:
                                    SetLevel(platforms1B, bloatEnemies1B, bloatEnemies1B.Count, bloatedIcon, jungleBorder, jungleBG, bgmJungleInstance);
                                    gameManager.gameLevel = GameManager.GameLevel.LEVEL1B;
                                    SaveLevel();
                                    break;
                                case GameManager.GameLevel.LEVEL1B:
                                    currentBGM.Stop(); 
                                    gameManager.gameLevel = GameManager.GameLevel.LEVEL1C;
                                    SetLevel(platforms1C, bloatEnemies1C, bloatEnemies1C.Count, bloatedIcon, jungleBorder, jungleBG, bgmJungleBossInstance);
                                    SaveLevel();
                                    break;
                                case GameManager.GameLevel.LEVEL1C:
                                    currentBGM.Stop();
                                    gameManager.gameLevel = GameManager.GameLevel.LEVEL2A;
                                    SetLevel(platforms2A, centipedeEnemies2A, centipedeEnemies2A.Count, centipedeIcon, deepBorder, deepJungleBG, bgmDeepJungleInstance);
                                    break;
                                case GameManager.GameLevel.LEVEL2A:
                                    SetLevel(platforms2B, centipedeEnemies2B, centipedeEnemies2B.Count, centipedeIcon, deepBorder, deepJungleBG, bgmDeepJungleInstance);
                                    gameManager.gameLevel = GameManager.GameLevel.LEVEL2B;
                                    SaveLevel();
                                    break;
                                case GameManager.GameLevel.LEVEL2B:
                                    currentBGM.Stop();
                                    gameManager.gameLevel = GameManager.GameLevel.LEVEL2C;
                                    SetLevel(platforms2C, centipedeEnemies2C, centipedeEnemies2C.Count, centipedeIcon, deepBorder, deepJungleBG, bgmDeepBossInstance);
                                    SaveLevel();
                                    break;
                                case GameManager.GameLevel.LEVEL2C:
                                    //VICTORY
                                    sfxLevelCmptInstance.Play();
                                    gameManager.gameLevel = GameManager.GameLevel.COMPLETE;
                                    hasPaused = true;
                                    break;
                            }
                        }
                        #endregion
                        //FOR UI
                        if (hasCheckPointReached || hasGameLoaded)
                        {
                            if(delay >= 120)
                            {
                                hasCheckPointReached = false;
                                hasGameLoaded = false;
                                delay = 0;
                            }
                            delay++;
                        }
                        foreach (Enemy en in currentEnemies)
                        {
                            en.EnemyUpdate(player, sfxbloatAtkInstance, sfxbloatDeathInstance);
                        }
                    }

                    #region PAUSE/GAME OVER BUTTON
                    foreach (ListButtons b in gameOverUIButtons)
                    {
                        if (b.InitialPos.Contains(Mouse.GetState().Position))
                        {
                            b.HoverButton();
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !hasPressed)
                            {
                                hasPressed = true;
                                switch (b.FontText)
                                {
                                    case " MAIN MENU":
                                        sfxClickInstance.Play();
                                        gameManager.gameState = GameManager.GameStateEnum.MENU;
                                        hasPressed = false;
                                        currentBGM.Stop();
                                        Initialize();
                                        //checkStartGame = true;
                                        break;
                                    case "         QUIT":
                                        sfxCancelInstance.Play();
                                        Exit();
                                        break;
                                }
                            }
                            else if (Mouse.GetState().LeftButton == ButtonState.Released)
                            {
                                hasPressed = false;
                            }
                        }
                        else
                        {
                            b.NormalButton();
                        }
                    }
                    #endregion

                    #region KEY PRESSED PAUSE
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !hasPressed && player.animState != Player.AnimState.Death
                        && gameManager.gameLevel != GameManager.GameLevel.COMPLETE)
                    {
                        //sfxClick.Play();
                        sfxClickInstance.Play();
                        hasPressed = true;
                        if (!hasPaused)
                        {
                            hasPaused = true;
                        }
                        else { hasPaused = false; }
                    }
                    else if (Keyboard.GetState().IsKeyUp(Keys.Escape) && hasPressed)
                    {
                        hasPressed = false;
                    }
                    #endregion
                    break;
                #endregion
                case GameManager.GameStateEnum.EXIT:
                    Exit();
                    break;
            }

            #region CHEAT
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                //1 LIFE SA ENEMY
                sfxAreaClear.Play();
                foreach (Enemy en in currentEnemies)
                {
                    en.CharLife = 1;
                }
                //SHOW SCREEN LEVEL COMPLETE
                //gameManager.gameLevel = GameManager.GameLevel.COMPLETE;
                //hasPaused = true;
            }
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null,
                null, null, null, null, camera);

            if (gameManager.gameState == GameManager.GameStateEnum.START)
            {
                spriteBatch.Draw(currentBG, new Rectangle(0, 0, 2304, 544), Color.White);
                foreach (Platform plat in currentPlatforms)
                {
                    spriteBatch.Draw(plat.PlatText, plat.PlatDisplay, plat.PlatSource, plat.PlatColor);
                    spriteBatch.Draw(plat.HitBoxText, plat.HitBoxDisplay, Color.Transparent);
                }
                foreach (Enemy en in currentEnemies)
                {
                    en.EnemyDraw(spriteBatch);
                }
                player.PlayerDraw(spriteBatch);

                spriteBatch.Draw(currentBorder.PlatText, currentBorder.PlatDisplay, currentBorder.PlatColor);
                spriteBatch.Draw(currentBorder.HitBoxText, currentBorder.HitBoxDisplay, Color.Transparent);
                #region GAME HUD
                for (int i = 0; i < player.PlayerLife; i++)
                {
                    spriteBatch.Draw(heart[i].UIText, heart[i].UIDisplay, heart[i].UIColor);
                }
                spriteBatch.Draw(objIcon.UIText, objIcon.UIDisplay, objIcon.UIColor);
                spriteBatch.Draw(currentEnemyIcon.UIText, currentEnemyIcon.UIDisplay, currentEnemyIcon.UIColor);
                string obj;
                if (isAreaClear)
                {
                    obj = "Area cleared!!!";
                }
                else { obj = "Cleanse the area \nof ravaging beasts.  X " + currentEnemyCount; }
                spriteBatch.DrawString(gameFont, obj , objIcon.FontPos, objIcon.FontColor);
                if (hasCheckPointReached)
                {
                    spriteBatch.DrawString(gameFont, "Checkpoint reached...", new Vector2(10, Window.ClientBounds.Height - 50), Color.White);
                }else if (hasGameLoaded)
                {
                    spriteBatch.DrawString(gameFont, "Game successfully loaded...", new Vector2(10, Window.ClientBounds.Height - 50), Color.White);
                }
                #endregion

                #region GAME UI
                if (hasPaused)
                {
                    if (gameManager.gameLevel != GameManager.GameLevel.COMPLETE)
                        spriteBatch.Draw(pauseUI.UIText, pauseUI.UIDisplay, pauseUI.UIColor);
                    foreach (ListButtons lb in gameOverUIButtons)
                    {
                        spriteBatch.Draw(lb.ButtonText, lb.ButtonRect, lb.ButtonColor);
                        spriteBatch.DrawString(gameFont, lb.FontText, lb.FontPos, lb.FontColor);
                    }
                    if (player.animState == Player.AnimState.Death)
                    {
                        spriteBatch.DrawString(gameFont, "GAME OVER", pauseUI.FontPos, pauseUI.FontColor);
                    }
                    else if(gameManager.gameLevel == GameManager.GameLevel.COMPLETE)
                    {
                        spriteBatch.Draw(pauseUI.UIText, new Rectangle(Window.ClientBounds.Width/2-200,Window.ClientBounds.Height/2-70,391,144), pauseUI.UIColor);
                        spriteBatch.DrawString(gameFont, "YOU ESCAPED THE DEEP FOREST!!\n      THANK YOU FOR PLAYING!!", 
                            new Vector2(Window.ClientBounds.Width/2- 168, Window.ClientBounds.Height/2 -25), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(gameFont, "   PAUSED", pauseUI.FontPos, pauseUI.FontColor);
                    }
                }
                #endregion
            }
            else
            {
                mainMenuUI.Draw(spriteBatch, buttonsUI, gameFont, title, boardUI, controls);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        void SetLevel(List<Platform> platforms, List<Enemy> enemies, int enemyCount, Texture2D enemyIcon, Texture2D bgBorder,
            Texture2D backGround, SoundEffectInstance bgm)
        {
            currentBG = backGround;
            currentPlatforms = platforms;
            currentEnemies = enemies;
            currentEnemyCount = enemyCount;
            currentEnemyIcon.UIText = enemyIcon;
            currentBorder.PlatText = bgBorder;
            currentBGM = bgm;
        }
        void SaveLevel()
        {
            sfxCheckPInst.Play();
            player.CheckPoint = new Point(player.CheckPoint.X, player.PlayerDisplay.Y - 100);
            hasCheckPointReached = true;
            saveSystem.SaveData(player.PlayerDisplay, player.CheckPoint, player.PlayerLife, gameManager.gameLevel.ToString());
        }
        void LoadLevel()
        {
            sfxCheckPInst.Play();
            PlayerData playerData = saveSystem.LoadData();
            player.PlayerDisplay = playerData.playerDisplay;
            player.CheckPoint = playerData.playerCheckPoint;
            player.PlayerLife = playerData.playerLife;
            gameManager.gameLevel = (GameManager.GameLevel)Enum.Parse(typeof(GameManager.GameLevel), playerData.level);
            hasGameLoaded = true;
            LoadAreaLevel();
            gameManager.gameState = GameManager.GameStateEnum.START;
        }
        void LoadAreaLevel()
        {
            switch (gameManager.gameLevel)
            {
                case GameManager.GameLevel.LEVEL1A:
                    currentBG = jungleBG;
                    currentPlatforms = platforms1A;
                    currentEnemies = bloatEnemies1A;
                    currentEnemyCount = bloatEnemies1A.Count;
                    currentEnemyIcon.UIText = bloatedIcon;
                    currentBorder.PlatText = jungleBorder;
                    currentBGM = bgmJungleInstance;
                    break;
                case GameManager.GameLevel.LEVEL1B:
                    currentBG = jungleBG;
                    currentPlatforms = platforms1B;
                    currentEnemies = bloatEnemies1B;
                    currentEnemyCount = bloatEnemies1B.Count;
                    currentEnemyIcon.UIText = bloatedIcon;
                    currentBorder.PlatText = jungleBorder;
                    currentBGM = bgmJungleInstance;
                    break;
                case GameManager.GameLevel.LEVEL1C:
                    currentBG = jungleBG;
                    currentPlatforms = platforms1C;
                    currentEnemies = bloatEnemies1C;
                    currentEnemyCount = bloatEnemies1C.Count;
                    currentEnemyIcon.UIText = bloatedIcon;
                    currentBorder.PlatText = jungleBorder;
                    currentBGM = bgmJungleBossInstance;
                    break;
                case GameManager.GameLevel.LEVEL2A:
                    currentBG = deepJungleBG;
                    currentPlatforms = platforms2A;
                    currentEnemies = centipedeEnemies2A;
                    currentEnemyCount = centipedeEnemies2A.Count;
                    currentEnemyIcon.UIText = centipedeIcon;
                    currentBorder.PlatText = deepBorder;
                    currentBGM = bgmDeepJungleInstance;
                    break;
                case GameManager.GameLevel.LEVEL2B:
                    currentBG = deepJungleBG;
                    currentPlatforms = platforms2B;
                    currentEnemies = centipedeEnemies2B;
                    currentEnemyCount = centipedeEnemies2B.Count;
                    currentEnemyIcon.UIText = centipedeIcon;
                    currentBorder.PlatText = deepBorder;
                    currentBGM = bgmDeepJungleInstance;
                    break;
                case GameManager.GameLevel.LEVEL2C:
                    currentBG = deepJungleBG;
                    currentPlatforms = platforms2C;
                    currentEnemies = centipedeEnemies2C;
                    currentEnemyCount = centipedeEnemies2C.Count;
                    currentEnemyIcon.UIText = centipedeIcon;
                    currentBorder.PlatText = deepBorder;
                    currentBGM = bgmDeepBossInstance;
                    break;
            }
        }
    }
}
