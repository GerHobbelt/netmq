using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NetMQ.Tests
{
    internal class MockReceivingSocket : IReceivingSocket
    {
        private readonly Queue<byte[]> m_frames = new Queue<byte[]>();

        public TimeSpan LastTimeout { get; private set; }

        public bool TryReceive(ref Msg msg, TimeSpan timeout)
        {
            LastTimeout = timeout;

            if (m_frames.Count == 0)
                return false;

            var bytes = m_frames.Dequeue();

            msg.InitGC(bytes, bytes.Length);

            if (m_frames.Count != 0)
                msg.SetFlags(MsgFlags.More);

            return true;
        }

        public byte[] PushFrame(string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            m_frames.Enqueue(bytes);
            return bytes;
        }
    }

    public class ReceivingSocketExtensionsTests
    {
        private readonly MockReceivingSocket m_socket = new MockReceivingSocket();

        #region ReceiveFrameBytes

        [Test]
        public void ReceiveFrameBytesSingleFrame()
        {
            var expected = m_socket.PushFrame("Hello");

            byte[] actual = m_socket.ReceiveFrameBytes();

            Assert.True(actual.SequenceEqual(expected));
             Assert.AreEqual(SendReceiveConstants.InfiniteTimeout, m_socket.LastTimeout);

            // The buffer is copied into a new array
            Assert.AreNotSame(expected, actual);
        }

        [Test]
        public void ReceiveFrameBytesMultiFrame()
        {
            var expected1 = m_socket.PushFrame("Hello");
            var expected2 = m_socket.PushFrame("World");

            byte[] actual1 = m_socket.ReceiveFrameBytes(out bool more);

             Assert.AreEqual(SendReceiveConstants.InfiniteTimeout, m_socket.LastTimeout);
            Assert.True(more);
            Assert.True(actual1.SequenceEqual(expected1));
            Assert.AreNotSame(expected1, actual1);

            byte[] actual2 = m_socket.ReceiveFrameBytes(out more);

             Assert.AreEqual(SendReceiveConstants.InfiniteTimeout, m_socket.LastTimeout);
            Assert.False(more);
            Assert.True(actual2.SequenceEqual(expected2));
            Assert.AreNotSame(expected2, actual2);
        }

        #endregion

        #region TryReceiveFrameBytes

        [Test]
        public void TryReceiveFrameBytes()
        {
            var expected = m_socket.PushFrame("Hello");

            Assert.True(m_socket.TryReceiveFrameBytes(out byte[] actual));

             Assert.AreEqual(TimeSpan.Zero, m_socket.LastTimeout);
            Assert.True(actual.SequenceEqual(expected));
            Assert.AreNotSame(expected, actual);

            Assert.False(m_socket.TryReceiveFrameBytes(out actual));

             Assert.AreEqual(TimeSpan.Zero, m_socket.LastTimeout);
            Assert.Null(actual);
        }

        [Test]
        public void TryReceiveFrameBytesWithMore()
        {
            var expected1 = m_socket.PushFrame("Hello");
            var expected2 = m_socket.PushFrame("World");

            Assert.True(m_socket.TryReceiveFrameBytes(out byte[] actual, out bool more));

             Assert.AreEqual(TimeSpan.Zero, m_socket.LastTimeout);
            Assert.True(actual.SequenceEqual(expected1));
            Assert.True(more);
            Assert.AreNotSame(expected1, actual);

            Assert.True(m_socket.TryReceiveFrameBytes(out actual, out more));

             Assert.AreEqual(TimeSpan.Zero, m_socket.LastTimeout);
            Assert.True(actual.SequenceEqual(expected2));
            Assert.False(more);
            Assert.AreNotSame(expected1, actual);

            Assert.False(m_socket.TryReceiveFrameBytes(out actual, out more));
        }

        #endregion

        #region ReceiveMultipartBytes

        [Test]
        public void ReceiveMultipartBytes()
        {
            var expected = m_socket.PushFrame("Hello");

            List<byte[]> actual = m_socket.ReceiveMultipartBytes();

             Assert.AreEqual(1, actual.Count);
             Assert.AreEqual(4, actual.Capacity);
            Assert.True(actual[0].SequenceEqual(expected));
            Assert.AreNotSame(expected, actual[0]);
        }

        [Test]
        public void ReceiveMultipartBytesWithExpectedFrameCount()
        {
            var expected = m_socket.PushFrame("Hello");

            List<byte[]> actual = m_socket.ReceiveMultipartBytes(expectedFrameCount: 1);

             Assert.AreEqual(1, actual.Count);
             Assert.AreEqual(1, actual.Capacity);
            Assert.True(actual[0].SequenceEqual(expected));
            Assert.AreNotSame(expected, actual[0]);
        }

        #endregion

        #region ReceiveMultipartStrings

        [Test]
        public void ReceiveMultipartStrings()
        {
            const string expected = "Hello";

            m_socket.PushFrame(expected);

            List<string> actual = m_socket.ReceiveMultipartStrings();

             Assert.AreEqual(1, actual.Count);
             Assert.AreEqual(4, actual.Capacity);
             Assert.AreEqual(expected, actual[0]);
            Assert.AreNotSame(expected, actual[0]);
        }

        [Test]
        public void ReceiveMultipartStringsWithExpectedFrameCount()
        {
            const string expected = "Hello";

            m_socket.PushFrame(expected);

            List<string> actual = m_socket.ReceiveMultipartStrings(expectedFrameCount: 1);

             Assert.AreEqual(1, actual.Count);
             Assert.AreEqual(1, actual.Capacity);
             Assert.AreEqual(expected, actual[0]);
            Assert.AreNotSame(expected, actual[0]);
        }

        #endregion
    }
}
