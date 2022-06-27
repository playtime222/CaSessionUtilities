namespace CaSessionUtilities;

public class TLVUtil
{
    public static byte[] wrapDO(int tag, byte[] data /*encoded Le*/)
    {
        using var ms = new MemoryStream();
        var tlvOutputStream = new TLVOutputStream(ms);
        tlvOutputStream.writeTag(tag);
        tlvOutputStream.writeValue(data);
        tlvOutputStream.flush();
        tlvOutputStream.close();
        return ms.ToArray();
    }

    public static byte[] getLengthAsBytes(int length)
    {
        var ms = new MemoryStream();
        if (length < 0x80)
        {
            /* short form */
            ms.WriteByte((byte)length);
        }
        else
        {
            int byteCount = log(length, 256);
            ms.WriteByte((byte)(0x80 | byteCount));
            for (int i = 0; i < byteCount; i++)
            {
                int pos = 8 * (byteCount - i - 1);
                ms.WriteByte((byte)((length & (0xFF << pos)) >> pos));
            }
        }
        return ms.ToArray();
    }


    private static int log(int n, int b)
    {
        int result = 0;
        while (n > 0)
        {
            n = n / b;
            result++;
        }
        return result;
    }

    public static byte[] getTagAsBytes(int tag)
    {
        var ms = new MemoryStream();
        int byteCount = (int)(Math.Log(tag) / Math.Log(256)) + 1;
        for (int i = 0; i < byteCount; i++)
        {
            int pos = 8 * (byteCount - i - 1);
            ms.WriteByte((byte)((tag & (0xFF << pos)) >> pos));
        }
        var tagBytes = ms.ToArray();
        switch (getTagClass(tag))
        {
            case ASN1Constants.APPLICATION_CLASS:
                tagBytes[0] |= 0x40;
                break;
            case ASN1Constants.CONTEXT_SPECIFIC_CLASS:
                tagBytes[0] |= 0x80;
                break;
            case ASN1Constants.PRIVATE_CLASS:
                tagBytes[0] |= 0xC0;
                break;
            default:
                /* NOTE: Unsupported tag class. Now what? */
                break;
        }
        if (!isPrimitive(tag))
        {
            tagBytes[0] |= 0x20;
        }
        return tagBytes;
    }

    static int getTagClass(int tag)
    {
        int i = 3;
        for (; i >= 0; i--)
        {
            int mask = (0xFF << (8 * i));
            if ((tag & mask) != 0x00)
            {
                break;
            }
        }
        int msByte = (((tag & (0xFF << (8 * i))) >> (8 * i)) & 0xFF);
        switch (msByte & 0xC0)
        {
            case 0x00:
                return ASN1Constants.UNIVERSAL_CLASS;
            case 0x40:
                return ASN1Constants.APPLICATION_CLASS;
            case 0x80:
                return ASN1Constants.CONTEXT_SPECIFIC_CLASS;
            case 0xC0:
            default:
                return ASN1Constants.PRIVATE_CLASS;
        }
    }

    public static bool isPrimitive(int tag)
    {
        int i = 3;
        for (; i >= 0; i--)
        {
            int mask = (0xFF << (8 * i));
            if ((tag & mask) != 0x00)
            {
                break;
            }
        }
        int msByte = (((tag & (0xFF << (8 * i))) >> (8 * i)) & 0xFF);
        return ((msByte & 0x20) == 0x00);
    }
}