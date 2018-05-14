using NetMQ.Core.Utils;
using NUnit.Framework;

namespace NetMQ.Tests.Core
{
    public class YQueueTests
    {
        [Test]
        public void PushingToQueueShouldIncreaseBackPosition()
        {
            string one = "One";
            string two = "Two";
            string three = "Three";

            var queue = new YQueue<string>(100);
             Assert.AreEqual(0, queue.BackPos);
            queue.Push(ref one);
             Assert.AreEqual(1, queue.BackPos);
            queue.Push(ref two);
             Assert.AreEqual(2, queue.BackPos);
            queue.Push(ref three);
             Assert.AreEqual(3, queue.BackPos);
        }

        [Test]
        public void PoppingFromQueueShouldIncreaseFrontPosition()
        {
            var queue = new YQueue<string>(100);

            string one = "One";
            string two = "Two";
            string three = "Three";

            queue.Push(ref one);
            queue.Push(ref two);
            queue.Push(ref three);
             Assert.AreEqual(0, queue.FrontPos);
            queue.Pop();
             Assert.AreEqual(1, queue.FrontPos);
            queue.Pop();
             Assert.AreEqual(2, queue.FrontPos);
            queue.Pop();
             Assert.AreEqual(3, queue.FrontPos);
        }

        [Test]
        public void QueuedItemsShouldBeReturned()
        {
            string one = "One";
            string two = "Two";
            string three = "Three";

            var queue = new YQueue<string>(100);
            queue.Push(ref one);
            queue.Push(ref two);
            queue.Push(ref three);
             Assert.AreEqual("One", queue.Pop());
             Assert.AreEqual("Two", queue.Pop());
             Assert.AreEqual("Three", queue.Pop());
        }

        [Test]
        public void SmallChunkSizeShouldNotAffectBehavior()
        {
            string one = "One";
            string two = "Two";
            string three = "Three";
            string four = "Four";
            string five = "Five";

            var queue = new YQueue<string>(2);
            queue.Push(ref one);
            queue.Push(ref two);
            queue.Push(ref three);
            queue.Push(ref four);
            queue.Push(ref five);
             Assert.AreEqual("One", queue.Pop());
             Assert.AreEqual("Two", queue.Pop());
             Assert.AreEqual("Three", queue.Pop());
             Assert.AreEqual("Four", queue.Pop());
             Assert.AreEqual("Five", queue.Pop());
            // On empty queue the front position should be equal to back position
             Assert.AreEqual(queue.FrontPos, queue.BackPos);
        }

        [Test]
        public void UnpushShouldRemoveLastPushedItem()
        {
            string one = "One";
            string two = "Two";
            string three = "Three";

            var queue = new YQueue<string>(2);
            queue.Push(ref one);
            queue.Push(ref two);
            queue.Push(ref three);
            // Back position should be decremented after unpush
             Assert.AreEqual(3, queue.BackPos);
            // Unpush should return the last item in a queue
             Assert.AreEqual("Three", queue.Unpush());
             Assert.AreEqual(2, queue.BackPos);
             Assert.AreEqual("Two", queue.Unpush());
             Assert.AreEqual(1, queue.BackPos);
             Assert.AreEqual("One", queue.Unpush());
             Assert.AreEqual(0, queue.BackPos);
        }
    }
}
