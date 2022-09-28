using System;
using System.Diagnostics;
using System.Linq;
using CaSessionUtilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step5_EncryptionKeyFromRbResponse
{
    public const string Case1_Length10_SecretKey = "4f9caea2a5c32ef15bae301cc1ca46719da563d81221ffe9b2e3f8e40411588e";
    public const string Case1_Length100_SecretKey = "9a41a5265679e20195601d16be9144585ab8805353498938c89550f3489460ed";
    public const string Case2_Length10_SecretKey = "1221a52eac456c8bc850cd76cf7d710f228b02fd7da57685fa12ee951073ab94";
    public const string Case2_Length100_SecretKey = "6bf87de30190bc7e1fcc59aea10b1da8a9cf3ba30702e9c1f729a0d974c07503";
    public const string Case3_Length10_SecretKey = "5e3627c1c8fba6035d8771a0f5c2f8ca81d25518398aa8a50f23e7dfe202bc06";
    public const string Case3_Length100_SecretKey = "f39ceacc1e426ddc47cd10e71548ec7e05ef1ea165d97e4c6f762dccb42fa71b";

    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case1_RB_Length10, Case1_Length10_SecretKey)]
    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case1_RB_Length100, Case1_Length100_SecretKey)]
    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case2_RB_Length10, Case2_Length10_SecretKey)]
    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case2_RB_Length100, Case2_Length100_SecretKey)]
    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case3_RB_Length10, Case3_Length10_SecretKey)]
    [InlineData(Step4_Simulate_APDU_ReadBinary_DG14.Case3_RB_Length100, Case3_Length100_SecretKey)]
    [Theory]
    public void Convert(string responseHex, string expectedKeyHex) 
    {
        var key = Crypto.GetAes256SecretKeyFromResponse(Hex.Decode(responseHex));
        Trace.WriteLine("RB Result   : " + responseHex);
        Trace.WriteLine("Key         : " + Hex.ToHexString(key));
        Trace.WriteLine("Expected Key: " + expectedKeyHex);
        Assert.Equal(Hex.Decode(expectedKeyHex), key);
    }
}