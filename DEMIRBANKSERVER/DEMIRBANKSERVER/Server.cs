using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DEMIRBANKLIB;
namespace DEMIRBANKSERVER
{
    public class Server
    {
        public static int PORT = 2525;
        TcpListener listener = new TcpListener(PORT);
        List<Client> clients = new List<Client>();
        public static List<Client> removeclients = new List<Client>();
        Thread baglantiThr,oluClientThr;
        public void startListen()
        {
            listener.Start();
            baglantiThr = new Thread(new ThreadStart(baglantiAl));
            oluClientThr = new Thread(new ThreadStart(oluClientKontrol));
            baglantiThr.Start();
            oluClientThr.Start();
        }
        
        void baglantiAl()
        {
            while (true)
            {
                clients.Add(new Client(listener.AcceptSocket()));
                Thread.Sleep(1000);
            }

        }
        void oluClientKontrol()
        {
            while (true)
            {
                foreach (Client item in removeclients)
                {
                    clients.Remove(item);
                    
                }
                Thread.Sleep(2000);
            }
        }



    }
    
}
