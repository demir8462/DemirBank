using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEMIRBANKLIB
{
    [Serializable]
    public class GelirGider
    {
        public enum ISLEMTIPI {DUZENLIGELIR,DUZENLIGIDER,TEKSEFERLIKHARCAMA,YATIRMA,CEKME ,DGELIRUPDATE,DGIDERUPDATE,SIL}
        public ISLEMTIPI ITYPE = ISLEMTIPI.DUZENLIGELIR;
        public string miktar="",detay="",id="";
        public DataTable Dgelir = new DataTable(),Dgider = new DataTable(), genel = new DataTable();
        public DateTime tarih = DateTime.Now,eklenmetarih = DateTime.Now, sonrakitarih = DateTime.Now;
    }
}
