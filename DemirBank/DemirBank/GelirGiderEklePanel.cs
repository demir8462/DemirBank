using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemirBank
{
    public partial class GelirGiderEklePanel : Form
    {
        Client client;
        public GelirGiderEklePanel()
        {
            InitializeComponent();
                
        }

        private void GelirGiderEklePanel_Load(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
            client = Form1.client;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.DgelirEkle(dateTimePicker1.Value,dateTimePicker2.Value,textBox1.Text,textBox2.Text);
        }

        private void GelirGiderEklePanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.DgiderEkle(dateTimePicker1.Value, dateTimePicker2.Value, textBox1.Text, textBox2.Text);
        }
    }
}
