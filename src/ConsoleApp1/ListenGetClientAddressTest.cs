using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ.Sockets;


namespace NetMQ.Tests
{
    [TestClass]
    class ListenGetClientAddressTest
    {
        [TestInitialize]
        public void Init()
        {
            NetMQConfig.Cleanup();
        }

        [TestMethod]
        public void GetClientAddressTest()
        {
            using (var server = new StreamSocket())
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

                NetMQMessage reqMessage = server.ReceiveMultipartMessage();
                Assert.AreEqual(reqMessage.Address, "tcp://127.0.0.1:" + port);

                Assert.AreEqual(request, server.ReceiveFrameString());

                server.SendMoreFrame(reqMessage.First.Buffer).SendFrame(response);

                Assert.AreEqual(clientId, client.ReceiveFrameBytes());
                Assert.AreEqual(response, client.ReceiveFrameString());
            }
        }

    }
}
