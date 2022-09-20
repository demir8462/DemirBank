using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DEMIRBANKLIB;

namespace DEMIRBANKSERVER
{
    public class dbmanager
    {
        MySqlConnection con = new MySqlConnection("Server=127.0.0.1; Database=demirbank; uid=root; Password=123123;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        DataTable kisiTablo = new DataTable();
        public dbmanager()
        {
            Form1.consoleLogs.Add("Yeni bir dbmanager oluştu !");
        }
        public void Baglan()
        {
            con.Open();
            Form1.consoleLogs.Add("VeriTabanı bağlantısı açılıyor");
        }
        public void Disconnect()
        {
            con.Close();
            Form1.consoleLogs.Add("VeriTabanı bağlantısı kesiliyor");
        }
        public DataTable veriCek(string query,DataTable tablo)
        {
            cmd.Connection = con;
            cmd.CommandText = query;
            adapter.SelectCommand = cmd;
            Baglan();
            Form1.consoleLogs.Add("Veri çekiliyor query :"+query);
            tablo.Clear();
            adapter.Fill(tablo);
            Disconnect();
            return tablo;
        }
        public string getUKey(User user)
        {
            veriCek("SELECT * FROM users WHERE TC='"+user.Tc+"';", kisiTablo);
            return kisiTablo.Rows[0][6].ToString();
        }
        public void getUserByTc(string tc,User user)
        {
            veriCek("SELECT * FROM users WHERE TC='" + tc + "';", kisiTablo);
            user.Name = kisiTablo.Rows[0][1].ToString();
            user.Tc = kisiTablo.Rows[0][2].ToString();
            user.Telno = kisiTablo.Rows[0][3].ToString();
            user.Adress = kisiTablo.Rows[0][4].ToString();
            user.Key = kisiTablo.Rows[0][6].ToString();
            user.Varlık = kisiTablo.Rows[0][7].ToString();
        }
        public void getUserByKey(User user)
        {
            veriCek("SELECT * FROM users WHERE UKEY='" + user.Key + "';", kisiTablo);
            user.Name = kisiTablo.Rows[0][1].ToString();
            user.Tc = kisiTablo.Rows[0][2].ToString();
            user.Telno = kisiTablo.Rows[0][3].ToString();
            user.Adress = kisiTablo.Rows[0][4].ToString();
            user.Varlık = kisiTablo.Rows[0][7].ToString();
        }
        public bool KullaniciVarmi(string tc)
        { 
            kisiTablo.Clear();
            Form1.consoleLogs.Add("Kişi kontrolü yapılıyor : "+tc);
            return (veriCek("SELECT * FROM users WHERE TC='" + tc + "';", kisiTablo).Rows.Count == 1);
        }
        public bool sifreKontrol(string tc,string pass)
        {
            veriCek("SELECT * FROM users WHERE TC='" + tc + "';", kisiTablo);
            return (pass == kisiTablo.Rows[0][5].ToString());
        }
        public bool KayitOl(User user)
        {
            if (KullaniciVarmi(user.Tc))
                return false;
            string cmdtext = "INSERT INTO users (ISIM,TC,TELNO,ADRES,SIFRE,UKEY,VARLIK) VALUES(@ISIM,@TC,@TELNO,@ADRES,@SIFRE,@UKEY,@VARLIK);";
            cmd.CommandText = cmdtext;
            Baglan();
            cmd.Parameters.Clear(); 
            cmd.Parameters.AddWithValue("@ISIM", user.Name);
            cmd.Parameters.AddWithValue("@TC", user.Tc);
            cmd.Parameters.AddWithValue("@TELNO", user.Telno);
            cmd.Parameters.AddWithValue("@ADRES", user.Adress);
            cmd.Parameters.AddWithValue("@SIFRE", user.Pass);
            cmd.Parameters.AddWithValue("@UKEY", MD5Encryption(user.Tc + user.Pass) + ":" + user.Name[0] + user.Adress[0]);
            cmd.Parameters.AddWithValue("@VARLIK",0);
            if (cmd.ExecuteNonQuery() == 1)
            {
                Disconnect();
                tabloOlustur(user.Tc);
                return true;
            }
            else
            {
                Disconnect();
                return false;
            }
        }
        void tabloOlustur(string tc)
        {
            string cmdtext = "CREATE TABLE `g" + tc+ "` ( `ID` INT NOT NULL AUTO_INCREMENT, `MIKTAR` INT NULL,`DETAY` VARCHAR(45) NULL, `ENSON` VARCHAR(45) NULL, `KACGUN` VARCHAR(45) NULL, `SONRAKI` VARCHAR(45) NULL, `TARIH` VARCHAR(45) NULL, `TYPE` VARCHAR(45) NULL, PRIMARY KEY (`ID`));";
            Baglan();
            Form1.consoleLogs.Add("[FROM SGNUP] GelirGider tablosu oluşturuluyor :g"+tc);
            cmd.CommandText = cmdtext;
            cmd.ExecuteNonQuery();
            Disconnect();
        }
        public bool paraEkle(User user, int miktar, DateTime tarih,string detay)
        {
            int para;
            string cmdtext;
            bool don;
            getUserByKey(user);
            para = int.Parse(kisiTablo.Rows[0][7].ToString());
            para += miktar;
            cmdtext = "UPDATE users SET VARLIK='" + para.ToString() + "' WHERE UKEY='"+user.Key+"';";
            cmd.CommandText = cmdtext;
            Baglan();
            don = (cmd.ExecuteNonQuery() == 1);
            Form1.consoleLogs.Add(miktar + " " + user.Name + " Hesabına eklendi : " + don.ToString());
            // ################### işlem tablosuna ekle
            don = paraTablosunaYaz(user,miktar,tarih,"YATIRMA",detay);
            Form1.consoleLogs.Add(miktar + " " + user.Name + " Yatırımlara Eklendi : " + don.ToString());
            Disconnect();
            return don;
        }
        public bool paraCek(User user, int miktar, DateTime tarih,string detay)
        {
            int para;
            string cmdtext;
            bool don;
            getUserByKey(user);
            para = int.Parse(kisiTablo.Rows[0][7].ToString());
            para -= miktar;
            cmdtext = "UPDATE users SET VARLIK='" + para.ToString() + "' WHERE UKEY='" + user.Key + "';";
            cmd.CommandText = cmdtext;
            Baglan();
            don = (cmd.ExecuteNonQuery() == 1);
            Form1.consoleLogs.Add(miktar + " " + user.Name + " Hesabından Çekildi eklendi : " + don.ToString());
            // ################### işlem tablosuna ekle
            don = paraTablosunaYaz(user,miktar,tarih,"CEKME",detay);
            Form1.consoleLogs.Add(miktar + " " + user.Name + " Harcamalarına Eklendi : " + don.ToString());
            Disconnect();
            return don;
        }
        public bool paraTablosunaYaz(User user, int miktar, DateTime tarih,string tip,string detay)
        {
            string cmdtext;
            bool don;
            cmdtext = "INSERT INTO g" + user.Tc + " (MIKTAR,DETAY,ENSON,KACGUN,SONRAKI,TARIH,TYPE) VALUES(@MIKTAR,@DETAY,@ENSON,@KACGUN,@SONRAKI,@TARIH,@TYPE);";
            cmd.CommandText = cmdtext;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@MIKTAR", miktar);
            cmd.Parameters.AddWithValue("@DETAY", detay);
            cmd.Parameters.AddWithValue("@ENSON", "--");
            cmd.Parameters.AddWithValue("@KACGUN", "--");
            cmd.Parameters.AddWithValue("@SONRAKI", "--");
            cmd.Parameters.AddWithValue("@TARIH", tarih);
            cmd.Parameters.AddWithValue("@TYPE", tip);
            don = (cmd.ExecuteNonQuery() == 1);
            Form1.consoleLogs.Add(miktar + " " + user.Name + " Harcamalarına Eklendi : " + don.ToString());
            Disconnect();
            return don;
        }
        // ######################### İŞLEM TABLOLARINI DÖNDÜREN FONKSİYONLAR
        public void getGelirTablo(User user,DataTable tablo)
        {
            veriCek("SELECT * FROM `g" + user.Tc+ "` WHERE TYPE='DGELIR';", tablo);
        }
        public void getGiderTablo(User user, DataTable tablo)
        {
            veriCek("SELECT * FROM `g" + user.Tc + "` WHERE TYPE='DGIDER';", tablo);
        }
        public void getGenelTablo(User user, DataTable tablo)
        {
            veriCek("SELECT * FROM `g" + user.Tc + "` WHERE TYPE='YATIRMA' OR TYPE='CEKME';", tablo);
        }
        public bool DuzenliGelirEkle(User user,GelirGider veri)
        {
            bool don;
            try
            {
                string cmdtext;
                
                cmdtext = "INSERT INTO g" + user.Tc + " (MIKTAR,DETAY,ENSON,KACGUN,SONRAKI,TARIH,TYPE) VALUES(@MIKTAR,@DETAY,@ENSON,@KACGUN,@SONRAKI,@TARIH,@TYPE);";
                cmd.CommandText = cmdtext;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@MIKTAR", veri.miktar);
                cmd.Parameters.AddWithValue("@DETAY", veri.detay);
                cmd.Parameters.AddWithValue("@ENSON", veri.eklenmetarih.ToString());
                cmd.Parameters.AddWithValue("@KACGUN", (veri.sonrakitarih.Date - veri.eklenmetarih.Date).Days);
                cmd.Parameters.AddWithValue("@SONRAKI", veri.sonrakitarih.ToString());
                cmd.Parameters.AddWithValue("@TARIH", veri.tarih.ToString());
                cmd.Parameters.AddWithValue("@TYPE", "DGELIR");
                Baglan();
                don = (cmd.ExecuteNonQuery() == 1);
                Form1.consoleLogs.Add(veri.miktar + " " + user.Name + " düzenli gelir Eklendi : " + don.ToString());
                Disconnect();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
                don = false;
            }
            return don;
        }
        public bool DuzenliGiderEkle(User user, GelirGider veri)
        {
            bool don;
            try
            {
                string cmdtext;

                cmdtext = "INSERT INTO g" + user.Tc + " (MIKTAR,DETAY,ENSON,KACGUN,SONRAKI,TARIH,TYPE) VALUES(@MIKTAR,@DETAY,@ENSON,@KACGUN,@SONRAKI,@TARIH,@TYPE);";
                cmd.CommandText = cmdtext;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@MIKTAR", veri.miktar);
                cmd.Parameters.AddWithValue("@DETAY", veri.detay);
                cmd.Parameters.AddWithValue("@ENSON", veri.eklenmetarih.ToString());
                cmd.Parameters.AddWithValue("@KACGUN", (veri.sonrakitarih.Date - veri.eklenmetarih.Date).Days);
                cmd.Parameters.AddWithValue("@SONRAKI", veri.sonrakitarih.ToString());
                cmd.Parameters.AddWithValue("@TARIH", veri.tarih.ToString());
                cmd.Parameters.AddWithValue("@TYPE", "DGIDER");
                Baglan();
                don = (cmd.ExecuteNonQuery() == 1);
                Form1.consoleLogs.Add(veri.miktar + " " + user.Name + " düzenli gider Eklendi : " + don.ToString());
                Disconnect();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                don = false;
            }
            return don;
        }
        public bool DuzenliGelirUpdate(User user,GelirGider veri)
        {
            bool islem;
            string cmdtext = "UPDATE g" + user.Tc + " SET MIKTAR='" + veri.miktar + "',DETAY='" + veri.detay + "',ENSON='" + veri.eklenmetarih.ToString() + "',KACGUN='" + (veri.sonrakitarih.Date - veri.eklenmetarih.Date).Days + "',SONRAKI='" + veri.sonrakitarih + "',TARIH='" + DateTime.Now + "' WHERE ID='" + veri.id + "';";
            cmd.CommandText = cmdtext;
            Baglan();
            islem = (cmd.ExecuteNonQuery() == 1);
            Disconnect();
            return islem;
        }
     
        public bool gelirgiderSil(User user,string id)
        {
            bool islem;
            string cmdtext = "UPDATE g" + user.Tc + " SET TYPE='IPTAL' WHERE ID='"+id+"';";
            cmd.CommandText = cmdtext;
            Baglan();
            islem = (cmd.ExecuteNonQuery() == 1);
            Disconnect();
            return islem;
        }
        public static string MD5Encryption(string encryptionText)
        {

            // We have created an instance of the MD5CryptoServiceProvider class.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //We converted the data as a parameter to a byte array.
            byte[] array = Encoding.UTF8.GetBytes(encryptionText);
            //We have calculated the hash of the array.
            array = md5.ComputeHash(array);
            //We created a StringBuilder object to store hashed data.
            StringBuilder sb = new StringBuilder();
            //We have converted each byte from string into string type.

            foreach (byte ba in array)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //We returned the hexadecimal string.
            return sb.ToString();
        }

    }
}
    

