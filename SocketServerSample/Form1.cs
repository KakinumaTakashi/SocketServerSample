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

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(ipAdd);
            //TcpListener listener = new TcpListener(ipAdd);

            socket.Listen(10);
            //listener.Start(0);

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
                    Socket clientSocket = socket.Accept();
                    //var client = listener.AcceptTcpClient();
                    File.AppendAllText(logFilePath, Log.i("クライアントが接続しました。") + Environment.NewLine);

                    NetworkStream netStream = client.GetStream();
                    StreamReader streamReader = new StreamReader(netStream, Encoding.UTF8);
                    StreamWriter streamWriter = new StreamWriter(netStream, Encoding.UTF8);
                    File.AppendAllText(logFilePath, Log.i("ストリーム オープン") + Environment.NewLine);

                    while (true)
                    {
                        // スレッド停止要求チェック
                        if (cancelToken.IsCancellationRequested)
                        {
                            streamReader.Close();
                            streamWriter.Close();
                            File.AppendAllText(logFilePath, Log.i("ストリーム クローズ") + Environment.NewLine);
                            client.Close();
                            File.AppendAllText(logFilePath, Log.i("接続切断") + Environment.NewLine);
                            break;
                        }

                        // 受信
                        File.AppendAllText(logFilePath, Log.i("受信待機中") + Environment.NewLine);
                        byte[] resBytes = new byte[256];
                        int len = clientSocket.Receive(resBytes);
                        string readString = streamReader.ReadLine();
                        File.AppendAllText(logFilePath, Log.i("受信データ:" + readString) + Environment.NewLine);
                        //streamReader.Close();
                        File.AppendAllText(logFilePath, Log.i("受信完了") + Environment.NewLine);

                        // 送信
                        string writeString = "[Server response : " + readString + "]";
                        File.AppendAllText(logFilePath, Log.i("送信データ:" + writeString) + Environment.NewLine);
                        streamWriter.WriteLine(writeString);
                        //streamWriter.Close();
                        File.AppendAllText(logFilePath, Log.i("送信完了") + Environment.NewLine);
                    }
                }
                // 受信処理
                //foreach (var session in sessions)
                //{
                //    session.Poll();
                //}

                //Task.Delay(16).Wait();  // 少し待機します
                Thread.Sleep(1000);
            }

            File.AppendAllText(logFilePath, Log.i("method end") + Environment.NewLine);
        }
    }
}
