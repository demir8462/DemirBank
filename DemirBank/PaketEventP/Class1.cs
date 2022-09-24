using DPlugin;
using DEMIRBANKLIB;
namespace PaketEventP
{
    public class PEP :DPLUGIN
    {
        public string Name { get; set; }    
        public string Description { get; set; }
        public EventManager manager { get; set; }
        public PEP()
        {
            Name = "Paket Eventi Plugin";
            Description = "Paket eventleri ile bir şeyler yapcak";
            manager = new EventManager();
            
        }

        public void Run()
        {
            manager.RegisterEvent(DPLUGIN.EVENTTYPE.SENDPACKAGE, paketYollaEvent);
            manager.RegisterEvent(DPLUGIN.EVENTTYPE.GETPACKAGE, paketAlEvent);
        }
        public void paketAlEvent(IPaket paket)
        {
            MessageBox.Show("PAKET ALINDI :"+paket.detay);
        }
        public void paketYollaEvent(IPaket paket)
        {
            MessageBox.Show("PAKET YOLLANDI :"+paket.detay);
        }
    }
}