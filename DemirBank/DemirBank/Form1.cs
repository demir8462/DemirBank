using DEMIRBANKLIB;
using System.Net.Sockets;

namespace DemirBank
{
    public partial class Form1 : Form
    {
        public static Client client = new Client();
        User kullanici = new User();
        Main panel = new Main();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kullanici.Tc = textBox1.Text;
            kullanici.Pass = textBox2.Text;
            client.GirisYap(kullanici);
            Hide();
            panel.goster();
            
            // GÝRÝÞ BAÞARILI OLUR ÝSE SUNUCU BÝZE KEY VERECEK ÝÞLEMLERÝ O KEY ÝLE YAPACAÐIZ
        }

        private void button2_Click(object sender, EventArgs e)
        {
            kullanici.Name = textBox4.Text;
            kullanici.Tc = textBox3.Text;
            kullanici.Telno = textBox5.Text;
            kullanici.Adress = textBox6.Text;
            kullanici.Pass = textBox7.Text;
            client.KayitOl(kullanici);
        }
    }
}