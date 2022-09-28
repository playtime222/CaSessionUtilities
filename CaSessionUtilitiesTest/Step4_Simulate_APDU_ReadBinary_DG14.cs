using System;
using System.Diagnostics;
using System.Linq;
using CaSessionUtilities.Wrapping;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step4_Simulate_APDU_ReadBinary_DG14
{
    public const string Case1_RB_Length10 = "8711011342ceb25180b830b9d709ce38f73321990290008e0870f137bd85dcca6b9000";
    public const string Case1_RB_Length100 = "8771011f7aaf02f3df52cfcdfb38fbb37aa55f743a8b79edc11d9cf1ce0da10c277611951c2af8d29497939f78c4816b6c0169a0a7b02e59d6702defa176312c43ceb3dddc749c42c34f289f397e6ae150b93c2270b25e7ff8a4f43a8b00046e626350299b8d3b457481a11a807c0969da3cf8990290008e082bdd07f44a1e3aea9000";
    public const string Case2_RB_Length10 = "87110114f49c70e1994c1a41d9abb7f8a017db990290008e080edb80e3652153869000";
    public const string Case2_RB_Length100 = "8771011fe8fb70b52d86ebfe51a15ac340646b529a2e4e95711410865769015cdfaa8f259049c6d6d51a6e64fb9608278de1c383e844e4824b2c861cd4e43e9ac8cf94077349c4d348d21281d928e4a4324a8effa785934e717743129c485c5b826b95b08aeccae0ee6cfd635e4a908bd8ca64990290008e088010e187203d9a739000";
    public const string Case3_RB_Length10 = "8711013355dcb2c385b18fa1e3695dffe9d865990290008e08e8b2e9db387c8ac69000";
    public const string Case3_RB_Length100 = "8771018ff89cd5f96169ae5abcf43a4dc5dec89c1df82be43c2f910b4b1089b943e5b7001b22484b4df3a876745f72e16dccb7b6973c805343116e63fff0792aea81fa6c31ee1a94478387cc3f2841e97b2252c9cd4b1c11a37fca41cae0e039005b72788bf6b5eb081dc2a11cedd003ee4491990290008e08fcddab640c22d1989000";

    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 10, Case1_RB_Length10)]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case1_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case1_KsMac, 100, Case1_RB_Length100)]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 10, Case2_RB_Length10)]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case2_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case2_KsMac, 100, Case2_RB_Length100)]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 10, Case3_RB_Length10)]
    [InlineData(Step2_KsEncAndKsMacFromSharedSecret.Case3_KsEnc, Step2_KsEncAndKsMacFromSharedSecret.Case3_KsMac, 100, Case3_RB_Length100)]

    //From Android app responses
    [InlineData("CF1D8F4D1450D3ADE8A44E8F5737E9DDCBE5CD614CE0147185510FD8C35AD12F", "C9C8E169D1FDDBC1A186AB98552A5F5C146D84315F3C8AB8A9DDE6AA5B64105F", 64, "87510132355228C6B16E4A06B8934E4C235067A5D6D3855FBF8B912559E25F4C8F78693425CFFF0AB906C8F1F4DE082EBCD20DD07B13BDDD0B47BA89EDB6C0778F95560EFD66CEE04E35C831442EE9A90FAA42990290008E086618B0A723E0D1769000")]
    [InlineData("0B89852F4866E35BED4BB7B3C271742A7E73F179CEFFEDD1F18A69A59A04251C", "D8F7140D5AA85134507F6D298B760A7CFC8F1C92C600BEC55E963BC5C4D4FC0D", 1, "8711010C1A91577955228ADB5AA0F4FAADF15D990290008E08F702D6A477B6165A9000")]
    [InlineData("9DFA50578F1F91EF61B0EA9C02AD7297570DAB7B4EBCAF40FE5F004296B89A73", "4B84CA4A4492F6190B07ED0E4A00F0AE7046E165C25C60C51D7C7025A04E250B", 15, "8711010D9B7C00D2767DE181AD9616A528A985990290008E0801BC2DCC3686E4E79000")]
    [Theory]
    public void EncodeResponse(string ksEncString, string ksMacString, int requestedLength, string expectedWrappedResponse)
    {
        var ksEnc = Hex.Decode(ksEncString);
        var ksMac = Hex.Decode(ksMacString);
        var encoder = new ResponseEncoder(new AesSecureMessagingWrapper(ksEnc, ksMac));
        var result = encoder.Write(Arrays.CopyOf(Hex.Decode(Spec2014Content.DG14Hex), requestedLength));

        Trace.WriteLine("Actual  : " + Hex.ToHexString(result));
        Trace.WriteLine("Expected: " + expectedWrappedResponse.ToLower());

        var encodedActual = Hex.ToHexString(result);

        Assert.StartsWith("87", encodedActual); //Start tag
        var substring = encodedActual.Substring(encodedActual.Length - 32); //9902 + 9000 + 8e0b + 8 bytes (= 16 char) + 9000 again
        Debug.WriteLine($"Mac and block delimiter extract: {substring}");
        Assert.StartsWith("990290008e08", substring); //End of data block, start of MAC
        Assert.EndsWith("9000", substring); //Ends of block

        //TODO length block at start

        Assert.Equal(Hex.Decode(expectedWrappedResponse), result);
    }
}