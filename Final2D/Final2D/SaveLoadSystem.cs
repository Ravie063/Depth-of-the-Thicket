using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Final2D
{
    class SaveLoadSystem
    {
        public void SaveData(Rectangle playerDisplay, Point playerCheckPoint, int playerLife, string gameLevel)
        {
            PlayerData pd = new PlayerData();
            pd.playerDisplay = playerDisplay;
            pd.playerCheckPoint = playerCheckPoint;
            pd.playerLife = playerLife;
            pd.level = gameLevel;
            XmlSerializer saveData = new XmlSerializer(typeof(PlayerData));
            StreamWriter sw = new StreamWriter("saveData.txt");
            saveData.Serialize(sw, pd);
            sw.Close();
        }
        public PlayerData LoadData()
        {
            XmlSerializer loadData = new XmlSerializer(typeof(PlayerData));
            StreamReader sr = new StreamReader("saveData.txt");
            PlayerData data = (PlayerData)loadData.Deserialize(sr);
            sr.Close();
            return data;
        }
    }
}
