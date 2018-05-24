using NetMQ.Sockets;
using System.Threading;
using NUnit.Framework;
using System;

namespace NetMQ.Tests
{
    /// <summary>
    /// 基于AsyncModule.NetMQ新增的功能测试
    /// </summary>
    public class AsyncModuleNetMQTest 
    {
        public AsyncModuleNetMQTest() => NetMQConfig.Cleanup();
        [Test(Description = "服务端可获取客户端的地址测试")]
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
                 Assert.AreEqual(reqMessage.Address.ToString(), client.Options.LocalEndpoint);

                 Assert.AreEqual(request, reqMessage.Last.ConvertToString());

                server.SendMoreFrame(reqMessage.First.Buffer).SendFrame(response);

                 Assert.AreEqual(clientId, client.ReceiveFrameBytes());
                 Assert.AreEqual(response, client.ReceiveFrameString());
            }
        }

        [Test(Description ="服务端主动关闭连接测试")]
        public void ProactiveCloseConnect()
        {
            using (var server = new StreamSocket())
            {
                //响应客户端完成时主动关闭连接
                server.Options.ProactiveCloseConnect = true;
                var port = 10010;
                server.Bind("tcp://*:" + port);
                using (var client = new StreamSocket())
                {
                    client.Connect("tcp://127.0.0.1:" + port);
                    client.Options.NotifyWhenConnectedFail = true;
                    byte[] clientId = client.Options.Identity;

                    const string request = "GET /\r\n";

                    const string response = "HTTP/1.0 200 OK\r\n" +
                        "Content-Type: text/plain\r\n" +
                        "\r\n" +
                        "Hello, World!";

                    client.SendMoreFrame(clientId).SendFrame(request);

                    NetMQMessage reqMessage = server.ReceiveMultipartMessage();
                    Assert.AreEqual(reqMessage.Address.ToString(), client.Options.LocalEndpoint);

                    Assert.AreEqual(request, reqMessage.Last.ConvertToString());

                    server.SendMoreFrame(reqMessage.First.Buffer).SendFrame(response);

                    var respMessage =  client.ReceiveMultipartMessage();
                    Assert.AreEqual(respMessage.MessageType, NetMQMessageType.Data);
                }
            }
        }

    }
}
