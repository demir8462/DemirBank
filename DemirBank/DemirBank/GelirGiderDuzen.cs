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
    public partial class GelirGiderDuzen : Form
    {
        Client client;
        string ID;
        public GelirGiderDuzen()
        {
            InitializeComponent();
        }

        private void GelirGiderDuzen_Load(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
            client = Form1.client;
        }
        public void Goster(DateTime son,DateTime yeni,string miktar,string ID)
        {
            dateTimePicker1.Value = son;
            dateTimePicker2.Value = yeni;
            textBox1.Text = miktar;
            this.ID = ID;
            Show();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
        }

        private void GelirGiderDuzen_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void button1_Click(object sender, EventArgs e) // PAKETE ID EKLENCEK YOKSA YARRAMI BULURSUN
        {
            if (client.DgelirGuncelle(dateTimePicker1.Value, dateTimePicker2.Value, textBox1.Text, textBox2.Text,ID))
                MessageBox.Show("BAŞARIYLA GÜNCELLENDİ !");
            else
                MessageBox.Show("Sorun Oluştu !");
        }
    }
}
