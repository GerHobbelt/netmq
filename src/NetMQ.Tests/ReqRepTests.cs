﻿using NetMQ.Sockets;
using NUnit.Framework;

namespace NetMQ.Tests
{
    public class ReqRepTests 
    {
        public ReqRepTests() => NetMQConfig.Cleanup();

        [Theory]
        [TestCase("tcp://localhost")]
        [TestCase("tcp://127.0.0.1")]
        public void SimpleReqRep(string address)
        {
            using (var rep = new ResponseSocket())
            using (var req = new RequestSocket())
            {
                var port = rep.BindRandomPort(address);
                req.Connect(address + ":" + port);

                req.SendFrame("Hi");

                 Assert.AreEqual(new[] { "Hi" }, rep.ReceiveMultipartStrings());

                rep.SendFrame("Hi2");

                 Assert.AreEqual(new[] { "Hi2" }, req.ReceiveMultipartStrings());
            }
        }

        [Test]
        public void SendingTwoRequestsInARow()
        {
            using (var rep = new ResponseSocket())
            using (var req = new RequestSocket())
            {
                var port = rep.BindRandomPort("tcp://localhost");
                req.Connect("tcp://localhost:" + port);

                req.SendFrame("Hi");

                rep.SkipFrame();

                Assert.Throws<FiniteStateMachineException>(() => req.SendFrame("Hi2"));
            }
        }

        [Test]
        public void ReceiveBeforeSending()
        {
            using (var rep = new ResponseSocket())
            using (var req = new RequestSocket())
            {
                var port = rep.BindRandomPort("tcp://localhost");
                req.Connect("tcp://localhost:" + port);

                Assert.Throws<FiniteStateMachineException>(() => req.ReceiveFrameBytes());
            }
        }

        [Test]
        public void SendMessageInResponseBeforeReceiving()
        {
            using (var rep = new ResponseSocket())
            using (var req = new RequestSocket())
            {
                var port = rep.BindRandomPort("tcp://localhost");
                req.Connect("tcp://localhost:" + port);

                Assert.Throws<FiniteStateMachineException>(() => rep.SendFrame("1"));
            }
        }

        [Test]
        public void SendMultipartMessage()
        {
            using (var rep = new ResponseSocket())
            using (var req = new RequestSocket())
            {
                var port = rep.BindRandomPort("tcp://localhost");
                req.Connect("tcp://localhost:" + port);

                req.SendMoreFrame("Hello").SendFrame("World");

                 Assert.AreEqual(new[] { "Hello", "World" }, rep.ReceiveMultipartStrings());

                rep.SendMoreFrame("Hello").SendFrame("Back");

                 Assert.AreEqual(new[] { "Hello", "Back" }, req.ReceiveMultipartStrings());
            }
        }
    }
}
