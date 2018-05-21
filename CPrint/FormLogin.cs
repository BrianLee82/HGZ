using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPrint
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim()!="666888")
            {
                MessageBox.Show("密码不正确！");
                this.textBox1.Text = "";
                this.textBox1.Focus();
                return;
            }
            else
            {
                this.Close();
            }
        }
    }
}
