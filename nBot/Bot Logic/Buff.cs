using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
namespace nBot.Logic
{
    class Buff
    {
        public static Dictionary<uint, SkillData> ActiveBuffs = new Dictionary<uint, SkillData>();
        public static int delay = 1000;
        public static List<SkillData> ToBuff = new List<SkillData>();
        public static void MyLoop()
        {
            while (true)
            {
                try
                {
                    if (BotData.Bot == true && BotData.Buff == true && (BotData.buff_sw.ElapsedMilliseconds >= delay || !BotData.buff_sw.IsRunning))
                    {
                        BotData.buff_sw.Reset();
                        /* SkillData[] CurrentBuffs = ActiveBuffs.Values.ToArray();
                         for (int i = 0; i < Globals.Main.FirstBuffList.Items.Count; i++)
                         {
                             if (!ContainsName(CurrentBuffs, (string)Globals.Main.FirstBuffList.Items[i]))
                                 ToBuff.Add(getSkillData((string)Globals.Main.FirstBuffList.Items[i]));
                         }*/
                        ToBuff = new List<SkillData>();
                        ToBuff.AddRange(WaitingBuffs());
                        if (ToBuff.Count() == 0)
                        {
                            BotData.Buff = false;
                            BotData.Attack = true;
                        }
                        Globals.UpdateLogs(string.Format("{0} Buffs needs to be buffed", ToBuff.Count()));
                        for (int i = 0; i < ToBuff.Count(); i++)
                        {
                            if (!ToBuff[i].Ready) continue;
                            UseBuff(ToBuff[i]);
                            Globals.UpdateLogs("Trying to buff:" + ToBuff[i].Name);
                            Thread.Sleep(ToBuff[i].CastTime + 200);
                        }
                        ToBuff.Clear();
                        BotData.buff_sw.Start();
                    }
                    System.Threading.Thread.Sleep(10);
                }
                catch { }
            }

        }
        public static List<SkillData> WaitingBuffs()
        {
            SkillData[] CurrentBuffs = ActiveBuffs.Values.ToArray();
            List<SkillData> buffs = new List<SkillData>();
            /*buffs.AddRange(Globals.char_skills.Values.ToList().FindAll(x => Globals.Main.FirstBuffList.Items.Contains(x.Name) && !ContainsName(CurrentBuffs, x.Name) && x.Ready));*/
            for (int i = 0; i < Globals.Main.FirstBuffList.Items.Count; i++)
            {
                if (!ContainsName(CurrentBuffs, (string)Globals.Main.FirstBuffList.Items[i]))   
                {
                    SkillData skill = Globals.char_skills.Values.ToList().Find(x => x.Name == (string)Globals.Main.FirstBuffList.Items[i]);
                    if (skill.Ready)
                        buffs.Add(skill);
                }
            }
            return buffs;
        }
        public static bool ContainsName(SkillData[] array, string name)
        {
            /*for (int i = 0; i < array.Count(); i++)
            {
                if (array[i].Name == name)
                    return true;
            }
            return false;*/
            return array.Any(x => x.Name == name);
        }
        public static SkillData getSkillData(string name)
        {
            var skills = Globals.char_skills.Values.ToList();
            return skills.Find(x => x.Name == name);
        }
        public static void UseBuff(SkillData skill)
        {
            Packet packets = new Packet(0x7074);
            packets.AddBYTE(1);
            packets.AddBYTE(4);
            packets.AddDWORD(GetID(skill.ID));
            packets.AddBYTE(1);
            packets.AddDWORD(Character.ObjectID);
            Agent.SendToServer(packets);
        }
        public static uint GetID(string id)
        {
            return UInt32.Parse(Globals.Inverse(id), System.Globalization.NumberStyles.HexNumber);
        }
    }
}
