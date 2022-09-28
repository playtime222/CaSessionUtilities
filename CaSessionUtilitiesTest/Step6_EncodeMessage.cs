using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CaSessionUtilities.Messaging;
using CaSessionUtilities.Messaging.ZipV2;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step6_EncodeMessage
{
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case1_Length10_SecretKey)]
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case1_Length100_SecretKey)]
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case2_Length10_SecretKey)]
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case2_Length100_SecretKey)]
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case3_Length10_SecretKey)]
    [InlineData(Step5_EncryptionKeyFromRbResponse.Case3_Length100_SecretKey)]
    [Theory]
    public void Encode(string secretKeyHex)
    {
        var messageContentArgs = new MessageContentArgs();
        messageContentArgs.Add(new FileArgs("argle", Encoding.UTF8.GetBytes("argle...")));
        messageContentArgs.UnencryptedNote = "note";

        var rdeMessageDecryptionInfo = new RdeMessageDecryptionInfo
        {
            Command = "Now!",
            PcdPublicKey = "01"
        };

        var secretKey = Hex.Decode(secretKeyHex);
        var encoder = new ZipMessageEncoder();
        var encoded = encoder.Encode(messageContentArgs, rdeMessageDecryptionInfo, secretKey);

        Trace.WriteLine(Hex.ToHexString(encoded));

        File.WriteAllBytes("D:\\CSharpMessage.zip", encoded);
        new Step7_DecodeMessage().Decode(secretKeyHex, Hex.ToHexString(encoded));
    }
}