using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityStorage.Serializers.Binary.Tests
{
    public record Test
    {
        public int Int { get; set; }
        public float Float { get; set; }
        public byte Byte { get; set; }
    }

    [TestClass()]
    public class BinaryMaterializerTests
    {
        [TestMethod()]
        public void CreateWriterTest()
        {
            var data = new Test
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<Test>();

            Assert.IsNotNull(writer);

            writer(stream, 0, data);

            Assert.AreEqual(4 + 4 + 1, stream.Length);
        }


        [TestMethod()]
        public void CreateWriterReaderTest()
        {
            var data = new Test
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<Test>();
            var reader = BinaryMaterializer.CreateReader<Test>();

            Assert.IsNotNull(writer);
            Assert.IsNotNull(reader);

            writer(stream, 0, data);

            stream.Position = 0;

            var ret = reader(stream, 0);

            Assert.AreEqual(data.Int, ret.Int);
            Assert.AreEqual(data.Float, ret.Float);
            Assert.AreEqual(data.Byte, ret.Byte);
        }
    }
}