using CaSessionUtilities.Messaging;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public static class Mapping
{
    public static RdeMessageDecryptionInfo ToRdeMessageDecryptionInfo(this RdeMessageParameters thiz) 
        =>new () {
            Command = Hex.ToHexString(thiz.WrappedCommand),
            PcdPublicKey = Hex.ToHexString(thiz.EphemeralPublicKey)
        };

}