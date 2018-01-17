using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot
{
    struct ItemData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PK2Name { get; set; }
        public int Lvl { get; set; }
        public uint Plus { get; set; }
        public uint Slot { get; set; }
        public ushort Amount { get; set; }
        public byte typeid1{ get; set; }
        public byte typeid2{ get; set; }
        public byte typeid3{ get; set; }
        public byte typeid4 { get; set; }
        public ItemType Type { get; set; }

        public enum ItemType
        {
            Equipable,
            AttackPet,
            GrabPet,
            ETC
        }
    }
    struct Drop
    {
        public uint objID { get; set; }
        public uint ID { get; set; }
    }
    static class ItemsFunctions
    {
        public static ItemData FindItem(uint id)
        {
            return BotData.SROItems.Values.ToList().Find(x => x.ID == Globals.ModelToStr(id));
        }
        public static ItemData FindItem(string pk2name)
        {
            return BotData.SROItems.Values.ToList().Find(x => x.PK2Name == pk2name);
        }

    }
}
