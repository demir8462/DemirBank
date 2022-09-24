using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEMIRBANKLIB;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

namespace DemirBank
{
    public class Client
    {
        string ip="127.0.0.1";
        int port=2525;
        public User kullanici = new User();
        IPaket paket = new IPaket();
        TcpClient client;
        NetworkStream stream;
        BinaryFormatter bf = new BinaryFormatter();
        Thread paketAlThr;
        bool giris;
        public Client()
        {
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                stream.WriteTimeout = 2000;
                
                paketAlThr = new Thread(new ThreadStart(PaketKontrol));
                Main.eventler.Add(Main.EVENTTYPE.GETPACKAGE, Main.DELPackege);
                Main.eventler.Add(Main.EVENTTYPE.SENDPACKAGE, Main.DELPackege);
            }
            catch(Exception e)
            {
                MessageBox.Show("Sunucuya bağlanılamadı !");
            }
        }

        

        public void PaketYolla(IPaket paket)
        {
            try
            {
                Main.eventler[Main.EVENTTYPE.SENDPACKAGE](paket);
                bf.Serialize(stream, paket);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                ClientKapat();
            }
        }
        bool PaketAl()
        {
            try
            {
                paket = (IPaket)bf.Deserialize(stream);
                Main.eventler[Main.EVENTTYPE.GETPACKAGE](paket);
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return (paket != null);
        }
        public void PaketKontrol()
        {
            while (true)
            {
                if(PaketAl())
                {
                    paketIslemci(paket);
                }
                Thread.Sleep(1000);
            }
        }
        public void paketIslemci(IPaket paket)
        {
            if(paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
            {
                MessageBox.Show(paket.detay);
            }
        }
        public bool GirisYap(User user)
        {
            paket.user = user;
            paket.TYPE = IPaket.PAKETTYPE.LOGIN;
            PaketYolla(paket);
            while (true)
            {
                if(PaketAl())
                {
                    if(paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                    {
                        if (paket.CEVAP)
                        {
                            giris = true;
                            return true;
                            
                        }else
                        {
                            MessageBox.Show(paket.detay);
                            return false;
                        }
                        break;
                    }
                    else
                    {
                        MessageBox.Show("WTF BRAH");
                        break;
                    }
                }
                
            }
            // giriş başarılı olursa
            return false;
        }
        public void KayitOl(User user)
        {
            paket.user = user;
            paket.TYPE = IPaket.PAKETTYPE.SIGNUP;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                    {
                        MessageBox.Show(paket.detay);
                        break;
                    }
                    else
                    {
                        MessageBox.Show("WTF BRAH");
                        break;
                    }
                }
            }
        }
        public bool paraYatir(string miktar,string detay)
        {
            paket.TYPE = IPaket.PAKETTYPE.PARAEKLE;
            paket.gelirGider.tarih = DateTime.Now; // PARA YATIRMAK İÇİN GEREKLİ BİLGİLERİ DOLDURUP PAKETİ YOLLADIM
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.detay = detay;
            PaketYolla(paket);
            while (true)
            {
                if(PaketAl())
                {
                    if(paket.TYPE == IPaket.PAKETTYPE.RESPONSE) // CEVAP GELDİ
                    {
                        MessageBox.Show(paket.detay);
                        break;
                    }
                }
            }
            return paket.CEVAP;
        }
        public bool paraCek(string miktar,string detay)
        {
            paket.TYPE = IPaket.PAKETTYPE.PARACEK;
            paket.gelirGider.tarih = DateTime.Now; // PARA YATIRMAK İÇİN GEREKLİ BİLGİLERİ DOLDURUP PAKETİ YOLLADIM
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.detay = detay;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE) // CEVAP GELDİ
                    {
                        MessageBox.Show(paket.detay);
                        veriCek();
                        break;
                    }
                }
            }
            return paket.CEVAP;
        }
        public void veriCek()
        {
            paket.TYPE = IPaket.PAKETTYPE.VERISTEK;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                    break;
            }
        }
        public User GetUser()
        {
            return paket.user;
        }
        public DataTable getGenelTablo()
        {
            return paket.gelirGider.genel;
        }
        public DataTable getDGelirTablo()
        {
            DataView view = paket.gelirGider.Dgelir.DefaultView;
            view.Sort = "SONRAKI";
            return paket.gelirGider.Dgelir;
        }
        public DataTable getDGiderTablo()
        {
            DataView view = paket.gelirGider.Dgider.DefaultView;
            view.Sort = "SONRAKI";
            return paket.gelirGider.Dgider;
        }
        public bool DgelirEkle(DateTime baslangic,DateTime sonrakigelir,string miktar,string detay)
        {
            paket.user = this.paket.user;
            paket.TYPE = IPaket.PAKETTYPE.DUZENLIPARA;
            paket.gelirGider.ITYPE = GelirGider.ISLEMTIPI.DUZENLIGELIR;
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.eklenmetarih = baslangic;
            paket.gelirGider.sonrakitarih = sonrakigelir;
            paket.gelirGider.tarih = DateTime.Now;
            paket.gelirGider.detay = detay;
            paket.CEVAP = false;
            PaketYolla(paket);
            while (true)
            {
                if(PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                    {
                        MessageBox.Show(paket.CEVAP.ToString());
                        break;
                    }
                }
            }
            return true;
        }
        public bool DgelirGuncelle(DateTime baslangic, DateTime sonrakigelir, string miktar, string detay,string ID)
        {
            paket.TYPE = IPaket.PAKETTYPE.DUZENLIPARA;
            paket.gelirGider.ITYPE = GelirGider.ISLEMTIPI.DGELIRUPDATE;
            paket.gelirGider.eklenmetarih = baslangic;
            paket.gelirGider.sonrakitarih = sonrakigelir;
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.detay = detay;
            paket.gelirGider.id = ID;
            paket.CEVAP = false;
            PaketYolla(paket);
            while (true)
            {
                if(PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                        return paket.CEVAP;
                }
            }
        }
        public bool gelirgiderSil(string ID)
        {
            paket.TYPE = IPaket.PAKETTYPE.DUZENLIPARA;
            paket.gelirGider.ITYPE = GelirGider.ISLEMTIPI.SIL;
            paket.gelirGider.id = ID;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                        return paket.CEVAP;
                }
            }
        }
        public void gelirleriKontrolEt(DataTable duzenliGelirler)
        {
            DateTime enson,birdahaki;
            int kacgundebir,miktar;
            string detay;
            for (int i = 0; i <duzenliGelirler.Rows.Count; i++)
            {
                enson = Convert.ToDateTime(duzenliGelirler.Rows[i][3].ToString()); // EN SON NE ZAMAN YATIRILMIŞ BELLEĞE AL
                kacgundebir = Convert.ToInt32(duzenliGelirler.Rows[i][4].ToString());   // KAC GÜNDE BİR YATIRILIYOR
                miktar = int.Parse(duzenliGelirler.Rows[i][1].ToString()); // NE KADAR EKLENİYOR
                detay  = Convert.ToString(duzenliGelirler.Rows[i][2].ToString()); // DETAY
                if((DateTime.Now.Date - enson.Date).Days >= kacgundebir)
                {
                    // kaç kere yatırılmadıysa o kadar kere para ekle hesaba !
                    int carpan = (DateTime.Now.Date - enson.Date).Days / kacgundebir;
                    enson = enson.AddDays(((DateTime.Now.Date - enson.Date).Days / kacgundebir)*kacgundebir);
                    birdahaki = enson.AddDays(kacgundebir);
                    DgelirGuncelle(enson, birdahaki, miktar.ToString(), detay, duzenliGelirler.Rows[i][0].ToString());
                    paraYatir((miktar * carpan).ToString(),detay);
                }
            }
        }
        
        public bool DgiderEkle(DateTime baslangic, DateTime sonrakigelir, string miktar, string detay)
        {
            paket.user = this.paket.user;
            paket.TYPE = IPaket.PAKETTYPE.DUZENLIPARA;
            paket.gelirGider.ITYPE = GelirGider.ISLEMTIPI.DUZENLIGIDER;
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.eklenmetarih = baslangic;
            paket.gelirGider.sonrakitarih = sonrakigelir;
            paket.gelirGider.tarih = DateTime.Now;
            paket.gelirGider.detay = detay;
            paket.CEVAP = false;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                    {
                        MessageBox.Show(paket.CEVAP.ToString());
                        break;
                    }
                }
            }
            return true;
        }
        public bool DgiderGuncelle(DateTime baslangic, DateTime sonrakigider, string miktar, string detay, string ID)
        {
            paket.TYPE = IPaket.PAKETTYPE.DUZENLIPARA;
            paket.gelirGider.ITYPE = GelirGider.ISLEMTIPI.DGIDERUPDATE;
            paket.gelirGider.eklenmetarih = baslangic;
            paket.gelirGider.sonrakitarih = sonrakigider;
            paket.gelirGider.miktar = miktar;
            paket.gelirGider.detay = detay;
            paket.gelirGider.id = ID;
            paket.CEVAP = false;
            PaketYolla(paket);
            while (true)
            {
                if (PaketAl())
                {
                    if (paket.TYPE == IPaket.PAKETTYPE.RESPONSE)
                        return paket.CEVAP;
                }
            }
        }
        public void giderleriKontrolEt(DataTable duzenliGiderler)
        {
            DateTime enson, birdahaki;
            int kacgundebir, miktar;
            string detay;
            for (int i = 0; i < duzenliGiderler.Rows.Count; i++)
            {
                enson = Convert.ToDateTime(duzenliGiderler.Rows[i][3].ToString()); // EN SON NE ZAMAN YATIRILMIŞ BELLEĞE AL
                kacgundebir = Convert.ToInt32(duzenliGiderler.Rows[i][4].ToString());   // KAC GÜNDE BİR YATIRILIYOR
                miktar = int.Parse(duzenliGiderler.Rows[i][1].ToString()); // NE KADAR EKLENİYOR
                detay = Convert.ToString(duzenliGiderler.Rows[i][2].ToString()); // DETAY
                if ((DateTime.Now.Date - enson.Date).Days >= kacgundebir)
                {
                    // kaç kere yatırılmadıysa o kadar kere para ekle hesaba !
                    int carpan = (DateTime.Now.Date - enson.Date).Days / kacgundebir;
                    enson = enson.AddDays(((DateTime.Now.Date - enson.Date).Days / kacgundebir) * kacgundebir);
                    birdahaki = enson.AddDays(kacgundebir);
                    DgelirGuncelle(enson, birdahaki, miktar.ToString(), detay, duzenliGiderler.Rows[i][0].ToString());
                    paraCek((miktar * carpan).ToString(), detay);
                }
            }
        }
        void ClientKapat()
        {
            stream.Close();
            client.Close();
            Environment.Exit(0);
        }
    }
}
