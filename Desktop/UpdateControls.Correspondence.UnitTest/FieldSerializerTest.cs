using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.FieldSerializer;
using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class FieldSerializerTest
    {
        [TestMethod]
        public void ByteTest()
        {
            ByteFieldSerializer serializer = new ByteFieldSerializer();

            TestSerializer((byte)0, new byte[] { 0x00 }, serializer);
            TestSerializer((byte)1, new byte[] { 0x01 }, serializer);
            TestSerializer((byte)127, new byte[] { 0x7f }, serializer);
            TestSerializer((byte)128, new byte[] { 0x80 }, serializer);
            TestSerializer((byte)255, new byte[] { 0xff }, serializer);
        }

        [TestMethod]
        public void IntTest()
        {
            IntFieldSerializer serializer = new IntFieldSerializer();

            TestSerializer((int)0, new byte[] { 0x00, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((int)1, new byte[] { 0x00, 0x00, 0x00, 0x01 }, serializer);
            TestSerializer((int)127, new byte[] { 0x00, 0x00, 0x00, 0x7f }, serializer);
            TestSerializer((int)128, new byte[] { 0x00, 0x00, 0x00, 0x80 }, serializer);
            TestSerializer((int)255, new byte[] { 0x00, 0x00, 0x00, 0xff }, serializer);
            TestSerializer((int)256, new byte[] { 0x00, 0x00, 0x01, 0x00 }, serializer);
            TestSerializer((int)65535, new byte[] { 0x00, 0x00, 0xff, 0xff }, serializer);
            TestSerializer((int)65536, new byte[] { 0x00, 0x01, 0x00, 0x00 }, serializer);
            TestSerializer((int)16777215, new byte[] { 0x00, 0xff, 0xff, 0xff }, serializer);
            TestSerializer((int)16777216, new byte[] { 0x01, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((int)int.MaxValue, new byte[] { 0x7f, 0xff, 0xff, 0xff }, serializer);

            TestSerializer((int)-1, new byte[] { 0xff, 0xff, 0xff, 0xff }, serializer);
            TestSerializer((int)-127, new byte[] { 0xff, 0xff, 0xff, 0x81 }, serializer);
            TestSerializer((int)-128, new byte[] { 0xff, 0xff, 0xff, 0x80 }, serializer);
            TestSerializer((int)-255, new byte[] { 0xff, 0xff, 0xff, 0x01 }, serializer);
            TestSerializer((int)-256, new byte[] { 0xff, 0xff, 0xff, 0x00 }, serializer);
            TestSerializer((int)-65535, new byte[] { 0xff, 0xff, 0x00, 0x01 }, serializer);
            TestSerializer((int)-65536, new byte[] { 0xff, 0xff, 0x00, 0x00 }, serializer);
            TestSerializer((int)-16777215, new byte[] { 0xff, 0x00, 0x00, 0x01 }, serializer);
            TestSerializer((int)-16777216, new byte[] { 0xff, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((int)int.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00 }, serializer);
        }

        [TestMethod]
        public void LongTest()
        {
            LongFieldSerializer serializer = new LongFieldSerializer();

            TestSerializer((long)0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((long)1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, serializer);
            TestSerializer((long)127, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f }, serializer);
            TestSerializer((long)128, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 }, serializer);
            TestSerializer((long)255, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff }, serializer);
            TestSerializer((long)256, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 }, serializer);
            TestSerializer((long)65535, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff }, serializer);
            TestSerializer((long)65536, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 }, serializer);
            TestSerializer((long)16777215, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff }, serializer);
            TestSerializer((long)16777216, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((long)int.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff }, serializer);
            TestSerializer((long)long.MaxValue, new byte[] { 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, serializer);

            TestSerializer((long)-1, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, serializer);
            TestSerializer((long)-127, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x81 }, serializer);
            TestSerializer((long)-128, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x80 }, serializer);
            TestSerializer((long)-255, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 }, serializer);
            TestSerializer((long)-256, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00 }, serializer);
            TestSerializer((long)-65535, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x01 }, serializer);
            TestSerializer((long)-65536, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00 }, serializer);
            TestSerializer((long)-16777215, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x01 }, serializer);
            TestSerializer((long)-16777216, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((long)int.MinValue, new byte[] { 0xff, 0xff, 0xff, 0xff, 0x80, 0x00, 0x00, 0x00 }, serializer);
            TestSerializer((long)long.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, serializer);
        }

        [TestMethod]
        public void CharTest()
        {
            CharFieldSerializer serializer = new CharFieldSerializer();

            TestSerializer((char)' ', new byte[] { 0x20 }, serializer);
            TestSerializer((char)'a', new byte[] { 0x61 }, serializer);
            TestSerializer((char)'A', new byte[] { 0x41 }, serializer);
            TestSerializer((char)'@', new byte[] { 0x40 }, serializer);
            TestSerializer((char)'2', new byte[] { 0x32 }, serializer);
            TestSerializer((char)':', new byte[] { 0x3a }, serializer);
            TestSerializer((char)'\r', new byte[] { 0x0d }, serializer);
            TestSerializer((char)'\n', new byte[] { 0x0a }, serializer);
            TestSerializer((char)'\x7ef', new byte[] { 0xdf, 0xaf }, serializer);
            TestSerializer((char)'\x8119', new byte[] { 0xe8, 0x84, 0x99 }, serializer);
        }

        [TestMethod]
        public void StringTest()
        {
            StringFieldSerializer serializer = new StringFieldSerializer();

            // Special case: null equals empty string.
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter output = new BinaryWriter(stream))
                {
                    serializer.WriteData(output, null);
                }
                byte[] written = stream.ToArray();
                Assert.IsTrue(ArraysEqual(new byte[] { 0x00, 0x00 }, written));
            }

            TestSerializer(string.Empty, new byte[] { 0x00, 0x00 }, serializer);

            TestSerializer("Hello, World!", new byte[] { 0x00, 0x0d, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x2c, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21 }, serializer);
            TestSerializer("\x7ef", new byte[] { 0x00, 0x02, 0xdf, 0xaf }, serializer);
            TestSerializer("\x8119", new byte[] { 0x00, 0x03, 0xe8, 0x84, 0x99 }, serializer);
        }

        private static void TestSerializer(object value, byte[] data, IFieldSerializer serializer)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
            {
                object read = serializer.ReadData(reader);
                Assert.AreEqual(value, read);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter output = new BinaryWriter(stream))
                {
                    serializer.WriteData(output, value);
                }
                byte[] written = stream.ToArray();
                Assert.IsTrue(ArraysEqual(data, written));
            }
        }

        public static bool ArraysEqual(byte[] first, byte[] second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
