using NUnit.Framework;

using NetMQ.Sockets;
using System.Threading;

namespace NetMQ.Tests
{
    public class StreamTests 
    {
        public StreamTests() => NetMQConfig.Cleanup();

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
        public void CtxCanReadSocket()
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
                    Thread.Sleep(100);
                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);
                    
                    byte[] serverId = server.ReceiveFrameBytes();
                    Assert.AreEqual(request, server.ReceiveFrameString());
                    Assert.AreEqual(0,  NetMQConfig.Context.GetCanReadSocket.Count);
                    server.SendMoreFrame(serverId).SendFrame(response);

                    Thread.Sleep(100);
                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);

                    Assert.AreEqual(clientId, client.ReceiveFrameBytes());
                    Assert.AreEqual(response, client.ReceiveFrameString());

                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);
                    string str;
                    if(client.TryReceiveFrameString(out str))
                    {

                    }
                    Assert.AreEqual(0, NetMQConfig.Context.GetCanReadSocket.Count);
                }
                Thread.Sleep(100);
                Assert.AreEqual(0, NetMQConfig.Context.GetCanReadSocket.Count);
            }
        }
        [Test]
        public void CtxCanReadSocket2()
        {
            using (var server = new StreamSocket())
            {
                server.Options.ThrowDelimiter = true;
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
                    Thread.Sleep(100);
                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);

                    byte[] serverId = server.ReceiveFrameBytes();
                    Assert.AreEqual(request, server.ReceiveFrameString());
                    Assert.AreEqual(0, NetMQConfig.Context.GetCanReadSocket.Count);
                    server.SendMoreFrame(serverId).SendFrame(response);

                    Thread.Sleep(100);
                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);

                    Assert.AreEqual(clientId, client.ReceiveFrameBytes());
                    Assert.AreEqual(response, client.ReceiveFrameString());

                    Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);
                    string str;
                    if (client.TryReceiveFrameString(out str))
                    {

                    }
                    Assert.AreEqual(0, NetMQConfig.Context.GetCanReadSocket.Count);
                }
                Thread.Sleep(100);
                Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);
                var message = server.ReceiveMultipartMessage();
                Assert.AreEqual(message.MessageType, NetMQMessageType.DisConnected);
                Assert.AreEqual(1, NetMQConfig.Context.GetCanReadSocket.Count);

                server.TryReceiveMultipartMessage(ref message);
                Assert.AreEqual(0, NetMQConfig.Context.GetCanReadSocket.Count);
            }
        }
    }
}
