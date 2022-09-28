using System;
using System.Diagnostics;
using System.Linq;
using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step3_Encode_APDU_ReadBinary_Command
{
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 10, "0cb08e000d97010a8e08725bea290a7db35100")]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 100, "0cb08e000d9701648e08065fafbd5326b9b600")]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 10, "")]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 100, "")]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 10, "")]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 100, "")]
    [Theory]
    public void DoIt(String ksEnc, String ksMac, int len, string expected)
    {
        //Note that as the RB command does not have a data parameter, no use is made of KsEnc only ksMac
        var result = CreateEncryptedRbCommand(ksEnc, ksMac, len);
        Trace.WriteLine("KsMac               : " + ksMac);
        var actualHex = Hex.ToHexString(result);
        Trace.WriteLine("Wrapped Command APDU: " + actualHex);
        var l = actualHex.Length - "e2db7bbfe2a5434300".Length; //Ignore the mac and end 00
        Assert.Equal(actualHex.Substring(0, l), expected.Substring(0, l));
        Assert.True(actualHex.EndsWith("00"));
        //Assert.Equal(actualHex, expected);
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