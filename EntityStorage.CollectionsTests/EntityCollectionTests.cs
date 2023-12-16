using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityStorage.Collections.Tests
{
    [TestClass()]
    public class EntityCollectionTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var collection = new EntityCollection<int>
            {
                1, 2, 3, 4
            };

            Assert.IsTrue(collection.Contains(1));
            Assert.IsTrue(collection.Contains(2));
            Assert.IsTrue(collection.Contains(3));
            Assert.IsTrue(collection.Contains(4));
        }
    }
}