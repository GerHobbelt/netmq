using System;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using NetMQ.Core.Mechanisms;
using System.Threading;

namespace NetMQ.SimpleTests
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Title = "NetMQ HelloWorld";

            /*using (var pub = new PublisherSocket())
            {

                bool usePlain = true;

                if (usePlain)
                {
                    pub.Options.PlainUsername = "operator";
                    pub.Options.PlainPassword = "psiori";
                    pub.Options.PlainServer = true;
                }
                pub.Bind("tcp://192.168.0.193:6021");
                int i = 0;
                while (true)
                {
                    Thread.Sleep(500);
                    pub.SendMoreFrame("A").SendFrame("Hello - " + i++);
                    Console.WriteLine("SENDING MESSAGE on port 6021");
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }*/





            using (var sub = new SubscriberSocket())
            {
                bool usePlain = true;


                if (usePlain)
                {
                    sub.Options.PlainUsername = "operator";
                    sub.Options.PlainPassword = "psiori";
                }

                sub.Connect("tcp://192.168.0.52:6021");
                sub.SubscribeToAnyTopic();
                while (true)
                {
                    Msg msg = new Msg();
                    msg.InitEmpty();
                    sub.Receive(ref msg);
                    Console.WriteLine("RECEIVED MESSAGE: " + msg.Size);
                    PrintMessage(msg);
                    // List<string> messageList = sub.ReceiveMultipartStrings();
                    // Console.WriteLine("Topic: {0} Message: {1}", messageList[0], messageList[1]);
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }



            /*using (var server = new ResponseSocket("@tcp://localhost:5556"))
            using (var client = new RequestSocket("tcp://localhost:5556"))
            {
                client.SendFrame("Hello");

                Console.WriteLine("From Client: {0}", server.ReceiveFrameString());

                server.SendFrame("Hi Back");

                Console.WriteLine("From Server: {0}", client.ReceiveFrameString());

                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }*/
        }

        public static void PrintMessage(Msg msg)
        {
            for (int i = 0; i < msg.Size; i++)
            {
                if (msg[i] < 32 || (msg[i] > 47 && msg[i] < 58))
                {
                    Console.Write(" " + msg[i] + " ");
                }
                else
                {
                    Console.Write(((char)msg[i]));
                }
            }
            Console.Write("\n");
        }
    }
}
