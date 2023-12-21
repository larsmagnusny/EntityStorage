using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace EntityStorage.Serializers.Binary
{
    public static class BinaryGlobalWriters
    {
        public unsafe static int Write<T>(Stream stream, T value) where T : unmanaged
        {
            var size = sizeof(T);
            var buf = stackalloc byte[size];

            Unsafe.WriteUnaligned(buf, value);

            stream.Write(new Span<byte>(buf, size));

            return size;
        }

        public unsafe static T Read<T>(Stream stream) where T : unmanaged
        {
            var size = sizeof(T);
            var buf = stackalloc byte[size];

            stream.Read(new Span<byte>(buf, size));

            return Unsafe.ReadUnaligned<T>(buf);
        }

        public unsafe static int WriteString(Stream stream, string value)
        {
            var span = MemoryMarshal.Cast<char, byte>(value);

            Write(stream, span.Length);
            stream.Write(span);
            return span.Length;
        }

        public unsafe static string ReadString(Stream stream)
        {
            var len = Read<int>(stream);

            var buffer = new byte[len];

            stream.Read(buffer.AsSpan());

            return new string(MemoryMarshal.Cast<byte, char>(buffer));
        }

        public static int WriteDateTime(Stream stream, DateTime value)
        {
            return Write(stream, value.ToBinary());
        }

        public static DateTime ReadDateTime(Stream stream)
        {
            var val = Read<long>(stream);
            return DateTime.FromBinary(val);
        }

        public unsafe static int WriteGuid(Stream stream, Guid value)
        {
            var bytes = stackalloc byte[16];
            var span = new Span<byte>(bytes, 16);
            value.TryWriteBytes(span);

            stream.Write(span);

            return 16;
        }

        public unsafe static Guid ReadGuid(Stream stream)
        {
            var bytes = stackalloc byte[16];
            var span = new Span<byte>(bytes, 16);
            stream.Read(span);

            return new Guid(span);
        }
    }
}
