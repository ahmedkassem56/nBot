using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace nBot
{
    class TxtFiles
    {
        public static void LoadFiles()
        {
            try
            {
                Globals.UpdateLogs("Loading silkroad data");
                #region parse_items
                string[] items = File.ReadAllLines(Environment.CurrentDirectory + @"\data\parse_items.txt");
                foreach (string item in items)
                {
                    try
                    {
                        if (item.StartsWith("//")) continue;
                        string[] s = item.Split(',');
                        ItemData i = new ItemData();
                        i.ID = s[0];
                        i.PK2Name = s[1];
                        i.Name = s[2];
                        i.Lvl = int.Parse(s[3]);
                        i.typeid1 = byte.Parse(s[6]);
                        i.typeid2 = byte.Parse(s[7]);
                        i.typeid3 = byte.Parse(s[8]);
                        i.typeid4 = byte.Parse(s[9]);
                        BotData.SROItems.Add(i.ID, i);
                    }
                    catch
                    {
                        //Globals.UpdateLogs(item);
                    }
                }
                #endregion

                #region parse_skills
                string[] skills = File.ReadAllLines(Environment.CurrentDirectory + @"\data\parse_skills.txt");
                foreach (string skill in skills)
                {
                    if (skill.StartsWith("//")) continue;
                    string[] s = skill.Split(',');
                    SkillData i = new SkillData();
                    i.ID = s[0];
                    i.PK2Name = s[1];
                    i.Name = s[2];
                    i.CastTime = int.Parse(s[3]);
                    i.CoolDown = int.Parse(s[4]);
                    i.MP = long.Parse(s[5]);
                    i.Ready = true;
                    BotData.SROSkills.Add(i.ID, i);
                }
                #endregion

                #region parse_mobs
                string[] mobs = File.ReadAllLines(Environment.CurrentDirectory + @"\data\parse_mobs.txt");
                foreach (string mob in mobs)
                {
                    if (mob.StartsWith("//")) continue;
                    string[] s = mob.Split(',');
                    Monster i = new Monster();
                    i.ID = s[0];
                    i.PK2Name = s[1];
                    i.Name = s[2];
                    i.Lvl = int.Parse(s[3]);
                    i.HP = long.Parse(s[4]);
                    i.typeid1 = byte.Parse(s[5]);
                    i.typeid2 = byte.Parse(s[6]);
                    i.typeid3 = byte.Parse(s[7]);
                    i.typeid4 = byte.Parse(s[8]);
                    BotData.SROMobs.Add(i.ID, i);
                }
                #endregion

                Globals.LevelData = File.ReadAllLines(@"data\parse_exp.txt");

                Globals.UpdateLogs("Loaded " + BotData.SROItems.Count + " Items & " + BotData.SROSkills.Count + " Skills & " + BotData.SROMobs.Count + " Mobs");
            }
            catch (Exception ex)
            {
                Globals.UpdateLogs("[LoadFiles] Error : " + ex.ToString());
            }
        }
    }
}
