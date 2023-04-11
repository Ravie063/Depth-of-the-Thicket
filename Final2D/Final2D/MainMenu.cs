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
    class MainMenu
    {
        Texture2D menuBGText;
        Rectangle menuBGDis;
        Color menuBGCol;
        bool checkMenu, checkOption;
        //bool hasPressed;
        int count, buttonCount;
        public MainMenu(Texture2D menuBGText, Rectangle menuBGDis, Color menuBGCol)
        {
            this.menuBGText = menuBGText;
            this.menuBGDis = menuBGDis;
            this.menuBGCol = menuBGCol;
            checkMenu = true;

            count = 0;
            buttonCount = 4;
        }

        public void Update(List<ListButtons> buttonsUI, GameManager gameManager,
            SoundEffectInstance bgmMenuInstance, SoundEffectInstance sfxClick, SoundEffectInstance sfxCancel)
        {
            bgmMenuInstance.Play();
            #region MAIN MENU
            for (int i = 0; i < 4; i++)
            {
                if (buttonsUI[i].ButtonRect.Contains(Mouse.GetState().Position) && checkMenu)
                {
                    buttonsUI[i].HoverButton();
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !Game1.hasPressed)
                    {
                        Game1.hasPressed = true;
                        switch (buttonsUI[i].FontText)
                        {
                            case "       START":
                                sfxClick.Play();
                                gameManager.gameState = GameManager.GameStateEnum.START;
                                Game1.hasPressed = false;
                                bgmMenuInstance.Stop();
                                //checkStartGame = true;
                                break;
                            case "   CONTINUE":
                                sfxClick.Play();
                                gameManager.gameState = GameManager.GameStateEnum.CONTINUE;
                                Game1.hasPressed = false;
                                bgmMenuInstance.Stop();
                                break;
                            case "   CONTROLS":
                                sfxClick.Play();
                                checkMenu = false;
                                checkOption = true;
                                break;
                            case "        EXIT":
                                sfxCancel.Play();
                                gameManager.gameState = GameManager.GameStateEnum.EXIT;
                                //checkExit = true;
                                break;
                        }
                    }
                    else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        Game1.hasPressed = false;
                    }
                }
                else
                {
                    buttonsUI[i].NormalButton();
                }
            }
            #endregion

            #region OPTION
            if (buttonsUI[4].ButtonRect.Contains(Mouse.GetState().Position) && checkOption)
            {
                buttonsUI[4].HoverButton();
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !Game1.hasPressed 
                    && buttonsUI[4].FontText == "        BACK")
                {
                    Game1.hasPressed = true;
                    sfxCancel.Play();
                    checkMenu = true;
                    checkOption = false;
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    Game1.hasPressed = false;
                }
            }
            else
            {
                buttonsUI[4].NormalButton();
            }
            #endregion

            if (checkMenu)
            {
                count = 0;
                buttonCount = 4;
            }
            if (checkOption)
            {
                count = 4;
                buttonCount = buttonsUI.Count;
            }
        }

        public void Draw(SpriteBatch spriteBatch, List<ListButtons> buttonsUI, SpriteFont gameFont, Texture2D title,
            Texture2D boardUI, Texture2D controls)
        {
            spriteBatch.Draw(menuBGText, menuBGDis, menuBGCol);
            if (checkOption)
            {
                spriteBatch.Draw(boardUI, new Rectangle(39, 83, 721, 317), Color.White);
                spriteBatch.Draw(controls, new Rectangle(80, 126, 640, 225), Color.White);
            }
            else
            {
                spriteBatch.Draw(title, new Rectangle(150, 30, 500, 161), Color.White);
            }
            for (; count < buttonCount; count++)
            {
                spriteBatch.Draw(buttonsUI[count].ButtonText, buttonsUI[count].ButtonRect, buttonsUI[count].ButtonColor);
                spriteBatch.DrawString(gameFont, buttonsUI[count].FontText, buttonsUI[count].FontPos, buttonsUI[count].FontColor);
            }
        }
    }
}
