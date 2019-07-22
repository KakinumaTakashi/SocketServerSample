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

        private CancellationTokenSource tokenSource;
        private CancellationToken cancelToken;
        private string logFilePath = @"SocketServerSample.log";

        private void Form1_Load(object sender, EventArgs e)
        {
            File.AppendAllText(logFilePath, Log.i("Form1_Load start") + Environment.NewLine);

            // .Net Framework 4.0
            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;
            Task.Run(new Action(method), cancelToken);

            // .Net Framework 1.1
            //Thread thread1 = new Thread(new ThreadStart(method));

            File.AppendAllText(logFilePath, Log.i("Form1_Load end") + Environment.NewLine);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tokenSource.Cancel();
        }

        private void method()
        {
            File.AppendAllText(logFilePath, Log.i("method start") + Environment.NewLine);

            //IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse("192.168.56.101"), 8888);
            IPEndPoint ipAdd = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 8888);
            TcpListener listener = new TcpListener(ipAdd);
            listener.Start(0);
            File.AppendAllText(logFilePath, Log.i("Port:8888のListenを開始しました。") + Environment.NewLine);

            while (true)
            {
                // スレッド停止要求チェック
                if (cancelToken.IsCancellationRequested)
                {
                    listener.Stop();
                    File.AppendAllText(logFilePath, Log.i("Port:8888のListenを停止しました。") + Environment.NewLine);
                    break;
                }

                // 接続要求あるかどうか確認
                if (listener.Pending())
                {
                    // 接続要求を処理する
                    File.AppendAllText(logFilePath, Log.i("接続待機開始") + Environment.NewLine);
                    var client = listener.AcceptTcpClient();
                    File.AppendAllText(logFilePath, Log.i("クライアントが接続しました。") + Environment.NewLine);
                    //sessions.Add(new Session(client, SessionCommandExec));
                    //Console.WriteLine("AcceptTcpClient : {0}", client.Client.RemoteEndPoint);

                    NetworkStream netStream = client.GetStream();
                    File.AppendAllText(logFilePath, Log.i("ストリーム オープン") + Environment.NewLine);
                    StreamReader sReader = new StreamReader(netStream, Encoding.UTF8);
                    string str = sReader.ReadLine();
                    File.AppendAllText(logFilePath, Log.i(str) + Environment.NewLine);

                    sReader.Close();
                    File.AppendAllText(logFilePath, Log.i("ストリーム クローズ") + Environment.NewLine);
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

            File.AppendAllText(logFilePath, Log.i("method end") + Environment.NewLine);
        }


    }
}
