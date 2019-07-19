using SocketServerSample.util;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private string logFilePath = @"./SocketServerSample.log";

        private void Form1_Load(object sender, EventArgs e)
        {
            // .Net Framework 4.0
            Task.Run(new Action(method));

            Task.Run(() =>
            {
                Log.i("Thread 2 start");
            });
            // .Net Framework 1.1
            //Thread thread1 = new Thread(new ThreadStart(method));


        }

        private void method()
        {
            Log.i("Thread 1 start");

            //IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse("192.168.56.101"), 8888);
            IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 8888);
            TcpListener listener = new TcpListener(ipAdd);
            listener.Start(0);
            File.AppendAllText(logFilePath, Log.i("Port:8888のListenを開始しました。"));

            while (true)
            {
                // 接続要求あるかどうか確認
                if (listener.Pending())
                {
                    // 接続要求を処理する
                    File.AppendAllText(logFilePath, Log.i("接続待機開始"));
                    var client = listener.AcceptTcpClient();
                    File.AppendAllText(logFilePath, Log.i("クライアントが接続しました。"));
                    //sessions.Add(new Session(client, SessionCommandExec));
                    //Console.WriteLine("AcceptTcpClient : {0}", client.Client.RemoteEndPoint);

                    NetworkStream netStream = client.GetStream();
                    StreamReader sReader = new StreamReader(netStream, Encoding.UTF8);
                    string str = sReader.ReadLine();
                    File.AppendAllText(logFilePath, Log.i(str));

                    sReader.Close();
                    File.AppendAllText(logFilePath, Log.i("ストリーム クローズ"));
                    //client.Close();
                }
                // 受信処理
                //foreach (var session in sessions)
                //{
                //    session.Poll();
                //}

                //Task.Delay(16).Wait();  // 少し待機します
            }

            //TcpClient client = listener.AcceptTcpClient();
            //Console.WriteLine("クライアントが接続しました。");

            //if (client.Connected)
            //{
            //    listener.Stop();
            //    NetworkStream netStream = client.GetStream();
            //    StreamReader sReader = new StreamReader(netStream, Encoding.UTF8);

            //    string str = String.Empty;

            //    do
            //    {
            //        str = sReader.ReadLine();
            //        if (null == str)
            //        {
            //            break;
            //        }
            //        Console.WriteLine(str);
            //    } while (!str.Equals("quit"));
            //    sReader.Close();
            //    client.Close();
            //}
            //Console.WriteLine("終了するには、Enterキーを押してください");
            //Console.ReadLine();
            //Log.i("Thread 1 end");
        }

    }
}
