using DEMIRBANKLIB;
namespace DPlugin
{
    public interface DPLUGIN
    {
        public enum EVENTTYPE {GETPACKAGE,SENDPACKAGE,LOADTABLE }
        public string Name { get; set; }   
        public string Description { get; set; }
        public EventManager manager { get; set; }
        public void Run();
    }
    public class EventManager
    {
        IPaket paket;
        public Dictionary<DPLUGIN.EVENTTYPE,EventDelegate> events = new Dictionary<DPLUGIN.EVENTTYPE,EventDelegate>();
        public delegate void EventDelegate(IPaket paket);

        public bool RegisterEvent(DPLUGIN.EVENTTYPE TYPE,EventDelegate del)
        {
            events.Add(TYPE, del);
            return true;
        }
    }
}