using NetMQ.Sockets;
using System.Threading;
using Xunit;

namespace NetMQ.Tests
{
    /// <summary>
    /// 基于AsyncModule.NetMQ新增的功能测试
    /// </summary>
    public class AsyncModuleNetMQTest : IClassFixture<CleanupAfterFixture>
    {
        public AsyncModuleNetMQTest() => NetMQConfig.Cleanup();

        [Fact(DisplayName = "服务端可获取客户端的地址测试")]
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
                Assert.Equal(reqMessage.Address.ToString(), client.Options.LocalEndpoint);

                Assert.Equal(request, reqMessage.Last.ConvertToString());

                server.SendMoreFrame(reqMessage.First.Buffer).SendFrame(response);

                Assert.Equal(clientId, client.ReceiveFrameBytes());
                Assert.Equal(response, client.ReceiveFrameString());
            }
        }

        [Fact(DisplayName ="服务端主动关闭连接测试")]
        public void ProactiveCloseConnect()
        {
            var server = new StreamSocket();
            //响应客户端完成时主动关闭连接
            server.Options.ProactiveCloseConnect = true;
            var port = 10010;
            server.Bind("tcp://*:" + port);
            using (var client = new StreamSocket())
            {
                client.Connect("tcp://127.0.0.1:" + port);

                byte[] clientId = client.Options.Identity;

                const string request = "GET /\r\n";

                const string response = "HTTP/1.0 200 OK\r\n" +
                        "Content-Type: text/plain\r\n" +
                        "\r\n" +
                        "Hello, World!";

                client.SendMoreFrame(clientId).SendFrame(request);

                NetMQMessage reqMessage = server.ReceiveMultipartMessage();
                Assert.Equal(reqMessage.Address.ToString() , client.Options.LocalEndpoint);

                Assert.Equal(request ,reqMessage.Last.ConvertToString());

                server.SendMoreFrame(reqMessage.First.Buffer).SendFrame(response);

                var respMessage =  client.ReceiveMultipartMessage();
                Thread.Sleep(1000);
                //连接主动断开，自动重连，地址应该变了
                Assert.NotEqual(reqMessage.Address.ToString(), client.Options.LocalEndpoint);
            }
        }

    }
}
