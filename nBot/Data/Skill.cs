using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace nBot
{

    struct SkillData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PK2Name { get; set; }
        public int CoolDown { get; set; }
        public int CastTime { get; set; }
        public long MP { get; set; }
        public uint TempID { get; set; }
        public bool Ready { get; set; }
        public System.Threading.Timer timer { get; set; }
    }

    static class SkillDataFunctions
    {
        public static SkillData FindSkill(uint id)
        {
            return BotData.SROSkills.Values.ToList().Find(x => x.ID == Globals.ModelToStr(id));
        }
        public static SkillData FindSkill(string pk2name)
        {
            return BotData.SROSkills.Values.ToList().Find(x => x.PK2Name == pk2name);
        }
    }

    class CoolDown
    {
        public static List<string> Skills_CoolDowned = new List<string>();
        public static void StartCoolDownTimer(uint id)
        {
            try
            {

                if (Globals.char_skills.ContainsKey(id))
                {
                    SkillData skill = Globals.char_skills[id];
                    if (skill.Ready && !Skills_CoolDowned.Contains(skill.Name))
                    {
                        skill.Ready = false;
                        skill.timer = new System.Threading.Timer(new TimerCallback(CoolDownTimer_callback), (object)id, skill.CoolDown, 0);
                        Globals.char_skills[id] = skill; 
                        Skills_CoolDowned.Add(skill.Name);
                        //Globals.UpdateLogs(skill.Name + " " + skill.ID + "  cooldown started!");
                    }
                }
            }
            catch (Exception exx)
            {
                System.Windows.Forms.MessageBox.Show(exx.ToString());
            }
        }
        public static void CoolDownTimer_callback(object e)
        {
            try
            {
                uint s = (uint)e;
                SkillData skill = Globals.char_skills[s];
                skill.Ready = true;
                skill.timer.Dispose();
                skill.timer = null;
                Globals.char_skills[s] = skill;
                Skills_CoolDowned.Remove(skill.Name);
               // Globals.UpdateLogs(skill.Name + " cooldown ended!");
            }
            catch (Exception ex)
            {
               System.Windows.Forms.MessageBox.Show(ex.ToString());
                //throw new Exception("Error",ex);
            }
        }

    }
}
