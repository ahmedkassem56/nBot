using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot
{
    struct Monster
    {
        public string ID { get; set; }
        public string PK2Name { get; set; }
        public string Name { get; set; }
        public int Lvl { get; set; }
        public long HP { get; set; }
        public uint ObjectID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public byte DeathFlag { get; set; }
        public byte typeid1 { get; set; }
        public byte typeid2 { get; set; }
        public byte typeid3 { get; set; }
        public byte typeid4 { get; set; }
        public MobType SpawnType { get; set; }
        public int Distance { get; set; }
        public enum MobType
        {
            General,
            General_Party,
            Champion,
            Champion_Party,
            Unique,
            Giant,
            Giant_Party,
            Titan,
            Elite
        }
    }
    static class MonsterFunctions
    {
        public static Monster FindMob(uint id)
        {
            return BotData.SROMobs.Values.ToList().Find(x => x.ID == Globals.ModelToStr(id));
        }
        public static Monster FindMob(string pk2name)
        {
            return BotData.SROMobs.Values.ToList().Find(x => x.PK2Name == pk2name);
        }
    }
}
