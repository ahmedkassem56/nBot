using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
namespace nBot
{
    class Globals
    {
        public static List<ItemData> char_items = new List<ItemData>();
        public static Dictionary<uint,SkillData> char_skills = new Dictionary<uint,SkillData>();
        public static Dictionary<string, ushort> servers_list = new Dictionary<string, ushort>();
        public static MainForm Main;
        public static Form2 Analyzer;
        public static Process SROClient;
        public static string IP = "login.mythonline.eu";
        public static string[] LevelData = new string[140];
        public static int ListenPort = GetPort();
        #region GetPort method
        private static int GetPort()
        {
            for (int i = 1600; i < 3000; i++)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), i));
                    socket.Close();
                    return i;
                }
                catch { i += 4; }
            }
            return 0;
        }
        #endregion
        public static ServerEnum Server = ServerEnum.None;
        public static enumLoginType LoginType;
        public static Dictionary<uint, Monster> AroundMobs = new Dictionary<uint, Monster>();
        public static Dictionary<uint, Character_> AroundChars = new Dictionary<uint, Character_>();
        public static Dictionary<uint, Drop> AroundDrops = new Dictionary<uint, Drop>();
        public enum ServerEnum
        {
            None,
            Gateway,
            Agent
        }
        public enum enumLoginType
        {
            Client,
            Clientless,
            Clientless2
        }
        public static int GameX(int xsec, int xpos)
        {
            return (xsec - 135) * 192 + (xpos / 10);
        }
        public static int GameY(int ysec, int ypos)
        {
            return (ysec - 92) * 192 + (ypos / 10);
        }
        public static void UpdateLogs(string msg)
        {
            Main.logs.AppendText(msg + Environment.NewLine);
        }

        public static string ModelToStr(uint model)
        {
            return Inverse(model.ToString("X8"));
        }

        public static string Inverse(string model)
        {
            string s1 = model.Substring(0, 2);
            string s2 = model.Substring(2, 2);
            string s3 = model.Substring(4, 2);
            string s4 = model.Substring(6, 2);
            model = s4 + s3 + s2 + s1;
            return model;
        }

    }
}
