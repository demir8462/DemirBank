using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using DEMIRBANKLIB;
namespace DEMIRBANKSERVER
{
    public class Client
    {
        Socket soket;
        NetworkStream stream;
        BinaryFormatter bf = new BinaryFormatter();

        public IPaket paket = new IPaket();

        Thread paketTHR;
        dbmanager manager = new dbmanager();
        public Client(Socket soket)
        {
            this.soket = soket;
            stream = new NetworkStream(soket);
            paketTHR = new Thread(new ThreadStart(paketKontrol));
            paketTHR.Start();
        }
        void paketKontrol()
        {
            while (soket.Connected)
            {
                if(paketAl())
                {
                    try
                    {
                        paketIslemci(paket);
                    }catch(Exception e)
                    {
                        ClientOl();
                    }
                }
                Thread.Sleep(1000);
            }
        }
        bool paketAl()
        {
            try
            {
                paket = (IPaket)bf.Deserialize(stream);
                if (paket != null)
                {
                    Form1.consoleLogs.Add("PAKET GELDİ : " + paket.user.Tc);
                }
            }catch(Exception e)
            {
                ClientOl();
            }
            return (paket != null);
        }
        void paketYolla()
        {
            try
            {
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("PAKET GİDİYOR : " + paket.user.Tc);
            }
            catch (Exception e)
            {
                ClientOl();
            }
        }
        void paketYolla(IPaket paket)
        {
            try
            {
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("PAKET GİDİYOR : " + paket.user.Tc);
            }catch(Exception e)
            {
                ClientOl();
            }
        }
        void ClientOl()
        {
            soket.Close();
            stream.Close();
            
            Server.removeclients.Add(this);
            
            if(paket.user.Tc == null)
                Form1.consoleLogs.Add("Client Düştü : " + "UNSIGNED USER");
            else
                Form1.consoleLogs.Add("Client Düştü : " + paket.user.Tc);
        }
        public void paketIslemci(IPaket paket)
        {
            if(paket.TYPE == IPaket.PAKETTYPE.LOGIN) // GİRİŞ YAPIYOR
            {
                if(manager.KullaniciVarmi(paket.user.Tc))
                {
                    if(manager.sifreKontrol(paket.user.Tc,paket.user.Pass))
                    {
                        paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                        paket.CEVAP = true;
                        paket.detay = "Giriş Başarılı";
                        manager.getUserByTc(paket.user.Tc, paket.user);
                        Form1.consoleLogs.Add(paket.user.Name+" GİRİŞ YAPTI KEY :"+paket.user.Key);

                        // KEY İŞLEMLERİ
                    }else
                    {
                        // kullanıcı var şifre yanlış
                        paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                        paket.CEVAP = false;
                        paket.detay = "Şifre yanlış";
                    }
                }else
                {
                    // böyle kayıtlı değil !
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    paket.CEVAP = false;
                    paket.detay = "Kayıtlı kişi bulunamadı !";
                    Form1.consoleLogs.Add(paket.user.Tc + " Kişi bulunamadı !");
                } 
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("[FROM LOGIN]paket yollanuıyor detay :" + paket.detay);
            }
            else if(paket.TYPE == IPaket.PAKETTYPE.SIGNUP) // KAYIT
            {
                paket.CEVAP = manager.KayitOl(paket.user);
                paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                if (paket.CEVAP)
                    paket.detay = "Kayıt Başarılı , Giriş Yapın !";
                else
                    paket.detay = "Kayıt yapılamadı !";
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("[FROM KAYIT]paket yollanıyor detay :" + paket.detay);
            }else if (paket.TYPE == IPaket.PAKETTYPE.PARAEKLE) // ############################## PARA EKLE
            {
                paket.CEVAP = manager.paraEkle(paket.user, int.Parse(paket.gelirGider.miktar),DateTime.Now,paket.gelirGider.detay);
                paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                if (paket.CEVAP)
                    paket.detay = paket.gelirGider.miktar + " Başarıyla eklendi !";
                else
                    paket.detay = paket.gelirGider.miktar + " Eklenemedi !";
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("[FROM PARAEKLE]paket yollanıyor detay :" + paket.detay);
            }else if(paket.TYPE == IPaket.PAKETTYPE.PARACEK) // ############################# PARA ÇEKME
            {
                paket.CEVAP = manager.paraCek(paket.user, int.Parse(paket.gelirGider.miktar), DateTime.Now,paket.gelirGider.detay);
                paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                if (paket.CEVAP)
                    paket.detay = paket.gelirGider.miktar + " Başarıyla çekildi !";
                else
                    paket.detay = paket.gelirGider.miktar + " Çekilemedi !";
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("[FROM PARAÇEK]paket yollanıyor detay :" + paket.detay);
            }
            else if(paket.TYPE == IPaket.PAKETTYPE.VERISTEK)
            {
                manager.getUserByKey(paket.user);
                manager.getGelirTablo(paket.user, paket.gelirGider.Dgelir);
                manager.getGiderTablo(paket.user, paket.gelirGider.Dgider);
                manager.getGenelTablo(paket.user, paket.gelirGider.genel);
                paket.detay = "Kullanıcının gelir gider varlık verileri";
                bf.Serialize(stream, paket);
                Form1.consoleLogs.Add("[FROM VERI ISTE]paket yollanıyor detay :" + paket.detay);
            }else if(paket.TYPE == IPaket.PAKETTYPE.DUZENLIPARA)
            {
                if(paket.gelirGider.ITYPE == GelirGider.ISLEMTIPI.DUZENLIGELIR) // DÜZENLİ GELİR EKLE
                {
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    paket.CEVAP = manager.DuzenliGelirEkle(paket.user, paket.gelirGider);
                    bf.Serialize(stream, paket);
                    Form1.consoleLogs.Add("[DGELIR]EKLEME YANIT :"+paket.CEVAP.ToString());
                }else if (paket.gelirGider.ITYPE == GelirGider.ISLEMTIPI.DUZENLIGIDER) // DÜZENLİ GELİR EKLE
                {
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    paket.CEVAP = manager.DuzenliGiderEkle(paket.user, paket.gelirGider);
                    bf.Serialize(stream, paket);
                    Form1.consoleLogs.Add("[DGIDER]EKLEME YANIT :" + paket.CEVAP.ToString());
                }
                else if(paket.gelirGider.ITYPE== GelirGider.ISLEMTIPI.DGELIRUPDATE)
                {
                    paket.CEVAP= manager.DuzenliGelirUpdate(paket.user, paket.gelirGider);
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    bf.Serialize(stream, paket);
                    Form1.consoleLogs.Add("[DGELIR GUNCELLEME]:" + paket.CEVAP.ToString());
                }
                else if (paket.gelirGider.ITYPE == GelirGider.ISLEMTIPI.DGIDERUPDATE)
                {
                    paket.CEVAP = manager.DuzenliGelirUpdate(paket.user, paket.gelirGider);
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    bf.Serialize(stream, paket);
                    Form1.consoleLogs.Add("[DGIDER GUNCELLEME]:" + paket.CEVAP.ToString());
                }
                else if(paket.gelirGider.ITYPE == GelirGider.ISLEMTIPI.SIL)
                {
                    paket.CEVAP = manager.gelirgiderSil(paket.user, paket.gelirGider.id);
                    paket.TYPE = IPaket.PAKETTYPE.RESPONSE;
                    bf.Serialize(stream, paket);
                    Form1.consoleLogs.Add("[GELIRGIDER SİL]:" + paket.CEVAP.ToString());
                }
            }
        }
    }
}
