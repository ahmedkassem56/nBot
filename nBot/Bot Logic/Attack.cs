using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace nBot.Logic
{
    class Attack
    {
        public static bool casting = false;
        public static bool should_cast = false;
        public static Timer cast_t;
        public static Timer castresponse_t;
        public static bool wait = false;
        public static Timer wait_t;
        public static void Attack_MainLoop()
        {
            while (true)
            {
                if (BotData.Bot == true && BotData.Attack == true && BotData.attack_sw.ElapsedMilliseconds >= 200)
                {
                    BotData.attack_sw.Reset();
                    if (Buff.WaitingBuffs().Count() > 0)
                    {
                        BotData.Attack = false;
                        BotData.Buff = true;
                    }
                    if (BotData.Bot == true && BotData.Attack == true)
                    {
                        List<Monster> mobsAround = new List<Monster>(Globals.AroundMobs.Values.ToList());
                        List<Monster> InRange = new List<Monster>();
                        InRange.AddRange(mobsAround.FindAll(m => DistanceFromCenter(m.x, m.y) <= int.Parse(Globals.Main.attack_range.Text)));
                        if (InRange.Count() > 0)
                        {
                            casting = false;
                            should_cast = false;
                            wait = false;
                            Monster nearest = GetNearest(InRange);
                            Thread killit = new Thread(KillMob);
                            killit.Start(nearest.ObjectID);
                            killit.Join();
                            InRange.Clear();
                        }
                        else
                        {
                            //Globals.UpdateLogs("No mobs in range");
                            Thread.Sleep(50);
                        }
                    }
                    BotData.attack_sw.Start();
                }
                Thread.Sleep(10);
            }
        }
        public static Monster GetNearest(List<Monster> mobs)
        {
            Monster nearest = new Monster();
            for (int i = 0; i < mobs.Count(); i++)
            {
                if (i == 0)
                {
                    nearest = mobs[i];
                    continue;
                }
                if (DistanceFromChar(mobs[i]) < DistanceFromChar(nearest))
                {
                    nearest = mobs[i];
                }
            }
            return nearest;
        }
        public static int DistanceFromCenter(int mob_x, int mob_y)
        {
            return Math.Abs((mob_x - int.Parse(Globals.Main.attack_x.Text))) + Math.Abs((mob_y - int.Parse(Globals.Main.attack_y.Text)));
        }
        public static int DistanceFromChar(Monster mob)
        {
            return Math.Abs((mob.x - Character.x)) + Math.Abs((mob.y - Character.y));
        }
        public static void KillMob(object e)
        {
            uint objectID = (uint)e;
            while (Globals.AroundMobs.ContainsKey(objectID))
            {
                if (BotData.Attack == true && casting == false && should_cast == false && !wait)
                {
                    SelectMob(objectID);
                    SkillData skill = getFirstSkill();
                    if (skill.ID != null)
                    {
                        Globals.UpdateLogs("Trying to cast:" + skill.Name);
                        wait = true;
                        wait_t = new Timer(new TimerCallback(wait_Tick), null, 2000, Timeout.Infinite);
                        CastSkill(Logic.Buff.GetID(skill.ID), objectID);
                    }
                    else
                    {
                        NormalAttack(objectID);
                        wait = true;
                        wait_t = new Timer(new TimerCallback(wait_Tick), null, 1500, Timeout.Infinite);
                    }
                    if (Buff.WaitingBuffs().Count() > 0)
                    {
                        BotData.Attack = false;
                        BotData.Buff = true;
                    }
                } 
                Thread.Sleep(10); 
            }
        }
        public static void wait_Tick(object e)
        {
            wait = false;
            wait_t = null;
        }
        public static SkillData getFirstSkill()
        {
           // return Globals.char_skills.Values.ToList().Find(x => Globals.Main.attackSkill.Items.Contains(x.Name) && x.Ready && Character.mp >= x.MP);
            for (int i = 0; i < Globals.Main.attackSkill.Items.Count; i++)
            {
                SkillData skill = Globals.char_skills.Values.ToList().Find(x => x.Name == (string)Globals.Main.attackSkill.Items[i]);
                if (skill.ID != null && skill.Ready && Character.mp >= skill.MP)
                {
                    return skill;
                }
            }
            return new SkillData();
        }
        public static void SelectMob(uint id)
        {
            Packet packet = new Packet(0x7045);
            packet.AddDWORD(id);
            Agent.SendToServer(packet);
        }
        public static void CastSkill(uint skillID, uint mobID)
        {
            Packet packet = new Packet(0x7074);
            packet.AddBYTE(0x01);
            packet.AddBYTE(0x04);
            packet.AddDWORD(skillID);
            packet.AddBYTE(0x01);
            packet.AddDWORD(mobID);
            Agent.SendToServer(packet);
        }
        public static void NormalAttack(uint mobID)
        {
            Packet packet = new Packet(0x7074);
            packet.AddBYTE(1);
            packet.AddBYTE(1);
            packet.AddBYTE(1);
            packet.AddDWORD(mobID);
            Agent.SendToServer(packet);
        }
        public static void HandleSkillCasting(Packet packet)
        {
            try
            {
                if (packet.ReadBYTE() == 1)
                {
                    byte flag1 = packet.ReadBYTE();
                    byte flag2 = packet.ReadBYTE();
                    uint skill = packet.ReadDWORD();
                    uint owner = packet.ReadDWORD();
                    if (owner == Character.ObjectID)
                    {
                        CoolDown.StartCoolDownTimer(skill);
                    }
                    if (flag1 == 2)
                    {
                        if (flag2 == 0x30)
                        {  
                            if (owner == Character.ObjectID)
                            {
                                SkillData s = BotData.SROSkills[Globals.ModelToStr(skill)];
                                if (Globals.Main.attackSkill.Items.Contains(s.Name) && BotData.Attack == true)
                                {
                                    casting = true;
                                    cast_t = new Timer(new TimerCallback(cast_t_Tick), null, s.CastTime, Timeout.Infinite);
                                    Globals.UpdateLogs("Casting skill:" + s.Name);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        public static void cast_t_Tick(object e)
        {
            casting = false;
            should_cast = false;
            wait = false;
            cast_t = null;
        }
        public static void HandleSelectResponse(Packet packet)
        {
            byte success = packet.ReadBYTE();
            if (success == 1)
            {
                uint id = packet.ReadDWORD();
                if (Globals.AroundMobs.ContainsKey(id))
                {
                    byte check = packet.ReadBYTE();
                    if (check == 1)
                    {
                        uint hp = packet.ReadDWORD();
                        if (hp <= 0)
                        {
                            Globals.AroundMobs.Remove(id);
                        }
                    }
                }
            }
        }
        public static void HandleCastSkillResponse(Packet packet)
        {
            byte flag1 = packet.ReadBYTE();
            if (flag1 == 0x01)
            {
                byte flag2 = packet.ReadBYTE();
                if (flag2 == 0x01)
                {
                    should_cast = true;
                    castresponse_t = new Timer(new TimerCallback(castresponse_Tick), null, 3000, Timeout.Infinite);
                }
            }
        }
        public static void castresponse_Tick(object e)
        {
            should_cast = false;
            wait = false;
            castresponse_t = null;
        }
    }
}
