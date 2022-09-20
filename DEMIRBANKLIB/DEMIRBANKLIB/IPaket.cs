namespace DEMIRBANKLIB
{
    [Serializable]
    public class IPaket
    {
        public enum PAKETTYPE {LOGIN,SIGNUP,RESPONSE,PARAEKLE,PARACEK,VERISTEK,DUZENLIPARA};
        public PAKETTYPE TYPE;
        public User user;
        public bool CEVAP;
        public string detay;
        public GelirGider gelirGider = new GelirGider();
    }
}