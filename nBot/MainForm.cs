using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace nBot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Skill Tab
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                attackSkill.Items.Add(skillList.SelectedItem);
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                attackSkill.Items.Remove(attackSkill.SelectedItem);
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                FirstBuffList.Items.Add(skillList.SelectedItem);
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                SecondBuffList.Items.Add(skillList.SelectedItem);
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FirstBuffList.Items.Remove(FirstBuffList.SelectedItem);
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                SecondBuffList.Items.Remove(SecondBuffList.SelectedItem);
            }
            catch { }
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            tabPage5.Show();
            Globals.Main = this;
            Form2 ss = new Form2();
            Globals.Analyzer = ss;
            TxtFiles.LoadFiles();
            Thread update_configs = new Thread(update);
            update_configs.Start();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new Loader().StartClient();

        }
        public static void update()
        {
            while (true)
            {
                try
                {
                    Globals.Main.charname_label.Text = Character.charName;
                    if (Character.max_hp != 0)
                    {
                        double final_hp = (double.Parse(Character.hp.ToString()) / double.Parse(Character.max_hp.ToString())) * 100;
                        if (final_hp <= 100)
                        {
                            Globals.Main.hp.Value = int.Parse(Math.Round(final_hp).ToString());
                        }
                    }
                    if (Character.max_mp != 0)
                    {
                        double final_mp = (double.Parse(Character.mp.ToString()) / double.Parse(Character.max_mp.ToString())) * 100;
                        if (final_mp <= 100)
                        {
                            Globals.Main.mp.Value = int.Parse(Math.Round(final_mp).ToString());
                        }
                    }
                    if (Character.Level > 0)
                    {
                        double final_exp = (double.Parse(Character.exp.ToString()) / double.Parse(Globals.LevelData[Character.Level - 1].ToString())) * 100;
                        if (final_exp >= 100)
                        {
                            Character.Level++;
                            Character.exp -= ulong.Parse(Globals.LevelData[Character.Level - 2]);
                            final_exp = (double.Parse(Character.exp.ToString()) / double.Parse(Globals.LevelData[Character.Level - 1].ToString())) * 100;
                        }
                        /*else if (final_exp < 0)
                        {
                            Character.Level -= 1;
                            //Character.exp -= ulong.Parse(Globals.LevelData[Character.Level -2]);
                            Character.exp += ulong.Parse(Globals.LevelData[Character.Level - 2]);
                            final_exp = (double.Parse(Character.exp.ToString()) / double.Parse(Globals.LevelData[Character.Level - 1].ToString())) * 100;
                        }*/
                        Globals.Main.exp_progress.Value = int.Parse(Math.Round(final_exp).ToString());

                    }

                    Globals.Main.str_label.Text = Character.str.ToString();
                    Globals.Main.int_label.Text = Character._int.ToString();
                    Globals.Main.x_label.Text = Character.x.ToString();
                    Globals.Main.y_label.Text = Character.y.ToString();
                    Globals.Main.lvl_label.Text = Character.Level.ToString();
                    Globals.Main.zerk_progress.Value = Character.ZerkPoints;
                    Globals.Main.sp_label.Text = Character.sp.ToString();
                    Globals.Main.gold_label.Text = Character.gold.ToString();

                    Thread.Sleep(200);
                }
                catch { }
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Globals.SROClient.Kill();
            }
            catch { }
            Environment.Exit(0);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (BotData.Bot == false)
            {
                BotData.BuffThread = new Thread(new ThreadStart(Logic.Buff.MyLoop));
                BotData.BuffThread.Start();
                BotData.AttackThread = new Thread(new ThreadStart(Logic.Attack.Attack_MainLoop));
                BotData.AttackThread.Start();
                BotData.attack_sw.Start();
                BotData.Bot = true;
                BotData.Attack = true;
                Globals.Main.startBtn.Text = "Stop Bot";
            }
            else
            {
                BotData.AttackThread.Abort();
                BotData.attack_sw.Reset();
                BotData.Bot = false;
                BotData.Attack = false;
                Globals.Main.startBtn.Text = "Start Bot";
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Globals.LoginType = Globals.enumLoginType.Clientless;
                radioButton2.Enabled = false;
                ClientlessGateway gw = new ClientlessGateway();
                gw.Start(Globals.IP, "40510");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Enabled = false;
                Proxy.Start();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Packet p = new Packet(0x6101);
            p.AddBYTE(22); // locale
            p.AddSTRING(id.Text, "ascii");
            p.AddSTRING(pw.Text, "ascii");
            p.AddWORD(Globals.servers_list[servers.Text]); // server ip
            Gateway.SendToServer(p);
        }

        private void Ping_Tick(object sender, EventArgs e)
        {
            if (Globals.LoginType == Globals.enumLoginType.Clientless)
            {
                if (Globals.Server != Globals.ServerEnum.None)
                {
                    Packet response = new Packet(0x2002);
                    if (Globals.Server == Globals.ServerEnum.Gateway)
                        ClientlessGateway.Send(response);
                    else if (Globals.Server == Globals.ServerEnum.Agent)
                        ClientlessAgent.Send(response);
                }
            }
        }

        private void captcha_send_Click(object sender, EventArgs e)
        {
            Captcha.SendCaptcha(captcha_text.Text);
        }

        private void charSelect_Click(object sender, EventArgs e)
        {
            Packet p = new Packet(0x7001);
            p.AddSTRING((string)char_list.SelectedItem, "ascii");
            Agent.SendToServer(p);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Packet packet = new Packet(0x7074);
            packet.AddBYTE(0x01);
            packet.AddBYTE(0x05);
            SkillData skill = Logic.Buff.getSkillData((string)activeBuffs.SelectedItem);
            packet.AddDWORD(Logic.Buff.GetID(skill.ID));
            packet.AddBYTE(0x00);
            Agent.SendToServer(packet);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            /*if (Globals.Server == Globals.ServerEnum.Gateway)
                Gateway.gw_local_client.Close();
            else if (Globals.Server == Globals.ServerEnum.Agent)
                Agent.ag_local_client.Close();*/
            Globals.LoginType = Globals.enumLoginType.Clientless2;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGetCords_Click(object sender, EventArgs e)
        {
            attack_x.Text = Character.x.ToString();
            attack_y.Text = Character.y.ToString();
        }

        private void charspy_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Globals.AroundChars.Values.Any(x => x.Name == charspy_list.Text))
            {
                Character_ c = Globals.AroundChars.Values.ToList().Find(x => x.Name == charspy_list.Text);
                charspy_text.Text = "";
                charspy_text.Text += "Character name:" + c.Name + Environment.NewLine;
                charspy_text.Text += "Guild:" + c.Guild + Environment.NewLine;
                charspy_text.Text += "Grant name:" + c.GrantName + Environment.NewLine;
                charspy_text.Text += "Items:" + Environment.NewLine;
                foreach (Item_ i in c.Items)
                {
                    charspy_text.Text += BotData.SROItems[Globals.ModelToStr(i.ID)].Name + " ("+ BotData.SROItems[Globals.ModelToStr(i.ID)].Lvl + ") Plus: " + i.Plus + Environment.NewLine;
                }
            }
            else
                charspy_text.Text = "Character not found.";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            charspy_list.Items.Clear();
            for (int i = 0;i < Globals.AroundChars.Count();i++)
            {
                charspy_list.Items.Add(Globals.AroundChars.Values.ToArray()[i].Name);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (Globals.AroundMobs.Values.Any(x => x.SpawnType == Monster.MobType.Unique))
            {
                Logic.Attack.SelectMob(Globals.AroundMobs.Values.ToList().Find(x => x.SpawnType == Monster.MobType.Unique).ObjectID);
            }
        }
    }
}
