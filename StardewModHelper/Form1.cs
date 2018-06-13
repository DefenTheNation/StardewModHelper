using StardewModHelper.Service;
using System;
using System.Windows.Forms;

namespace StardewModHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ItemEnumGenerator gen = new ItemEnumGenerator(InstallPathType.GOG);
            gen.GenerateEnumFromAll();

            Application.Exit();

            button1.Click += Button1_Click;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
