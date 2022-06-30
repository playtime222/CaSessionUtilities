namespace CaSessionUtilities.Wrapping;

public class AesSecureMessagingWrapper : SecureMessagingWrapper
{
    public AesSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
    : base(ksEnc, ksMac, "AES/CBC/NoPadding", "AESCMAC")
    {
    }

    public override int BlockSize => 16;

    public override byte[] CalculateMac(byte[] data)
        => Crypto.GetAesCMac(KsMac, data);

    public override byte[] GetEncodedDataForResponse(byte[] response, long ssc)
    {
        var iv = Crypto.GetAesEcbNoPaddingCipherText(KsEnc, GetEncodedSendSequenceCounter(2)); //Contains state -> 2 the SSC counter...
        return Crypto.GetAesCbcNoPaddingCipherText(KsEnc, iv, response);
    }

    public override byte[] GetEncodedSendSequenceCounter(long ssc) 
        => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)ssc };

}
