using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nBot
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (block_textbox.Text != "")
            {
                block_list.Items.Add(block_textbox.Text);
                block_textbox.Text = "";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (block_list.SelectedItem != null)
            {
                block_list.Items.Remove(block_list.SelectedItem);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listen_textbox.Text != "")
            {
                listen_list.Items.Add(listen_textbox.Text);
                listen_textbox.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listen_list.SelectedItem != null)
            {
                listen_list.Items.Remove(listen_list.SelectedItem);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Globals.Analyzer = this;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }
        public void AddPacket(string data)
        {
            Globals.Analyzer.packets.Text = data + Environment.NewLine + Globals.Analyzer.packets.Text;
        }
    }
}
