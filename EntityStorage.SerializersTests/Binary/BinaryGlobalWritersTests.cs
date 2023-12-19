using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityStorage.Serializers.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStorage.Serializers.Binary.Tests
{
    [TestClass()]
    public class BinaryGlobalWritersTests
    {
        [TestMethod()]
        public void ReadWriteTest()
        {
            var stream = new MemoryStream();

            BinaryGlobalWriters.Write(stream, 1234);

            stream.Position = 0;
            var r = BinaryGlobalWriters.Read<int>(stream);

            Assert.AreEqual(1234, r);
        }

        [TestMethod()]
        public void StringReadWriteTest()
        {
            var stream = new MemoryStream();

            BinaryGlobalWriters.WriteString(stream, "1234");

            stream.Position = 0;
            var r = BinaryGlobalWriters.ReadString(stream);

            Assert.AreEqual("1234", r);
        }

        [TestMethod()]
        public void DateTimeReadWriteTest()
        {
            var stream = new MemoryStream();

            var dt = DateTime.Now;

            BinaryGlobalWriters.WriteDateTime(stream, dt);

            stream.Position = 0;
            var r = BinaryGlobalWriters.ReadDateTime(stream);

            Assert.AreEqual(dt, r);
        }
    }
}