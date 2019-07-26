using NUnit.Framework;

using NetMQ.Sockets;
using System.Threading;
using System;

namespace NetMQ.Tests
{
    public class StreamTests 
    {
        public StreamTests() => NetMQConfig.Cleanup();

        [Test(Description = "发送空包测试")]
        public void SendEmptyDataTest()
        {
            using (var server = new StreamSocket())
            {
                server.Options.ThrowDelimiter = true;
                server.Bind("tcp://127.0.0.1:22111");
                using (var client = new StreamSocket())
                {
                    //不自动重连
                    client.Options.ReconnectInterval = TimeSpan.FromMilliseconds(-1);
                    //短链接不自动重连，连接断开立即释放
                    client.Options.DelayOnDisconnect = false;
                    client.Connect("tcp://127.0.0.1:22111");
                    client.SendMoreFrame(client.Options.Identity);
                    client.SendFrame("test");
                    NetMQMessage reqMessage = server.ReceiveMultipartMessage();
                    reqMessage.RemoveFrame(reqMessage.Last);
                    reqMessage.Append(EmptyArray<byte>.Instance);
                    server.SendMultipartMessage(reqMessage);
                    bool r = server.TryReceiveMultipartMessage(TimeSpan.FromSeconds(1),ref reqMessage);
                    Assert.IsFalse(r);
                    r = client.TryReceiveMultipartMessage(TimeSpan.FromSeconds(1), ref reqMessage);
                    Assert.IsFalse(r);
                    Assert.Throws<HostUnreachableException>(() => client.SendMoreFrame(client.Options.Identity));
                    r = server.TryReceiveMultipartMessage(TimeSpan.FromSeconds(1), ref reqMessage);
                    Assert.IsFalse(r);
                }
            }
        }
        [Test]
        public void StreamToStream()
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
                     Assert.AreEqual(request, server.ReceiveFrameString());

                    server.SendMoreFrame(serverId).SendFrame(response);

                     Assert.AreEqual(clientId, client.ReceiveFrameBytes());
                     Assert.AreEqual(response, client.ReceiveFrameString());
                }
            }
        }
        [Test]
        [Ignore("no run")]
        public void BigPackageStreamToStream()
        {
            using (var server = new StreamSocket())
            {
                server.Options.SendHighWatermark = -1;
                server.Options.ReceiveHighWatermark = -1;
                int port = 10021;
                server.Bind("tcp://*:" + port);
                using (var client = new StreamSocket())
                {
                    client.Options.SendHighWatermark = -1;
                    client.Options.ReceiveHighWatermark = -1;
                    client.Connect("tcp://*:" + port);

                    byte[] clientId = client.Options.Identity;

                    byte[] bytes =  new byte[1024*1024*8];
                    for (int i = 0; i < 100; i++)
                    {
                        client.SendMoreFrame(clientId).SendFrame(bytes);
                    }
                    //等待入队
                    Thread.Sleep(10000);
                    int count = 0;
                    long length = 0;
                    while (count < 100)
                    {
                        NetMQMessage message = null;
                        if (server.TryReceiveMultipartMessage(ref message))
                        {
                            server.SendMultipartMessage(message);
                            length += message.Last.BufferSize;
                            if (length >= 1024 * 1024 * 8)
                            {
                                count++;
                                length = length % (1024 * 1024 * 8);
                            }
                        }
                    }
                    //等待入队
                    Thread.Sleep(10000);
                    length = 0;
                    count = 0;
                    while (count < 100)
                    {
                        NetMQMessage message = null;
                        if (client.TryReceiveMultipartMessage(ref message))
                        {
                            length += message.Last.BufferSize;
                            if (length >= 1024 * 1024 * 8)
                            {
                                count++;
                                length = length % (1024 * 1024 * 8);
                            }
                        }
                    }
                    
                }
            }
        }
    }
}
