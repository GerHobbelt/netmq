using NUnit.Framework;

using NetMQ.Sockets;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetMQ.Tests
{
    public class CreateSocketTests
    {
        public CreateSocketTests() => NetMQConfig.Cleanup();

        [TearDown]
        public void Clear()
        {
            NetMQConfig.Cleanup();
        }
        [TestCase(false, 1024)]
        [TestCase(false, 2048)]
        [TestCase(true, 1024)]
        [TestCase(true, 2048)]
        [TestCase(true, 4096)]
        [TestCase(true, 8192)]
        public void DilateSocketTest(bool autoDilate,int maxSocketCount)
        {
            Assert.IsTrue(maxSocketCount >= 1024);
            NetMQConfig.AutoDilate = autoDilate;
            NetMQConfig.MaxAutoDilate = maxSocketCount;
            List<StreamSocket> sockets= new List<StreamSocket>();
            for (int i = 0; i < 1024; i++)
            {
                StreamSocket socket = new StreamSocket();
                sockets.Add(socket);
            }
            for (int i = 0; i < maxSocketCount - 1024; i++)
            {
                if (!autoDilate)
                {
                    NetMQException exception = Assert.Throws<NetMQException>(() => new StreamSocket());
                }
                else
                {
                    StreamSocket socket = new StreamSocket();
                    sockets.Add(socket);
                }
            }
            foreach (NetMQSocket socket in sockets)
            {
                socket.Dispose();
            }

        }
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void DilateSocketTest(int count)
        {
            List<StreamSocket> sockets= new List<StreamSocket>();
            Assert.IsTrue(count > 0);
            NetMQConfig.AutoDilate = true;
            NetMQConfig.MaxAutoDilate = (int)(1024 * Math.Pow(2, count));
            for (int i = 0; i < 1024 * Math.Pow(2, count - 1); i++)
            {
                StreamSocket socket = new StreamSocket();
                sockets.Add(socket);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            sockets.Add(new StreamSocket());
            Console.WriteLine(1024 * Math.Pow(2, count) + "->" + 1024 * Math.Pow(2, count - 1) + "：" + stopwatch.ElapsedMilliseconds);
            Assert.AreEqual(NetMQConfig.Context.m_sockets.Count, 1024 * Math.Pow(2, count - 1) + 1);
            Assert.AreEqual(NetMQConfig.Context.m_emptySlots.Count, 1024 * Math.Pow(2, count - 1) - 1);
            foreach(NetMQSocket socket in sockets)
            {
                socket.Dispose();
            }
        }
    }
}
