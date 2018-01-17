using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot
{
    class Character
    {
        public static string charName = "<None>";
        public static string GuildName = "<None>";
        public static string GuildGrantName = "<None>";
        public static uint hp = 0;
        public static uint mp = 0;
        public static ulong exp;
        public static uint AccountID;
        public static uint max_hp = 0;
        public static uint max_mp = 0;
        public static ushort str = 0;
        public static ushort _int = 0;
        public static uint ObjectID;
        public static byte[] CharID;
        public static int x;
        public static int y;
        public static byte MaxSlots;
        public static byte Level;
        public static byte ZerkPoints;
        public static uint sp;
        public static ulong gold;
    }
    struct Character_
    {
        public string Name { get; set; }
        public string Guild { get; set; }
        public string GrantName { get; set; }
        public uint objID { get; set; }
        public List<Item_> Items { get; set; }
    }
    struct Item_
    {
        public uint ID { get; set; }
        public byte Plus { get; set; }
    }
}
