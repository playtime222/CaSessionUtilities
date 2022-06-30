using CaSessionUtilities;
using Org.BouncyCastle.Utilities;

public class DESedeSecureMessagingWrapper : SecureMessagingWrapper
{
    private static readonly byte[] ZERO_IV_PARAM_SPEC = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
        : base(ksEnc, ksMac, "DESede/CBC/NoPadding", "ISO9797Alg3Mac")
    {
    }

    public override int BlockSize => 8;

    public override byte[] CalculateMac(byte[] data) => Crypto.getISO9797Alg3Mac(KsMac, data);

    public override byte[] GetEncodedDataForResponse(byte[] response)
        => Crypto.getDESedeCBCNoPaddingCipherText(KsEnc, ZERO_IV_PARAM_SPEC, response);
    
    public override byte[] GetEncodedSendSequenceCounter(long ssc)
        => new byte[] {  0, 0, 0, 0, 0, 0, 0, (byte)ssc };
}
