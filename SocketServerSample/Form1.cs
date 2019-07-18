using SocketServerSample.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServerSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse("192.168.11.3"), 8888);
            TcpListener listener = new TcpListener(ipAdd);
            listener.Start(0);
            Console.WriteLine("Port:8888のListenを開始しました。");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("クライアントが接続しました。");

            if (client.Connected)
            {
                listener.Stop();
                NetworkStream netStream = client.GetStream();
                StreamReader sReader = new StreamReader(netStream, Encoding.UTF8);

                string str = String.Empty;

                do
                {
                    str = sReader.ReadLine();
                    if (null == str)
                    {
                        break;
                    }
                    Console.WriteLine(str);
                } while (!str.Equals("quit"));
                sReader.Close();
                client.Close();
            }
            Console.WriteLine("終了するには、Enterキーを押してください");
            Console.ReadLine();
        }
    }
}
