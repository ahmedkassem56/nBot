using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot.Packets
{
    class CharData
    {
        public static Packet char_data;
        public static void ParseCharData(Packet packet)
        {
            try
            {
                /*byte[] packet_bytes = packet.GetBytes();
                string Date = "[" + DateTime.Now.Day + " -" + DateTime.Now.Month + "] [" + DateTime.Now.Hour + " - " + DateTime.Now.Minute + " ]";
                System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\characterdata_" + Date + ".txt",Utility.HexDump(packet_bytes));*/
                Logic.Buff.ActiveBuffs.Clear();
                Globals.char_skills.Clear();
                Globals.char_items.Clear();
                Globals.Main.skillList.Items.Clear();
                Globals.Main.itemList.Items.Clear();
                packet.ReadDWORD();
                packet.ReadDWORD();
                packet.ReadBYTE();
                Character.Level = packet.ReadBYTE();
                packet.ReadBYTE();
                Character.exp = packet.ReadQWORD();
                packet.ReadDWORD();
                Character.gold = packet.ReadQWORD();
                Character.sp = packet.ReadDWORD();
                packet.ReadWORD();
                Character.ZerkPoints = packet.ReadBYTE();
                packet.ReadDWORD();
                uint hp = packet.ReadDWORD();
                Character.hp = hp;
                uint mp = packet.ReadDWORD();
                Character.mp = mp;
                packet.ReadBYTE(9);
                packet.ReadBYTE();
                byte MaxSlots = packet.ReadBYTE();
                Character.MaxSlots = MaxSlots;
                byte item_count = packet.ReadBYTE();
                for (int x = 0; x < item_count; x++)
                {
                    byte slot = packet.ReadBYTE();
                    packet.ReadDWORD(); // ??
                    uint ID = packet.ReadDWORD();
                    string strID = Globals.ModelToStr(ID);
                    ItemData item = BotData.SROItems[strID];
                    if (item.typeid2 == 1)
                    {
                        byte plus = packet.ReadBYTE();
                        packet.ReadQWORD();
                        packet.ReadDWORD();
                        byte blue = packet.ReadBYTE();
                        for (int i = 0; i < blue; i++)
                        {
                            packet.ReadDWORD();
                            packet.ReadDWORD();
                        }
                        packet.ReadBYTE();
                        byte sockets = packet.ReadBYTE(); // socket stones
                        for (int i = 0; i < sockets; i++)
                        {
                            packet.ReadBYTE(); // count
                            packet.ReadQWORD(); // data
                        }
                        byte adv = packet.ReadBYTE();
                        byte used = packet.ReadBYTE();
                        uint adv_type = 0x00;
                        if (used == 0x01)
                        {
                            packet.ReadBYTE(); // 0x00
                            packet.ReadDWORD(); // ??
                            adv_type = packet.ReadDWORD();
                        }
                        ItemData newitem = item;
                        newitem.Plus = plus + adv_type;
                        newitem.Slot = slot;
                        newitem.Amount = 1;
                        Globals.char_items.Add(newitem);
                        Globals.Main.itemList.Items.Add(newitem.Name + " +" + newitem.Plus);
                    }
                    else if ((item.PK2Name.StartsWith("ITEM_COS") && item.PK2Name.Contains("SILK")) || (item.PK2Name.StartsWith("ITEM_EVENT_COS") && !item.PK2Name.Contains("_C_")))
                    {
                        byte flag = packet.ReadBYTE();
                        if (flag == 2 || flag == 3 || flag == 4)
                        {
                            uint objID = packet.ReadDWORD();
                            string name = packet.ReadSTRING("ascii");
                            packet.ReadBYTE();
                            if (item.typeid2 == 2 && item.typeid3 == 1 && item.typeid4 != 1)
                            {
                                packet.ReadDWORD();
                            }
                            packet.ReadBYTE();
                        }
  
                        ItemData newitem = item;
                        newitem.Slot = slot;
                        newitem.Amount = 1;
                        newitem.Plus = 0;
                        Globals.char_items.Add(newitem);
                        Globals.Main.itemList.Items.Add(newitem.Name + " +" + newitem.Plus);
                    }
                    else if (item.typeid2 == 2 && item.typeid3 == 1 && (item.typeid4 == 1 || item.typeid4 == 2))
                    {
                        byte flag = packet.ReadBYTE();
                        if (flag == 2 || flag == 3 || flag == 4)
                        {
                            uint objID = packet.ReadDWORD();
                            string name = packet.ReadSTRING("ascii");
                            if (item.typeid2 == 2 && item.typeid3 == 1 && item.typeid4 != 1)
                            {
                                packet.ReadDWORD();
                            }
                            packet.ReadBYTE();
                        }

                        ItemData newitem = item;
                        newitem.Slot = slot;
                        newitem.Amount = 1;
                        newitem.Plus = 0;
                        Globals.char_items.Add(newitem);
                        Globals.Main.itemList.Items.Add(newitem.Name + " +" + newitem.Plus);
                    }
                    else if (item.PK2Name == "ITEM_ETC_TRANS_MONSTER" || item.PK2Name.StartsWith("ITEM_MALL_MAGIC_CUBE"))
                    {
                        packet.ReadDWORD();
                        ItemData newitem = item;
                        newitem.Slot = slot;
                        newitem.Amount = 1;
                        newitem.Plus = 0;
                        Globals.char_items.Add(newitem);
                        Globals.Main.itemList.Items.Add(newitem.Name + " +" + newitem.Plus);
                    }
                    else
                    {
                        ushort amount = packet.ReadWORD();
                        if ((item.PK2Name.Contains("ITEM_ETC_ARCHEMY_ATTRSTONE") || item.PK2Name.Contains("ITEM_ETC_ARCHEMY_MAGICSTONE")) && item.typeid4 != 7 /* not spectial stones*/)
                            packet.ReadBYTE();
                       /* if (item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_DUR") || // some stones that needs extra byte TODO:complete the list
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_HP") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_MP") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_INT") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_STR") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_EVADE_BLOCK") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_ATTRSTONE_PA") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_MAGICSTONE_HR") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_ATTRSTONE_MA") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_ATTRSTONE_MAINT") ||
                            item.PK2Name.StartsWith("ITEM_ETC_ARCHEMY_ATTRSTONE_CRITICAL")
                            )
                            packet.ReadBYTE();*/
                        if (item.typeid3 == 14) // magic pop 
                            packet.ReadBYTE(17);
                        ItemData newitem = item;
                        newitem.Slot = slot;
                        newitem.Amount = amount;
                        newitem.Plus = 0;
                        Globals.char_items.Add(newitem);
                        Globals.Main.itemList.Items.Add(newitem.Name + " +" + newitem.Plus);
                    }
                }
                packet.ReadBYTE(); // avatars (0x05)
                byte avatars = packet.ReadBYTE();
                for (int i = 0; i < avatars; i++)
                {
                    byte slot = packet.ReadBYTE();
                    packet.ReadDWORD(); // 0x00000000
                    uint id = packet.ReadDWORD();
                    byte plus = packet.ReadBYTE();
                    packet.ReadQWORD();
                    packet.ReadDWORD();
                    byte blue = packet.ReadBYTE();
                    for (int ii = 0; ii < blue; ii++)
                    {
                        packet.ReadDWORD();
                        packet.ReadDWORD();
                    }
                    packet.ReadBYTE();
                    byte sockets = packet.ReadBYTE(); // socket stones
                    for (int ii = 0; ii < sockets; ii++)
                    {
                        packet.ReadBYTE(); // count
                        packet.ReadQWORD(); // data
                    }
                    byte adv = packet.ReadBYTE();
                    byte used = packet.ReadBYTE();
                    uint adv_type = 0x00;
                    if (used == 0x01)
                    {
                        packet.ReadBYTE(); // 0x00
                        packet.ReadDWORD(); // ??
                        adv_type = packet.ReadDWORD();
                    }
                }
                #region dafuq is that?!
                byte flag1 = packet.ReadBYTE();
                if (flag1 != 0x00)
                {
                    byte count = packet.ReadBYTE();
                    for (int i = 0; i < count; i++)
                    {
                        packet.ReadBYTE(13);
                    }
                }
                packet.ReadBYTE(); // max job inventory count
                byte count1 = packet.ReadBYTE(); // job inventory count
                for (int i = 0; i < count1; i++)
                {
                    byte slot = packet.ReadBYTE();
                    packet.ReadDWORD(); // 0x00000000
                    uint id = packet.ReadDWORD();
                    byte plus = packet.ReadBYTE();
                    packet.ReadQWORD();
                    packet.ReadDWORD();
                    byte blue = packet.ReadBYTE();
                    for (int ii = 0; ii < blue; ii++)
                    {
                        packet.ReadDWORD();
                        packet.ReadDWORD();
                    }
                    packet.ReadBYTE();
                    byte sockets = packet.ReadBYTE(); // socket stones
                    for (int ii = 0; ii < sockets; ii++)
                    {
                        packet.ReadBYTE(); // count
                        packet.ReadQWORD(); // data
                    }
                    byte adv = packet.ReadBYTE();
                    byte used = packet.ReadBYTE();
                    uint adv_type = 0x00;
                    if (used == 0x01)
                    {
                        packet.ReadBYTE(); // 0x00
                        packet.ReadDWORD(); // ??
                        adv_type = packet.ReadDWORD();
                    }
                }
                packet.ReadBYTE(7);
                #endregion
                byte mastery = packet.ReadBYTE(); // mastery start
                while (mastery == 0x01)
                {
                    packet.ReadDWORD(); // id 
                    packet.ReadBYTE();  // level
                    mastery = packet.ReadBYTE();
                }
                packet.ReadBYTE(); // skills list start / mastery end
                byte skill = packet.ReadBYTE();
                while (skill == 0x01)
                {          
                    uint id = packet.ReadDWORD();
                    packet.ReadBYTE(); // unknown (always 0x01)
                    skill = packet.ReadBYTE();
                    SkillData newskill = BotData.SROSkills[Globals.ModelToStr(id)];
                    Globals.char_skills.Add(id,newskill);
                    Globals.Main.skillList.Items.Add(newskill.Name);
                }
                int index;
                if ((index = IndexOf(packet.GetBytes(), Character.CharID)) != -1)
                {
                    packet.Position = index;
                    uint char_id = packet.ReadDWORD();
                    byte xsec = packet.ReadBYTE();
                    byte ysec = packet.ReadBYTE();
                    float xpos = packet.ReadSINGLE();
                    Character.x = Globals.GameX((int)xsec, (int)xpos);
                    float zpos = packet.ReadSINGLE();
                    float ypos = packet.ReadSINGLE();
                    Character.y = Globals.GameY((int)ysec, (int)ypos);
                    packet.ReadBYTE(24);
                    Character.charName = packet.ReadSTRING("ascii");
                    packet.ReadSTRING("ascii");
                    packet.ReadBYTE(25);
                    Character.AccountID = packet.ReadDWORD();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error in char data");
                }
            }
            catch (Exception ex)
            {
                Globals.UpdateLogs("Character data error");
                byte[] packet_bytes = packet.GetBytes();
                string Date = "[" + DateTime.Now.Day + " -" + DateTime.Now.Month + "] [" + DateTime.Now.Hour + " - " + DateTime.Now.Minute + " ]";
                System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\characterdata_" + Date + ".txt",ex.ToString() + Environment.NewLine + Utility.HexDump(packet_bytes));
            }
        }

        public static void ParseCharStats(Packet packet)
        {
            packet.ReadDWORD();
            packet.ReadDWORD();
            packet.ReadDWORD();
            packet.ReadDWORD();
            packet.ReadWORD();
            packet.ReadWORD();
            packet.ReadWORD();
            packet.ReadWORD();
            Character.max_hp = packet.ReadDWORD();
            Character.max_mp = packet.ReadDWORD();
            Character.str = packet.ReadWORD();
            Character._int = packet.ReadWORD();
        }

        #region IndexOf array
        public static int IndexOf(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}
