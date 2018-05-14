using System;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetMQ.Tests
{
    public class SocketOptionsTests 
    {
        public SocketOptionsTests() => NetMQConfig.Cleanup();

        [Test]
        public void DefaultValues()
        {
            using (var socket = new RouterSocket())
            {
                Assert.Null(socket.Options.Identity);
//                Assert.Null(socket.Options.TcpAcceptFilter);
                 Assert.AreEqual(false, socket.Options.ReceiveMore);
            }
        }

        [Test]
        public void GetAndSetAllProperties()
        {
            using (var socket = new RouterSocket())
            {
                socket.Options.Affinity = 1L;
                 Assert.AreEqual(1L, socket.Options.Affinity);

                socket.Options.Identity = new[] { (byte)1 };
                 Assert.AreEqual(1, socket.Options.Identity.Length);
                 Assert.AreEqual(1, socket.Options.Identity[0]);

                socket.Options.MulticastRate = 100;
                 Assert.AreEqual(100, socket.Options.MulticastRate);

                socket.Options.MulticastRecoveryInterval = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.MulticastRecoveryInterval);

                socket.Options.ReceiveBuffer = 100;
                 Assert.AreEqual(100, socket.Options.ReceiveBuffer);

//                socket.Options.ReceiveMore = true;

                socket.Options.Linger = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.Linger);

                socket.Options.ReconnectInterval = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.ReconnectInterval);

                socket.Options.ReconnectIntervalMax = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.ReconnectIntervalMax);

                socket.Options.Backlog = 100;
                 Assert.AreEqual(100, socket.Options.Backlog);

                socket.Options.MaxMsgSize = 100;
                 Assert.AreEqual(100, socket.Options.MaxMsgSize);

                socket.Options.SendHighWatermark = 100;
                 Assert.AreEqual(100, socket.Options.SendHighWatermark);

                socket.Options.ReceiveHighWatermark = 100;
                 Assert.AreEqual(100, socket.Options.ReceiveHighWatermark);

                socket.Options.MulticastHops = 100;
                 Assert.AreEqual(100, socket.Options.MulticastHops);

                socket.Options.IPv4Only = true;
                 Assert.AreEqual(true, socket.Options.IPv4Only);

                Assert.Null(socket.Options.LastEndpoint);

                socket.Options.RouterMandatory = true;
//                 Assert.AreEqual(true, socket.Options.RouterMandatory);

                socket.Options.TcpKeepalive = true;
                 Assert.AreEqual(true, socket.Options.TcpKeepalive);

//                socket.Options.TcpKeepaliveCnt = 100;
//                 Assert.AreEqual(100, socket.Options.TcpKeepaliveCnt);

                socket.Options.TcpKeepaliveIdle = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.TcpKeepaliveIdle);

                socket.Options.TcpKeepaliveInterval = TimeSpan.FromMilliseconds(100);
                 Assert.AreEqual(TimeSpan.FromMilliseconds(100), socket.Options.TcpKeepaliveInterval);

                socket.Options.DelayAttachOnConnect = true;
                 Assert.AreEqual(true, socket.Options.DelayAttachOnConnect);

                socket.Options.RouterRawSocket = true;
//                 Assert.AreEqual(true, socket.Options.RouterRawSocket);

                socket.Options.Endian = Endianness.Little;
                 Assert.AreEqual(Endianness.Little, socket.Options.Endian);

                Assert.False(socket.Options.DisableTimeWait);
                socket.Options.DisableTimeWait = true;
                Assert.True(socket.Options.DisableTimeWait);
            }

            using (var socket = new XPublisherSocket())
            {
                socket.Options.XPubVerbose = true;
//                 Assert.AreEqual(true, socket.Options.XPubVerbose);

                socket.Options.ManualPublisher = true;
//                 Assert.AreEqual(true, socket.Options.ManualPublisher);
            }
        }
    }
}
