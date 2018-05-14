using NetMQ.Sockets;
using NUnit.Framework;

namespace NetMQ.Tests
{
    public class ActorTests 
    {
        public ActorTests() => NetMQConfig.Cleanup();
        [Test]
        public void Simple()
        {
            void ShimAction(PairSocket shim)
            {
                shim.SignalOK();

                while (true)
                {
                    var msg = shim.ReceiveMultipartMessage();
                    var command = msg[0].ConvertToString();

                    if (command == NetMQActor.EndShimMessage)
                        break;

                    if (command == "Hello")
                    {
                         Assert.AreEqual(2, msg.FrameCount);
                         Assert.AreEqual("Hello", msg[1].ConvertToString());
                        shim.SendFrame("World");
                    }
                }
            }

            using (var actor = NetMQActor.Create(ShimAction))
            {
                actor.SendMoreFrame("Hello").SendFrame("Hello");

                 Assert.AreEqual("World", actor.ReceiveFrameString());
            }
        }
    }
}
