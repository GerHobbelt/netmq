using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        public static void ReceiveNoticeTest()
        {
            bool serverReceived =false;
            bool clientReceived = false;
            AutoResetEvent waitEvent = new AutoResetEvent(false);
            using (var server = new StreamSocket())
            {
                server.SetReceiveNotice(sender =>
                {
                    serverReceived = true;
                    waitEvent.Set();
                });
                server.Options.ThrowDelimiter = true;
                int port = server.BindRandomPort("tcp://*");
                using (var client = new StreamSocket())
                {
                    client.SetReceiveNotice(sender =>
                    {
                        clientReceived = true;
                        waitEvent.Set();
                    });
                    client.Connect("tcp://127.0.0.1:" + port);
                    client.SendMoreFrame(client.Options.Identity);
                    client.SendFrame("test");
                    waitEvent.WaitOne();
                    NetMQMessage reqMessage = server.ReceiveMultipartMessage();
                    server.SendMultipartMessage(reqMessage);

                    waitEvent.WaitOne();
                    reqMessage = client.ReceiveMultipartMessage();
                }
            }
        }
        static void Main(string[] args)
        {
            ReceiveNoticeTest();
        }
    }
}
