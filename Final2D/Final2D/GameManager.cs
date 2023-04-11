using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final2D
{
    class GameManager
    {
        public enum GameStateEnum
        {
            MENU,
            START,
            CONTINUE,
            EXIT,
        }
        public GameStateEnum gameState;

        public enum GameLevel
        {
            LEVEL1A,
            LEVEL1B,
            LEVEL1C,
            LEVEL2A,
            LEVEL2B,
            LEVEL2C,
            COMPLETE
        }
        public GameLevel gameLevel;

        public GameManager(GameStateEnum gameState, GameLevel gameLevel)
        {
            this.gameState = gameState;
            this.gameLevel = gameLevel;
        }
    }
}
