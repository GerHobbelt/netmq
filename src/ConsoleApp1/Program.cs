using NetMQ;
using NetMQ.Sockets;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        private static  void CreateClient()
        {
            while (true)
            {
                try
                {
                    NetMQSocket socket = new StreamSocket();
                    socket.Connect(string.Format("tcp://{0}:{1}", "127.0.0.1", 10010));
                    socket.Options.Linger = TimeSpan.FromMinutes(1);
                    string transNo = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    string req =
                        string.Format(
                            "{0}<TransSeqID>201709081726360</TransSeqID><VerifyCode>R+Fo9QDXJKGE8h51nDl6Nrst/LjO59CKRrNbqHq8Q8Afct0zD6BQQVuuJ7CMdE1+3LegwgvXE351r0m5qyCl1RY3XTB1Mnu5IzsmloeXbaha9v3P0aVYgWL6GAc/rD6Kiemu4VjptwZb+O81pBY8OVtCyRZjCfC4NKXDVBlbMdA=</VerifyCode><ZipType></ZipType><CorpBankCode>103</CorpBankCode><FGCommandCode>11121</FGCommandCode><EnterpriseNum>QT330001</EnterpriseNum><FGVerifyCode>PYZjVNxLyNcRTP1A5EC0YC/Ogk7SHA8ZPeMx9Px0nxReyPKDfdGGzGwyZB5usAzlbFK/JB976z+S0wEp6SuP/1VZnUN4ZkDH+kbY2qnquD8RXSxrWmmOHlPIh9cJQGRvls1mrJQpti1FvJmeGDwdaLxdu+TLkr51LEpwZuQq6tQ=</FGVerifyCode></Head><RealTimeSingleTransReq><MoneyWay>1</MoneyWay><TransDate>20170908</TransDate><Trans><TransNo>{1}</TransNo><ProtocolCode></ProtocolCode><EnterpriseAccNum>103330101</EnterpriseAccNum><CustBankCode>103</CustBankCode><CustAccNum>1031234567890000000</CustAccNum><CustAccName></CustAccName><AreaCode></AreaCode><BankLocationCode></BankLocationCode><BankLocationName></BankLocationName><CardType></CardType><IsPrivate></IsPrivate><IsUrgent></IsUrgent><Amount>1.00</Amount><Currency>CNY</Currency><CertType>0</CertType><CertNum></CertNum><Mobile></Mobile><Purpose></Purpose><Memo></Memo><PolicyNumber></PolicyNumber><Extent1></Extent1><Extent2></Extent2><SourceTransNo>{1}</SourceTransNo></Trans></RealTimeSingleTransReq></Root>","10009<Root><Head><CommandCode>10009</CommandCode>", transNo);
                    byte[] bytes = Encoding.ASCII.GetBytes(req);

                    socket.SendMoreFrame(socket.Options.Identity);
                    socket.SendFrame(Combine(Length2Bytes(bytes.Length), bytes));
                    Interlocked.Increment(ref sum);
                    NetMQMessage resp = null;
                    if (socket.TryReceiveMultipartMessage(TimeSpan.FromSeconds(10), ref resp))
                    {
                        //成功计数+1
                    }
                    socket.Disconnect(string.Format("tcp://{0}:{1}", "127.0.0.1", 10010));
                    socket.Dispose();
                }
                catch (Exception exception)
                {

                }
            }
        }
        public static byte[] Length2Bytes(int length)
        {
            string lengthString = (length + "").PadLeft(8);
            byte[] resultBytes = Encoding.ASCII.GetBytes(lengthString);
            return resultBytes;
        }
        public static byte[] Combine(byte[] bytes1, byte[] bytes2)
        {
            byte[] c = new byte[bytes1.Length + bytes2.Length];
            bytes1.CopyTo(c, 0);
            bytes2.CopyTo(c, bytes1.Length);
            return c;
        }

        public static void StreamToStream()
        {
            using (var server = new StreamSocket())
            {
                using (var client = new StreamSocket())
                {
                    var port = server.BindRandomPort("tcp://*");
                    client.Connect("tcp://127.0.0.1:" + port);

                    byte[] clientId = client.Options.Identity;

                    const string request = "GET /\r\n";

                    const string response = "HTTP/1.0 200 OK\r\n" +
                        "Content-Type: text/plain\r\n" +
                        "\r\n" +
                        "Hello, World!";

                    client.SendMoreFrame(clientId).SendFrame(request);

                    byte[] serverId = server.ReceiveFrameBytes();
                    var a = server.ReceiveFrameString();
                    server.SendMoreFrame(serverId).SendFrame(response);
                    var b = client.ReceiveFrameBytes();
                    var c = client.ReceiveFrameString();
                }
                var message = server.ReceiveMultipartMessage();
            }
        }
        private static void Main(string[] args)
        {
            StreamToStream();
            //因为identity是内部生成的，第一部分是identity，第二部分是报文内容

            //for (int i = 0; i < 200; i++)
            //{
            //    Thread thread = new Thread(CreateClient);
            //    thread.IsBackground = true;
            //    thread.Start();
            //}

            //message = new NetMQMessage();
            //message.Append(message3.First);
            //message.Append("test2");
            //listen.SendMultipartMessage(message);
            //double sp7 = stopwatch.Elapsed.TotalMilliseconds;
            //var message4 =client.ReceiveMultipartMessage();
            //double sp8 = stopwatch.Elapsed.TotalMilliseconds;
            //var message2 = client.ReceiveMultipartMessage();
            Console.ReadKey();
            //try
            //{
            //    //创建一个轮询线程轮询处理客户端
            //    using (var poll = new NetMQPoller())
            //    {
            //        for (int i = 1; i <= 1; i++)
            //        {
            //            //心跳
            //            var timer = new NetMQTimer(TimeSpan.FromSeconds(10));
            //            var client = CreateClient("client" + i);
            //            poll.Add(client);
            //            poll.Add(timer);
            //            timer.Elapsed += (s, a) =>
            //            {
            //                client.SendFrame("Beep!");
            //            };
            //        }
            //        poll.RunAsync();
            //        Console.ReadKey();
            //    }
            //}
            //catch (Exception exception)
            //{
            //}
        }

        private static void Send_SendReady(object sender, NetMQSocketEventArgs e)
        {

        }

        private static void Listen_SendReady(object sender, NetMQSocketEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Listen_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var message1 = e.Socket.ReceiveMultipartMessage();
            NetMQMessage message = new NetMQMessage();
            message.Append(message1.First);
            message.Append("test2");
            e.Socket.SendMultipartMessage(message);
        }

        private static void Client_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {

        }

        private static int sum = 0;
        private static NetMQSocket CreateClient(string clientName)
        {
            DealerSocket client = new DealerSocket();
            client.Connect("tcp://localhost:5556");
            client.ReceiveReady += (s, a) =>
            {
                string msg = a.Socket.ReceiveFrameString();
                //Console.WriteLine("{0}:{1} Received{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), clientName,
                //    msg);
            };
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Interlocked.Increment(ref sum);
                        client.SendFrame(new byte[1024]);
                        Console.WriteLine("{0}：{1} Send {2}",
                            DateTime.Now.ToString("yyyyMMddHHmmssfff"), clientName, sum);
                    }
                    catch (Exception exception)
                    {

                    }
                }
            })
            {IsBackground = true};
            thread.Start();
            return client;
        }
    }
}