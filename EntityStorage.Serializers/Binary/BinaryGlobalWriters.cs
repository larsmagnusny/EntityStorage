using System.Runtime.CompilerServices;

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
    }
}
