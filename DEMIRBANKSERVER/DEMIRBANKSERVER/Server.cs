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
        Task baglantiTask,oluClientTask;
        public void startListen()
        {
            listener.Start();
            baglantiTask = new Task(baglantiAl);
            oluClientTask = new Task(oluClientKontrol);
            baglantiTask.Start();
            oluClientTask.Start();
        }
        
        void baglantiAl()
        {
            object a = new object();

            while (true)
            {
                lock(a)
                {
                    clients.Add(new Client(listener.AcceptSocket()));
                    Thread.Sleep(1000);
                }
            }

        }
        void oluClientKontrol()
        {
            object a = new object();
            while (true)
            {
                foreach (Client item in removeclients)
                {
                    lock(a)
                    {
                        clients.Remove(item);
                    }
                    
                }
                Thread.Sleep(2000);
            }
        }



    }
    
}
