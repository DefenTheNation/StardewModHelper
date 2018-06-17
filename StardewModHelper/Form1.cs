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

            Load += Form1_Load;
            button1.Click += Button1_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ItemEnumGenerator gen = new ItemEnumGenerator(InstallPathType.GOG);
            gen.GenerateEnumFromAll();

            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Fuck it
        }
    }
}
