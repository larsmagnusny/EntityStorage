using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityStorage.Serializers.Binary.Tests
{
    public record ValueTypesTest
    {
        public int Int { get; set; }
        public float Float { get; set; }
        public byte Byte { get; set; }
    }

    public record MixedTypesTest
    {
        public int Int { get; set; }
        public float Float { get; set; }
        public byte Byte { get; set; }
        public string String { get; set; }
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
    }

    [TestClass()]
    public class BinaryMaterializerTests
    {
        [TestMethod()]
        public void ValueCreateWriterTest()
        {
            var data = new ValueTypesTest
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<ValueTypesTest>();

            Assert.IsNotNull(writer);

            writer(stream, 0, data);

            Assert.AreEqual(4 + 4 + 1, stream.Length);
        }


        [TestMethod()]
        public void ValueCreateWriterReaderTest()
        {
            var data = new ValueTypesTest
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<ValueTypesTest>();
            var reader = BinaryMaterializer.CreateReader<ValueTypesTest>();

            Assert.IsNotNull(writer);
            Assert.IsNotNull(reader);

            writer(stream, 0, data);

            stream.Position = 0;

            var ret = reader(stream, 0);

            Assert.AreEqual(data.Int, ret.Int);
            Assert.AreEqual(data.Float, ret.Float);
            Assert.AreEqual(data.Byte, ret.Byte);
        }

        [TestMethod()]
        public void MixedCreateWriterTest()
        {
            var data = new MixedTypesTest
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12,
                String = "1234",
                DateTime = DateTime.Now,
                Guid = Guid.NewGuid()
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<MixedTypesTest>();

            Assert.IsNotNull(writer);

            writer(stream, 0, data);

            Assert.AreEqual(4 + 4 + 1 + 8 + 4 + 8 + 16, stream.Length);
        }

        [TestMethod()]
        public void MixedCreateWriterReaderTest()
        {
            var data = new MixedTypesTest
            {
                Int = 1,
                Float = 1.23f,
                Byte = 12,
                String = "1234",
                DateTime = DateTime.Now,
                Guid = Guid.NewGuid()
            };

            var stream = new MemoryStream();
            var writer = BinaryMaterializer.CreateWriter<MixedTypesTest>();
            var reader = BinaryMaterializer.CreateReader<MixedTypesTest>();

            Assert.IsNotNull(writer);
            Assert.IsNotNull(reader);

            writer(stream, 0, data);

            stream.Position = 0;

            var ret = reader(stream, 0);

            Assert.AreEqual(data.Int, ret.Int);
            Assert.AreEqual(data.Float, ret.Float);
            Assert.AreEqual(data.Byte, ret.Byte);
            Assert.AreEqual(data.String, ret.String);
            Assert.AreEqual(data.DateTime, ret.DateTime);
            Assert.AreEqual(data.Guid, ret.Guid);
        }
    }
}