namespace CaSessionUtilities.Wrapping;

public class DESedeSecureMessagingWrapper : SecureMessagingWrapper
{
    private static readonly byte[] Iv = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
        : base(ksEnc, ksMac, "DESede/CBC/NoPadding", "ISO9797Alg3Mac")
    {
    }

    public override int BlockSize => 8;

    public override byte[] CalculateMac(byte[] data) => Crypto.GetIso9797Alg3Mac(KsMac, data);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="ssc">Unused here.</param>
    /// <returns></returns>
    public override byte[] GetEncodedDataForResponse(byte[] response, long ssc)
        => Crypto.GetDeSedeCbcNoPaddingCipherText(KsEnc, Iv, response);
    
    public override byte[] GetEncodedSendSequenceCounter(long ssc)
        => new byte[] {  0, 0, 0, 0, 0, 0, 0, (byte)ssc };
}