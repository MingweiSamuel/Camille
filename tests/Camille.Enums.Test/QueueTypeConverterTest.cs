using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class QueueTypeConverterTest
    {
        [TestMethod]
        public void KnownValue_Deserializes()
        {
            var result = JsonSerializer.Deserialize<QueueType>("\"RANKED_SOLO_5x5\"");
            Assert.AreEqual(QueueType.RANKED_SOLO_5x5, result);
        }

        [TestMethod]
        public void UnrecognizedValue_FallsBackToUnknown()
        {
            var result = JsonSerializer.Deserialize<QueueType>("\"NOT_A_REAL_QUEUE_TYPE\"");
            Assert.AreEqual(QueueType.UNKNOWN, result);
        }

        [TestMethod]
        public void KnownValue_DifferentCase_Deserializes()
        {
            var result = JsonSerializer.Deserialize<QueueType>("\"ranked_solo_5x5\"");
            Assert.AreEqual(QueueType.RANKED_SOLO_5x5, result);
        }

        [TestMethod]
        public void NumericString_FallsBackToUnknown()
        {
            var result = JsonSerializer.Deserialize<QueueType>("\"123\"");
            Assert.AreEqual(QueueType.UNKNOWN, result);
        }

        [TestMethod]
        public void NonStringToken_FallsBackToUnknown()
        {
            var result = JsonSerializer.Deserialize<QueueType>("123");
            Assert.AreEqual(QueueType.UNKNOWN, result);
        }

        [TestMethod]
        public void UnrecognizedValue_DoesNotThrow_InArray()
        {
            var result = JsonSerializer.Deserialize<QueueType[]>(
                "[\"RANKED_SOLO_5x5\", \"NOT_A_REAL_QUEUE_TYPE\", \"RANKED_FLEX_SR\"]");
            CollectionAssert.AreEqual(
                new[] { QueueType.RANKED_SOLO_5x5, QueueType.UNKNOWN, QueueType.RANKED_FLEX_SR },
                result);
        }

        [TestMethod]
        public void KnownValue_RoundTrips()
        {
            var json = JsonSerializer.Serialize(QueueType.RANKED_SOLO_5x5);
            Assert.AreEqual("\"RANKED_SOLO_5x5\"", json);
        }
    }
}
