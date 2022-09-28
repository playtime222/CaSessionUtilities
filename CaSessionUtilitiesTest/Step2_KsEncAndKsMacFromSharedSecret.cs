using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaSessionUtilities.Wrapping.Implementation;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

//Seed:  8f54c4f830fdd59f0d20f5118d19d9f03295319180da36ba812353b108a9fb43ad8ddb4505da89df111db04ede91934d026a404999fea9d81fabfdcb5df34cc00c6e7f16a7bb50d93ea6461ebbd9ad94
//alg:  AES
//format:  RAW
//raw:  e71666984de8c71293a4b8363850128f9f9350078ae2faaa6dc0da2c464e9ea1
public class Step2_KsEncAndKsMacFromSharedSecret
{
    public const string Case1_KsEnc = "55a5d572e5d4e99625b3fde47388244b8741a7f2a04ac5c7e84f022ad647cea7";
    public const string Case1_KsMac = "3107d894b7ef43f50d7e3da0968373d8340e1adf315dbc92bec118459d494755";
    public const string Case2_KsEnc = "0019666bcd8f18512a34a6ca368c6069b90f35feccbfc980568291266303e7eb";
    public const string Case2_KsMac = "ed93f9068a8153856670d170346460a90bc5e0a644e29f6b100c730753008d4c";
    public const string Case3_KsEnc = "73997b9688979a59ac7bc86320b719d80bd884ae7bea81014b7afa688feef70b";
    public const string Case3_KsMac = "f349280fd9a0d191144b7d0d1a50432c34ea6018951f44f94241e47dfbebc930";

    [InlineData(Step1_SharedSecretGeneration.SharedSecretCase1, Case1_KsEnc, Case1_KsMac)]
    [InlineData(Step1_SharedSecretGeneration.SharedSecretCase2, Case2_KsEnc, Case2_KsMac)]
    [InlineData(Step1_SharedSecretGeneration.SharedSecretCase3, Case3_KsEnc, Case3_KsMac)]
    [Theory]
    public void SeedToKsEnc(string seedHex, string ksEncHex, string ksMacHex)
    {
        var ksEnc = SessionMessagingWrapperKeyUtility.DeriveKey(Hex.Decode(seedHex), new(CipherAlgorithm.Aes, 256), SessionMessagingWrapperKeyUtility.ENC_MODE);
        var ksMac = SessionMessagingWrapperKeyUtility.DeriveKey(Hex.Decode(seedHex), new(CipherAlgorithm.Aes, 256), SessionMessagingWrapperKeyUtility.MAC_MODE);

        Trace.WriteLine("Seed         : " + seedHex);
        Trace.WriteLine("KsEnc Actual : " + Hex.ToHexString(ksEnc));
        Trace.WriteLine("    Expected : " + ksEncHex);
        Trace.WriteLine("KsMac Actual : " + Hex.ToHexString(ksMac));
        Trace.WriteLine("    Expected : " + ksMacHex);

        Assert.Equal(Hex.Decode(ksEncHex), ksEnc);
        Assert.Equal(Hex.Decode(ksMacHex), ksMac);
    }
}