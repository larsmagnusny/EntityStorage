using EntityStorage.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace EntityStorage.Collections.Tests
{
    public class User
    {
        [Key(AutoIncrement = true)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestClass()]
    public class InMemoryEntityCollectionTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var collection = new InMemoryEntityCollection<User>
            {
                new User { Name = "Lars" },
                new User { Name = "Charlotte" },
            };

            Assert.IsTrue(collection.Contains(new User { Id = 0, Name = "Lars"}));
            Assert.IsTrue(collection.Contains(new User { Id = 1, Name = "Charlotte" }));
        }


        [TestMethod()]
        public void FindTest()
        {
            var collection = new InMemoryEntityCollection<User>
            {
                new User { Name = "Lars" },
                new User { Name = "Charlotte" },
            };

            var lars = collection.Find(o => o.Id == 0);
            var charlotte = collection.Find(o => o.Id == 1);

            Assert.IsTrue(lars.First().Id == 0 && lars.First().Name == "Lars");
            Assert.IsTrue(charlotte.First().Id == 1 && charlotte.First().Name == "Charlotte");


            var lars2 = collection.Find(o => o.Name == "Lars");
            var charlotte2 = collection.Find(o => o.Id == 1);
            Assert.IsTrue(lars2.First().Id == 0 && lars2.First().Name == "Lars");
            Assert.IsTrue(charlotte2.First().Id == 1 && charlotte2.First().Name == "Charlotte");
        }


        [TestMethod()]
        public async Task FindFirstNonIndexedTest_ExtendedAsync()
        {
            var collection = new InMemoryEntityCollection<User>();

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("EntityStorage.CollectionsTests.users.txt");

            using var streamReader = new StreamReader(stream);

            List<string> names = new();
            string? line = default;
            do
            {
                line = streamReader.ReadLine();

                names.Add(line);
                collection.Add(new User { Name = line });
            } while (line is not null);

            foreach (var name in names) {
                Assert.IsTrue(collection.FindFirst(o => o.Name == name, out var entity));
                Assert.AreEqual(name, entity.Name);
            }
        }

        [TestMethod()]
        public async Task FindFirstIndexedTest_ExtendedAsync()
        {
            var collection = new InMemoryEntityCollection<User>();

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("EntityStorage.CollectionsTests.users.txt");

            using var streamReader = new StreamReader(stream);

            List<string> names = new();
            string? line = default;
            do
            {
                line = streamReader.ReadLine();

                names.Add(line);
                collection.Add(new User { Name = line });
            } while (line is not null);

            //var count = 0;
            //foreach(var item in collection.Find(o => o.Id >= 0 && o.Id < collection.Count))
            //{
            //    Assert.AreEqual(count++, item.Id);
            //}
        }
    }
}