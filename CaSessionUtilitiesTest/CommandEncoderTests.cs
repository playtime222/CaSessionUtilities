using System.Diagnostics;
using CaSessionUtilities;
using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;
namespace CaSessionUtilitiesTest;

public class CommandEncoderTests
{
    /// <summary>
    /// Test cases generated RDE-LIB - nl.rijksoverheid.rdw.rde.TestDataGeneratorTests.AesCommandApduTest //TODO may move...
    /// </summary>
    /// <param name="requestedLength"></param>
    /// <param name="hexPlain"></param>
    /// <param name="hexWrapped"></param>
    /// <param name="ksEncString"></param>
    /// <param name="ksMacString"></param>
    [InlineData(1, "00b08e0001", "0cb08e000d9701018e08fb1cc883d1a9b32d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(65, "00b08e0041", "0cb08e000d9701418e08cea04768ff4fd54d00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(254, "00b08e00fe", "0cb08e000d9701fe8e083fdc947adcea421200", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(255, "00b08e00ff", "0cb08e000d9701ff8e085d83100cf76663d300", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    [InlineData(256, "00b08e0000", "0cb08e000d9701008e08b16e78e0d1d0023f00", "7319D1537EF2FE5CB46AFCFF2DF33B521F3A0C4FA92212D98EB49D9CD6BB8916", "ADCBA368FD14A836908252EF76D09BAD2766C5FFB2FE7857F468676FC4B293E0")]
    //Unlikely the phones handle this cos max transceive length is fixed to 256
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
        var actualPlain = plainApdu.ToArray();
        Trace.WriteLine("act apdu  : " + Hex.ToHexString(actualPlain));
        Trace.WriteLine("exp apdu  : " + hexPlain);
        Assert.Equal(Hex.Decode(hexPlain), actualPlain);
        Trace.WriteLine("");

        var wrapper = new AesSecureMessagingWrapper(Hex.Decode(ksEncString), Hex.Decode(ksMacString));
        var wrapped = new CommandEncoder(wrapper).Encode(plainApdu);

        Trace.WriteLine("data      : " + wrapped.getData().PrettyHexFormat());

        var actual = wrapped.ToArray();

        Trace.WriteLine("");
        Trace.WriteLine("Actual    : " + Hex.ToHexString(actual));
        Trace.WriteLine("Expected  : " + hexWrapped);

        Assert.Equal(Hex.Decode(hexWrapped), actual);
    }

}