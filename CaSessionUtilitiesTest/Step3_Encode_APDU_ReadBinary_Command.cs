using System;
using System.Diagnostics;
using System.Linq;
using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step3_Encode_APDU_ReadBinary_Command
{
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 10,  "0cb08e000d97010a8e08725bea290a7db35100")]
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 100, "0cb08e000d9701648e08065fafbd5326b9b600")]
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 10, "")]
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 100, "")]
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 10, "")]
    //[InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 100, "")]
    [InlineData("New 5", Step2_KsEncAndKsMacFromSharedSecret.Case5_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case5_KsMac, 64, "0CB08E000D9701408E083D9E4F0E1079F77600")]
    [InlineData("New 6", "491d608bd275efded22349fd23a8caf5afb40e73c9f777c8ed138afa940f374d", "51f74babf372099fb6c68f83fa73df5602e120d3d62147581f9391ef6c24154c", 63, "0CB08E000D97013F8E08943DDEC4472584F800")]
    [Theory]
    public void DoIt(string name, String ksEnc, String ksMac, int len, string expected)
    {
        //Note that as the RB command does not have a data parameter, no use is made of KsEnc only ksMac
        var result = CreateEncryptedRbCommand(ksEnc, ksMac, len);
        Trace.WriteLine("KsMac               : " + ksMac);
        var actualHex = Hex.ToHexString(result);
        Trace.WriteLine("Wrapped Command APDU");
        Trace.WriteLine("Actual   : " + actualHex);
        Trace.WriteLine("Expected : " + expected.ToLower());
        Assert.Equal(Hex.Decode(actualHex), result);
    }

    private byte[] CreateEncryptedRbCommand(string ksEncHex, String ksMacHex, int fidByteCount)
    {
        var command = CreateRbCommandApdu(14, fidByteCount); //Using DG14 for all tests
        Trace.WriteLine("Command APDU        : " + Hex.ToHexString(command.ToArray()));
        var ksEnc = Hex.Decode(ksEncHex);
        var ksMac = Hex.Decode(ksMacHex);
        return new CommandEncoder(new AesSecureMessagingWrapper(ksEnc, ksMac)).Encode(command).ToArray();
    }

    private CommandApdu CreateRbCommandApdu(int shortFileId, int fidByteCount)
    {
        int sfi = 0x80 | (shortFileId & 0xFF);
        return new CommandApdu(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, (byte)sfi, 0, fidByteCount);
    }
}