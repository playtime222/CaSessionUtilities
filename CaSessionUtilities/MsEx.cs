namespace CaSessionUtilities;

public static class MsEx
{
    public static void write(this MemoryStream thiz, int value) => thiz.WriteByte((byte)value);
    public static void write(this MemoryStream thiz, byte value) => thiz.WriteByte(value);
    public static void write(this MemoryStream thiz, byte[] value) => thiz.Write(value);
    public static void write(this MemoryStream thiz, byte[] value, int start, int len) => thiz.Write(value, start, len);
}