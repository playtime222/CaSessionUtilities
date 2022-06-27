using System.Diagnostics;
using CaSessionUtilities;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class ResponseEncoderTests
{
    //From SPECI2014 passport
    private const string HexEncodedDg14 = "6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396";

    [InlineData("7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0", 1, "8711016D9F6F6FDA79FF285C2C1D3AEA1FFD03990290008E089C3B7B89BB7849929000")]
    //TODO more!!!!!
    [Theory]
    private void Write(string ksEncString, string ksMacString, int requestedLength, string expectedWrappedResponse)
    {
        var ksEnc = Hex.Decode(ksEncString);
        var ksMac = Hex.Decode(ksMacString);
        var encoder = new AesSecureMessagingWrapperResponseEncoder(ksEnc, ksMac);
        var result = encoder.Write(Arrays.CopyOf(Hex.Decode(HexEncodedDg14), requestedLength));

        Trace.WriteLine("Actual  : " + Hex.ToHexString(result));
        Trace.WriteLine("Expected: " + expectedWrappedResponse.ToLower());
        Assert.Equal(Hex.Decode(expectedWrappedResponse), result);
    }
}


public class CommandEncoderTests
{
    private byte[] actualPlain;

    [InlineData(1,   "00b08e0001",     "0cb08e000d9701018e08fb1cc883d1a9b32d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(65,  "00b08e0041",     "0cb08e000d9701418e08cea04768ff4fd54d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(254, "00b08e00fe",     "0cb08e000d9701fe8e083fdc947adcea421200", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(255, "00b08e00ff",     "0cb08e000d9701ff8e085d83100cf76663d300", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(256, "00b08e0000",     "0cb08e000d9701008e08b16e78e0d1d0023f00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(257, "00b08e00000101", "0cb08e0000000e970201018e08be975ead600e26f70000", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(300, "00b08e0000012c", "0cb08e0000000e9702012c8e08bd308fb58e028d140000", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [Theory]
    private void Write(int requestedLength, string hexPlain, string hexWrapped, string ksEncString, string ksMacString)
    {
        Trace.WriteLine("KsEnc     : " + ksEncString);
        Trace.WriteLine("KsMac     : " + ksMacString);
        Trace.WriteLine("Length    : " + requestedLength);
        Trace.WriteLine("");

        const int shortFileId = 14;

        var sfi = 0x80 | (shortFileId & 0xFF);
        var plainApdu = new CommandAPDU(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, sfi, 0, requestedLength);
        actualPlain = plainApdu.ToArray();
        Trace.WriteLine("act apdu  : " + Hex.ToHexString(actualPlain));
        Trace.WriteLine("exp apdu  : " + hexPlain);
        Assert.Equal(Hex.Decode(hexPlain), actualPlain);
        Trace.WriteLine("");

        var wrapper = new AESSecureMessagingWrapper(Hex.Decode(ksEncString), Hex.Decode(ksMacString), 0);
        var wrapped = wrapper.wrap(plainApdu);

        Trace.WriteLine("data      : " + wrapped.getData().PrettyHexFormat());
        
        var actual = wrapped.ToArray();

        Trace.WriteLine("");
        Trace.WriteLine("Actual    : " + Hex.ToHexString(actual));
        Trace.WriteLine("Expected  : " + hexWrapped);

        Assert.Equal(Hex.Decode(hexWrapped), actual);
    }

    //[InlineData(1, "0101")]
    //[InlineData(65, "0141")]
    //[InlineData(254, "01fe")]
    //[InlineData(255, "01ff")]
    //[InlineData(256, "0100")]
    //[InlineData(257, "020101")]
    //[InlineData(300, "02012c")]
    //[Theory]
    //public void Le(int requestedLength, string hexPlain)
    //{
    //    var result = SecureMessagingWrapper.encodeLe(requestedLength);
    //    Trace.WriteLine("Actual  : " + Hex.ToHexString(result));
    //    Trace.WriteLine("Expected: " + hexPlain);

    //    Assert.Equal(Hex.Decode(hexPlain), result);
    //}


    [InlineData("00", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0", "69fb0629543d3ac966ca0b39d795f182")]
    [InlineData("00010203", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0", "06981c1a7f9bf61d7d12f1bf4e65de27")]
    [Theory]
    private void AesCmac(string buffer, string ksMac, string expected)
    {
        var result = Crypto.getMac("AESCMAC", Hex.Decode(ksMac), Hex.Decode(buffer));
        Assert.Equal(Hex.Decode(expected), result);
    }
}