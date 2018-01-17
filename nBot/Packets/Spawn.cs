using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot.Packets
{
    class Spawn
    {
        public static byte action;
        public static ushort count;
        public static List<byte> groupspawn_bytes = new List<byte>();
        public static Packet p;
        public static void GroupSpawn_Start(Packet packet)
        {
            action = packet.ReadBYTE();
            count = packet.ReadWORD();
        }
        public static void GroupSpawnEnd()
        {
            p = new Packet(0x3019, false, false, groupspawn_bytes.ToArray(),false);
            if (action == 0x01)
                GroupSpawn_Parse(p);
            else
            {
                for (int i = 0; i < count; i++)
                    Despawn(p.ReadDWORD());
            }
            action = 0;
            count = 0;
            groupspawn_bytes = new List<byte>();
            p = null;
        }
        public static void GroupSpawn(Packet packet)
        {
            groupspawn_bytes.AddRange(packet.GetBytes());
        }
        public static void SingleSpawn(Packet packet)
        {
            Spawn_Parse(packet);
        }
        public static void GroupSpawn_Parse(Packet packet)
        {
            if (action == 0x01)
            {
                for (int i = 0; i < count; i++)
                {
                    Spawn_Parse(packet);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    uint objID = packet.ReadDWORD();
                    Despawn(objID);
                }
            }
        }
        public static void Spawn_Parse(Packet packet)
        {
            uint model = packet.ReadDWORD();
            string id = Globals.ModelToStr(model);
            if (BotData.SROMobs.ContainsKey(id))
            {
                Monster mob = BotData.SROMobs[id];
                if (mob.PK2Name.Contains("MOB_"))
                {
                    Spawn_Monster(packet, model);
                }
                else if (mob.PK2Name.Contains("_GATE"))
                {
                    Spawn_Portal(packet, model);
                }
                else if (mob.PK2Name.Contains("NPC_"))
                {
                    Spawn_NPC(packet, model);
                }
                else if (mob.PK2Name.Contains("CHAR_"))
                {
                    Spawn_Char(packet, model);
                }
                else if (mob.PK2Name.Contains("COS_"))
                {
                    Spawn_Pet(packet, model);
                }
            }
            else if (BotData.SROItems.ContainsKey(id))
            {
                Spawn_Item(packet, model);
            }
            else
            {
                Globals.UpdateLogs("Group/single spawn error");
                byte[] packet_bytes = packet.GetBytes();
                string Date = "[" + DateTime.Now.Day + " -" + DateTime.Now.Month + "] [" + DateTime.Now.Hour + " - " + DateTime.Now.Minute + " ]";
                System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\groupspawn_" + Date + ".txt",Utility.HexDump(packet_bytes));
            }
        }
        public static void SingleDespawn(Packet packet)
        {
            uint objID = packet.ReadDWORD();
            Despawn(objID);
        }
        public static void Despawn(uint objID)
        {
            if (Globals.AroundMobs.ContainsKey(objID))
                Globals.AroundMobs.Remove(objID);
            else if (Globals.AroundChars.ContainsKey(objID))
                Globals.AroundChars.Remove(objID);
            else if (Globals.AroundDrops.ContainsKey(objID))
                Globals.AroundDrops.Remove(objID);
        }
        #region Monster
        public static void Spawn_Monster(Packet packet, uint model)
        {
            Monster mob = BotData.SROMobs[Globals.ModelToStr(model)];
            uint objID = packet.ReadDWORD();
            mob.ObjectID = objID;
            byte xsec = packet.ReadBYTE(); // x sec
            byte ysec = packet.ReadBYTE(); // y sec
            float xpos = packet.ReadSINGLE(); // x 
            float zpos = packet.ReadSINGLE(); // z 
            float ypos = packet.ReadSINGLE(); // y
            packet.ReadWORD(); // angle
            byte has_destination = packet.ReadBYTE();
            packet.ReadBYTE(); // walk flag
            if (has_destination == 0x01)
            {
                xsec = packet.ReadBYTE(); // x sector 
                ysec = packet.ReadBYTE(); // y sector
                if (ysec == 0x80)
                {
                    xpos = packet.ReadDWORD();
                    packet.ReadDWORD();
                    ypos = packet.ReadDWORD();
                }
                xpos = packet.ReadWORD(); // x position
                zpos = packet.ReadWORD(); // z position 
                ypos = packet.ReadWORD(); // y position
            }
            else
            {
                packet.ReadBYTE(); // no destination
                packet.ReadWORD(); // angle
            }
            byte DeathFlag = packet.ReadBYTE();
            mob.DeathFlag = DeathFlag;
            packet.ReadBYTE(3);
            packet.ReadSINGLE();
            packet.ReadSINGLE();
            packet.ReadSINGLE();
            byte buffs = packet.ReadBYTE();
            for (int i = 0; i < buffs; i++)
            {
                packet.ReadDWORD();
                packet.ReadDWORD();
            }
            packet.ReadBYTE(3);
            byte mobType = packet.ReadBYTE();
            mob.SpawnType = GetMobType(mobType);
            mob.x = Globals.GameX((int)xsec, (int)xpos);
            mob.y = Globals.GameY((int)ysec, (int)ypos);
            int dist = Math.Abs((mob.x - Character.x)) + Math.Abs((mob.y - Character.y));
            mob.Distance = dist;
            Globals.AroundMobs.Add(mob.ObjectID, mob);
            if (Globals.Main.autouniques.Checked && mob.SpawnType == Monster.MobType.Unique)
            {
                Logic.Attack.SelectMob(mob.ObjectID);
            }
            //Globals.UpdateLogs(string.Format("Mob:{0} ({1}) Spawned", mob.Name, mob.SpawnType.ToString()));
        }
        public static Monster.MobType GetMobType(byte monsterType)
        {
            switch (monsterType)
            {
                case 0x00:
                    return Monster.MobType.General;
                case 0x01:
                    return Monster.MobType.Champion;
                case 0x03:
                    return Monster.MobType.Unique;
                case 0x04:
                    return Monster.MobType.Giant;
                case 0x05:
                    return Monster.MobType.Titan;
                case 0x06:
                    return Monster.MobType.Elite;
                case 0x10:
                    return Monster.MobType.General_Party;
                case 0x11:
                    return Monster.MobType.Champion_Party;
                case 0x14:
                    return Monster.MobType.Giant_Party;
            }
            return new Monster.MobType();
        }
        #endregion
        #region Portal
        public static void Spawn_Portal(Packet packet, uint model)
        {
            uint UniqueID = packet.ReadDWORD();
            packet.ReadBYTE();
            packet.ReadBYTE();
            packet.ReadSINGLE();
            packet.ReadSINGLE();
            packet.ReadSINGLE();
            packet.ReadWORD();
            packet.ReadDWORD();
            packet.ReadQWORD();
            //Globals.UpdateLogs(string.Format("Portal:{0} Spawned", BotData.SROMobs[Globals.ModelToStr(model)].PK2Name));
        }
        #endregion
        #region NPC
        public static void Spawn_NPC(Packet packet, uint model)
        {
            uint UniqueID = packet.ReadDWORD();
            byte xsec = packet.ReadBYTE();
            byte ysec = packet.ReadBYTE();
            float xcoord = packet.ReadSINGLE();
            packet.ReadSINGLE();
            float ycoord = packet.ReadSINGLE();

            packet.ReadWORD(); // Position
            byte move = packet.ReadBYTE(); // Moving
            packet.ReadBYTE(); // Running

            if (move == 1)
            {
                xsec = packet.ReadBYTE();
                ysec = packet.ReadBYTE();
                if (ysec == 0x80)
                {
                    xcoord = packet.ReadDWORD();
                    packet.ReadDWORD();
                    ycoord = packet.ReadDWORD();
                }
                else
                {
                    xcoord = packet.ReadWORD();
                    packet.ReadWORD();
                    ycoord = packet.ReadWORD();
                }
            }
            else
            {
                packet.ReadBYTE(); // Unknown
                packet.ReadWORD(); // Unknwon
            }

            packet.ReadBYTE(16); // ??
            packet.ReadBYTE(); // ?? (0x00)
            try
            {
                uint id = packet.ReadDWORD(); // this will decide if the NPC got extra bytes or not by reading the next dword,if its new spawn packet then the npc don't need extra parsing
                if (BotData.SROMobs.ContainsKey(Globals.ModelToStr(id)) || BotData.SROItems.ContainsKey(Globals.ModelToStr(id)))
                {
                    packet.Position -= 4;
                }
                else
                {
                    packet.Position -= 4;
                    byte check = packet.ReadBYTE();
                    if (check != 0)
                    {
                        byte count = packet.ReadBYTE();
                        for (byte i = 0; i < count; i++)
                        {
                            packet.ReadBYTE();
                        }
                    }
                }
            }
            catch { }
           // Globals.UpdateLogs(string.Format("NPC:{0} Spawned", BotData.SROMobs[Globals.ModelToStr(model)].PK2Name));
        }
        #endregion
        #region Character
        public static void Spawn_Char(Packet packet, uint model)
        {
            Character_ character = new Character_();
            byte trade = 0;
            byte stall = 0;
            packet.ReadBYTE(); // Volume/Height
            packet.ReadBYTE(); // Rank
            packet.ReadBYTE(); // Icons
            packet.ReadBYTE(); // Unknown
            packet.ReadBYTE(); // Max Slots
            byte items_count = packet.ReadBYTE();
            character.Items = new List<Item_>(items_count);
            for (byte i = 0; i < items_count; i++)
            {
                uint itemid = packet.ReadDWORD();
                ItemData item = BotData.SROItems[Globals.ModelToStr(itemid)];
                string type = item.PK2Name;
                if (type.StartsWith("ITEM_CH") || type.StartsWith("ITEM_EU") || type.StartsWith("ITEM_FORT") || type.StartsWith("ITEM_ROC_CH") || type.StartsWith("ITEM_ROC_EU"))
                {
                    byte plus = packet.ReadBYTE(); // Item Plus
                    character.Items.Add(new Item_ { ID = itemid, Plus = plus }); 
                }
                if (type.Contains("_TRADE_TRADER_") || type.Contains("_TRADE_HUNTER_") || type.Contains("_TRADE_THIEF_"))
                {
                    trade = 1;
                }
            }
            packet.ReadBYTE(); // Max Avatars Slot
            byte avatar_count = packet.ReadBYTE();
            for (byte i = 0; i < avatar_count; i++)
            {
                packet.ReadDWORD(); // Avatar ID
                packet.ReadBYTE(); // Avatar Plus
            }
            int mask = packet.ReadBYTE();
            if (mask == 1)
            {
                uint id = packet.ReadDWORD();
                string type = BotData.SROMobs[Globals.ModelToStr(id)].PK2Name;
                if (type.StartsWith("CHAR"))
                {
                    packet.ReadBYTE();
                    byte count = packet.ReadBYTE();
                    for (int i = 0; i < count; i++)
                    {
                        packet.ReadDWORD();
                    }
                }
            }

            uint UniqueID = packet.ReadDWORD();
            character.objID = UniqueID;
            byte xsec = packet.ReadBYTE();
            byte ysec = packet.ReadBYTE();
            float xcoord = packet.ReadSINGLE();
            packet.ReadSINGLE();
            float ycoord = packet.ReadSINGLE();

            packet.ReadWORD(); // Position
            byte move = packet.ReadBYTE(); // Moving
            packet.ReadBYTE(); // Running

            if (move == 1)
            {
                xsec = packet.ReadBYTE();
                ysec = packet.ReadBYTE();
                if (ysec == 0x80)
                {
                    xcoord = packet.ReadDWORD();
                    packet.ReadDWORD();
                    ycoord = packet.ReadDWORD();
                }
                else
                {
                    xcoord = packet.ReadWORD();
                    packet.ReadWORD();
                    ycoord = packet.ReadWORD();
                }
            }
            else
            {
                packet.ReadBYTE(); // No Destination
                packet.ReadWORD(); // Angle
            }

            packet.ReadBYTE(); // Alive
            packet.ReadBYTE(); // Unknown
            packet.ReadBYTE(); // Unknown
            packet.ReadBYTE(); // Unknown

            packet.ReadSINGLE(); // Walking speed
            packet.ReadSINGLE(); // Running speed
            packet.ReadSINGLE(); // Berserk speed

            byte active_buffs = packet.ReadBYTE(); // Buffs count
            for (int a = 0; a < active_buffs; a++)
            {
                uint skillid = packet.ReadDWORD();
                string type1 = "";
                try
                {
                    type1 = BotData.SROSkills[Globals.ModelToStr(skillid)].PK2Name;
                }
                catch { }
                packet.ReadDWORD(); // Temp ID
                if (type1.StartsWith("SKILL_EU_CLERIC_RECOVERYA_GROUP") || type1.StartsWith("SKILL_EU_BARD_BATTLAA_GUARD") || type1.StartsWith("SKILL_EU_BARD_DANCEA") || type1.StartsWith("SKILL_EU_BARD_SPEEDUPA_HITRATE"))
                {
                    packet.ReadBYTE();
                }
            }
            string name = packet.ReadSTRING("ascii");
            character.Name = name;
            if (trade == 1)
            {
                packet.ReadQWORD();
                packet.ReadSTRING("ascii");
            }
            else
            {
                packet.ReadBYTE(); // Unknown
                byte cnt = packet.ReadBYTE();
                if (cnt == 1)
                {
                    packet.ReadDWORD(); // Unknown
                }
                packet.ReadBYTE(); // Job type
                packet.ReadBYTE(); // Job level
                stall = packet.ReadBYTE(); // Stall flag
                character.Guild = packet.ReadSTRING("ascii"); // Guild
                packet.ReadDWORD(); // Guild ID
                character.GrantName = packet.ReadSTRING("ascii"); // Grant Name
                packet.ReadDWORD();
                packet.ReadDWORD();
                packet.ReadDWORD();
                packet.ReadWORD();
                if (stall == 4)
                {
                    packet.ReadSTRING("ascii"); // Stall Name
                    packet.ReadDWORD(); // Unknown
                }
            }
            packet.ReadWORD(); // 00 and PK Flag (0xFF)
            Globals.AroundChars.Add(character.objID,character);
           // Globals.UpdateLogs(string.Format("Char:{0} Spawned", character.Name));
        }
        #endregion
        #region Pet
        public static void Spawn_Pet(Packet packet, uint model)
        {
            uint pet_id = packet.ReadDWORD(); // PET ID
            byte xsec = packet.ReadBYTE();
            byte ysec = packet.ReadBYTE();
            float xcoord = packet.ReadSINGLE();
            packet.ReadSINGLE();
            float ycoord = packet.ReadSINGLE();

            packet.ReadWORD(); // Position
            byte move = packet.ReadBYTE(); // Moving
            packet.ReadBYTE(); // Running

            if (move == 1)
            {
                xsec = packet.ReadBYTE();
                ysec = packet.ReadBYTE();
                if (ysec == 0x80)
                {
                    xcoord = packet.ReadDWORD();
                    packet.ReadDWORD();
                    ycoord = packet.ReadDWORD();
                }
                else
                {
                    xcoord = packet.ReadWORD();
                    packet.ReadWORD();
                    ycoord = packet.ReadWORD();
                }
            }
            else
            {
                packet.ReadBYTE(); // Unknown
                packet.ReadWORD(); // Unknwon
            }
            packet.ReadBYTE();
            packet.ReadBYTE();
            packet.ReadBYTE();
            packet.ReadBYTE();
            //packet.ReadBYTE();
            packet.ReadSINGLE();
            float speed = packet.ReadSINGLE();
            packet.ReadSINGLE();
            packet.ReadWORD();
            Monster mob = BotData.SROMobs[Globals.ModelToStr(model)];
            string type = mob.PK2Name;

            if (type.StartsWith("COS_C") || type.StartsWith("COS_T_DHORSE"))
            {
                //Do Nothing
            }
            else if (mob.typeid2 == 2 && mob.typeid3 == 3 && mob.typeid4 == 4)
            {
                packet.ReadSTRING("ascii"); //Owner Name
                packet.ReadSTRING("ascii"); //Pet Name
                packet.ReadBYTE(); //Unknown ( Always ?? 4 )
                packet.ReadDWORD(); //Looks like Pet Owner ID
            }
            else if (mob.typeid2 == 2 && mob.typeid3 == 3 && mob.typeid4 == 3)
            {
                packet.ReadSTRING("ascii"); //Owner Name
                packet.ReadSTRING("ascii"); //Pet Name
                packet.ReadBYTE(); //Unknown ( Always ?? 4 )
                packet.ReadBYTE();
                packet.ReadDWORD(); //Looks like Pet Owner ID
                try
                {
                    byte b1 = packet.ReadBYTE();
                    byte b2 = packet.ReadBYTE();
                    byte b3 = packet.ReadBYTE();
                    byte b4 = packet.ReadBYTE();
                    if (b1 == 255 && b2 == 255 & b3 == 255 && b4 == 255)
                    {
                        packet.ReadWORD();
                    }
                    else
                    {
                        packet.Position -= 4;
                    }
                }
                catch { }
            }
            /*else if (type.Contains("COS_T"))
            {
                packet.ReadSTRING("ascii"); //Owner Name
                packet.ReadBYTE();
                packet.ReadBYTE();
                packet.ReadDWORD();
                packet.ReadBYTE();
            }*/
            else if (type.StartsWith("TRADE_COS_QUEST_TRADE"))
            {
                packet.ReadSTRING("ascii"); //Owner Name
                packet.ReadWORD(); //Unknwon
                packet.ReadDWORD(); //Owner ID
            }
            else
            {
                packet.ReadSTRING("ascii"); //Owner Name
                packet.ReadBYTE();
                packet.ReadBYTE();
                packet.ReadDWORD();
            }
            //Globals.UpdateLogs(string.Format("Pet:{0} Spawned", BotData.SROMobs[Globals.ModelToStr(model)].Name));
        }
        #endregion
        #region Item
        public static void Spawn_Item(Packet packet, uint model)
        {
            string type = BotData.SROItems[Globals.ModelToStr(model)].PK2Name;
            if (type.StartsWith("ITEM_ETC_GOLD"))
            {
                packet.ReadDWORD(); // Ammount
            }
            if (type.StartsWith("ITEM_QSP") || type.StartsWith("ITEM_ETC_E090825") || type.StartsWith("ITEM_QNO") || type == "ITEM_TRADE_SPECIAL_BOX")
            {
                packet.ReadSTRING("ascii"); // Name
            }
            if (type.StartsWith("ITEM_CH") || type.StartsWith("ITEM_EU"))
            {
                packet.ReadBYTE(); // Plus
            }
            uint id = packet.ReadDWORD(); // ID
            packet.ReadBYTE(); //XSEC
            packet.ReadBYTE(); //YSEC
            packet.ReadSINGLE(); //X
            packet.ReadSINGLE(); //Z
            packet.ReadSINGLE(); //Y
            packet.ReadWORD(); //POS
            byte owner = packet.ReadBYTE();
            if (owner == 1) // Owner exist
            {
                uint owner_id = packet.ReadDWORD();
                if (owner_id == Character.AccountID) // Owner ID
                {
                    Globals.AroundDrops.Add(id, new Drop { ID = model, objID = id });
                }
            }
            packet.ReadBYTE(); // Item Blued
            //Globals.UpdateLogs("Drop:" + type + " Spawned");
        }
        #endregion

    }
}
