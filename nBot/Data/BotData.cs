using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace nBot
{
    class BotData
    {
        public static Dictionary<string, ItemData> SROItems = new Dictionary<string, ItemData>();
        public static Dictionary<string, SkillData> SROSkills = new Dictionary<string, SkillData>();
        public static Dictionary<string, Monster> SROMobs = new Dictionary<string, Monster>();
        public static bool Bot = false;
        public static bool Buff = false;
        public static bool Attack = false;
        public static Stopwatch buff_sw = new Stopwatch();
        public static Stopwatch attack_sw = new Stopwatch();
        public static Thread BuffThread = new Thread(new ThreadStart(Logic.Buff.MyLoop));
        public static Thread AttackThread = new Thread(new ThreadStart(Logic.Attack.Attack_MainLoop));
    }
}
