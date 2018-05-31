using NetMQ.Sockets;
using System;
using System.Threading;
using NUnit.Framework;

namespace NetMQ.Tests
{
    /// <summary>
    /// 基于AsyncModule.NetMQ新增的功能测试
    /// </summary>
    public class NetMQMessageTypeTest //
    {
        public NetMQMessageTypeTest() => NetMQConfig.Cleanup();
        [MaxTime(3000)]
        [Test(Description = "连接断开通知")]
        public void DisConnectedNotifyReceiveMessageTypeTest()
        {
            using (var server = new StreamSocket())
            {
                server.Options.ThrowDelimiter = true;
                int port= 10001;
                server.Bind("tcp://127.0.0.1:10001");
                using (var client = new StreamSocket())
                {
                    client.Connect("tcp://127.0.0.1:" + port);
                }
                NetMQMessage reqMessage =  server.ReceiveMultipartMessage();
                Assert.AreEqual(1, reqMessage.FrameCount);
                Assert.AreEqual(NetMQMessageType.DisConnected, reqMessage.MessageType);
            }
        }

        private void Server_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket server = e.Socket;
            NetMQMessage reqMessage =  server.ReceiveMultipartMessage();
            Assert.AreEqual(2, reqMessage.FrameCount);
            Assert.AreEqual(NetMQMessageType.Data, reqMessage.MessageType);
            reqMessage = server.ReceiveMultipartMessage();
            Assert.AreEqual(1, reqMessage.FrameCount);
            Assert.AreEqual(NetMQMessageType.DisConnected, reqMessage.MessageType);
        }

        [Test(Description = "连接断开通知")]
        public void DisConnectedNotifyTryReceiveMessageTypeTest()
        {
            using (var server = new StreamSocket())
            {
                server.Options.ThrowDelimiter = true;
                int port = server.BindRandomPort("tcp://*");
                using (var client = new StreamSocket())
                {
                    client.Connect("tcp://127.0.0.1:" + port);
                }
                NetMQMessage reqMessage = null;
                if (server.TryReceiveMultipartMessage(TimeSpan.FromSeconds(3), ref reqMessage))
                {
                    Assert.AreEqual(1, reqMessage.FrameCount);
                    Assert.AreEqual(NetMQMessageType.DisConnected, reqMessage.MessageType);
                }
                else
                {
                    Assert.False(true);
                }
            }
        }
        [Test(Description = "连接断开不通知")]
        public void DisConnectedNotNotifyMessageTypeTest()
        {
            using (var server = new StreamSocket())
            {
                server.Options.ThrowDelimiter = false;
                int port = server.BindRandomPort("tcp://*");
                using (var client = new StreamSocket())
                {
                    client.Connect("tcp://127.0.0.1:" + port);
                }
                NetMQMessage reqMessage = null;
                if (server.TryReceiveMultipartMessage(TimeSpan.FromSeconds(1), ref reqMessage))
                {
                    Assert.False(true);
                }
            }
        }
        [MaxTime(3000)]
        [Test(Description = "连接失败")]

        public void ConnectFailedReceiveMessageTypeTest()
        {
            using (var client = new StreamSocket())
            {
                client.Connect("tcp://127.0.0.1:" + 12345);
                client.Options.NotifyWhenConnectedFail = true;
                client.Options.MaxConnectedFailCount = 1;
                NetMQMessage reqMessage =  client.ReceiveMultipartMessage();
                Assert.AreEqual(2, reqMessage.FrameCount);
                Assert.AreEqual(NetMQMessageType.SocketError, reqMessage.MessageType);
                Assert.AreEqual("127.0.0.1", reqMessage.Address.Address.ToString());
                Assert.AreEqual(12345, reqMessage.Address.Port);
            }
        }
        [Test(Description = "连接失败")]

        public void ConnectFailedTryReceiveMessageTypeTest()
        {
            using (var client = new StreamSocket())
            {
                client.Connect("tcp://127.0.0.1:" + 12345);
                client.Options.NotifyWhenConnectedFail = true;
                client.Options.MaxConnectedFailCount = 1;

                NetMQMessage reqMessage = null;
                if (client.TryReceiveMultipartMessage(TimeSpan.FromSeconds(3), ref reqMessage))
                {
                    Assert.AreEqual(2, reqMessage.FrameCount);
                    Assert.AreEqual(NetMQMessageType.SocketError, reqMessage.MessageType);
                    Assert.AreEqual("127.0.0.1", reqMessage.Address.Address.ToString());
                    Assert.AreEqual(12345, reqMessage.Address.Port);
                }
                else
                {
                    Assert.False(true);
                }
            }
        }
        [Test(Description = "连接失败")]

        public void ConnectFailed2TryReceiveMessageTypeTest()
        {
            using (var client = new StreamSocket())
            {
                client.Connect("tcp://127.0.0.1:" + 12345);
                client.Options.ReconnectInterval = TimeSpan.FromMilliseconds(-1);
                NetMQMessage reqMessage = null;
                if (client.TryReceiveMultipartMessage(TimeSpan.FromSeconds(3), ref reqMessage))
                {
                    Assert.AreEqual(2, reqMessage.FrameCount);
                    Assert.AreEqual(NetMQMessageType.SocketError, reqMessage.MessageType);
                    Assert.AreEqual("127.0.0.1", reqMessage.Address.Address.ToString());
                    Assert.AreEqual(12345, reqMessage.Address.Port);
                }
                else
                {
                    Assert.False(true);
                }
            }
        }
        [MaxTime(3000)]
        [Test(Description = "连接失败")]

        public void ConnectFailed2ReceiveMessageTypeTest()
        {
            using (var client = new StreamSocket())
            {
                client.Connect("tcp://127.0.0.1:" + 12345);
                client.Options.ReconnectInterval = TimeSpan.FromMilliseconds(-1);
                NetMQMessage reqMessage =  client.ReceiveMultipartMessage();
                Assert.AreEqual(2, reqMessage.FrameCount);
                Assert.AreEqual(NetMQMessageType.SocketError, reqMessage.MessageType);
                Assert.AreEqual("127.0.0.1", reqMessage.Address.Address.ToString());
                Assert.AreEqual(12345, reqMessage.Address.Port);
            }
        }
    }
}
