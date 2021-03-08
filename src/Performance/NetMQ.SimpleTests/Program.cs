using System;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;

namespace NetMQ.SimpleTests
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Title = "NetMQ HelloWorld";

            using (var sub = new SubscriberSocket())
            {
                sub.Options.PlainUsername = "operator";
                sub.Options.PlainPassword = "psiori";
                sub.Connect("tcp://192.168.0.52:6021");
                sub.SubscribeToAnyTopic();
                while (true)
                {
                    Msg msg = new Msg();
                    msg.InitEmpty();
                    sub.Receive(ref msg);
                    Console.WriteLine("RECEIVED MESSAGE: " + msg.Size);
                    // List<string> messageList = sub.ReceiveMultipartStrings();
                    // Console.WriteLine("Topic: {0} Message: {1}", messageList[0], messageList[1]);
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }

            /*using (var sub = new SubscriberSocket())
            {
                sub.Options.PlainUsername = "operator";
                sub.Options.PlainPassword = "psiori";
                sub.Connect("tcp://192.168.0.52:6021");
                sub.SubscribeToAnyTopic();
                while (true)
                {
                    Msg msg = new Msg();
                    msg.InitEmpty();
                    sub.Receive(ref msg);
                    Console.WriteLine("RECEIVED MESSAGE: " + msg.Size);
                    // List<string> messageList = sub.ReceiveMultipartStrings();
                    // Console.WriteLine("Topic: {0} Message: {1}", messageList[0], messageList[1]);
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }*/

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
    }
}
