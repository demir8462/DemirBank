namespace DEMIRBANKSERVER
{
    public partial class Form1 : Form
    {
        Server server = new Server();
        public static List<string> consoleLogs = new List<string>();
        Thread consoleTHR;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            consoleTHR = new Thread(new ThreadStart(ConsoleLogYaz));
            consoleTHR.Start(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.startListen();
            consoleLogs.Add("Sunucu Dinlemeye Baþladý Baðlantýlar Kabul Edilecek.");
        }
        void ConsoleLogYaz()
        {
            while(true)
            {
                try
                {
                    foreach (string item in consoleLogs)
                    {
                        textBox1.AppendText(item);
                        textBox1.AppendText(Environment.NewLine);
                    }
                    consoleLogs.Clear();
                    Thread.Sleep(1000);
                }catch(Exception e)
                {
                    continue;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}