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
    public partial class Main : Form
    {
        Client client;
        DataTable geneltablo,dgelirtablo,dgidertablo;
        GelirGiderEklePanel gelirGiderEklePanel = new GelirGiderEklePanel();
        GelirGiderDuzen duzenlePanel = new GelirGiderDuzen();
        public Main()
        {
            InitializeComponent();
            client = Form1.client;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            client.paraYatir(textBox1.Text,textBox4.Text);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
        }
        public void goster()
        {
            Show();
            client.veriCek();
            label1.Text = "HOS GELDIN " + client.GetUser().Name;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            client.veriCek();
            label3.Text = client.GetUser().Varlık;
            genelTabloDoldur();
            dgelirTabloDoldur();
            dgiderTabloDoldur();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.paraCek(textBox2.Text,textBox3.Text);
        }
        void genelTabloDoldur()
        {
            geneltablo = client.getGenelTablo();
            listView3.Items.Clear();
            for (int i = 0; i < geneltablo.Rows.Count; i++)
            {
                string[] LVI = { geneltablo.Rows[i][0].ToString(), geneltablo.Rows[i][2].ToString(), geneltablo.Rows[i][1].ToString(), geneltablo.Rows[i][5].ToString(), geneltablo.Rows[i][6].ToString() };
                listView3.Items.Add(new ListViewItem(LVI));
            }
        }
        void dgelirTabloDoldur()
        {
            DateTime sonra;
            string kacgun;
            dgelirtablo = client.getDGelirTablo();
            client.gelirleriKontrolEt(dgelirtablo);
            listView1.Items.Clear();
            for (int i = 0; i < dgelirtablo.Rows.Count; i++)
            {
                sonra = Convert.ToDateTime(dgelirtablo.Rows[i][5].ToString());
                kacgun = (sonra.Date - DateTime.Now.Date).Days.ToString();
                string[] LVI = { dgelirtablo.Rows[i][0].ToString() , dgelirtablo.Rows[i][1].ToString(), dgelirtablo.Rows[i][3].ToString(), dgelirtablo.Rows[i][4].ToString(), kacgun, dgelirtablo.Rows[i][5].ToString(), dgelirtablo.Rows[i][2].ToString() };
                listView1.Items.Add(new ListViewItem(LVI));
            }
        }
        void dgiderTabloDoldur()
        {
            DateTime sonra;
            string kacgun;
            dgidertablo = client.getDGiderTablo();
            client.giderleriKontrolEt(dgidertablo);
            listView2.Items.Clear();
            for (int i = 0; i < dgidertablo.Rows.Count; i++)
            {
                sonra = Convert.ToDateTime(dgidertablo.Rows[i][5].ToString());
                kacgun = (sonra.Date - DateTime.Now.Date).Days.ToString();
                string[] LVI = { dgidertablo.Rows[i][0].ToString(), dgidertablo.Rows[i][1].ToString(), dgidertablo.Rows[i][3].ToString(), dgidertablo.Rows[i][4].ToString(), kacgun, dgidertablo.Rows[i][5].ToString(), dgidertablo.Rows[i][2].ToString() };
                listView2.Items.Add(new ListViewItem(LVI));
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1)
            {
                duzenlePanel.Goster(Convert.ToDateTime(listView1.SelectedItems[0].SubItems[2].Text), Convert.ToDateTime(listView1.SelectedItems[0].SubItems[5].Text), listView1.SelectedItems[0].SubItems[1].Text, listView1.SelectedItems[0].SubItems[0].Text);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            gelirGiderEklePanel.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (client.gelirgiderSil(listView1.SelectedItems[0].SubItems[0].Text))
                    MessageBox.Show("Silindi");
                else
                    MessageBox.Show("Sorun var !");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            gelirGiderEklePanel.Show();
        }
    }
}
