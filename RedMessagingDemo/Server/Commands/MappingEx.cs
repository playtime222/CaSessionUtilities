using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Commands;

public static class MappingEx
{
    public static ChipAuthenticationProtocolInfo ToCaSessionArgs(this CaSessionUtilities.CaSessionArgs thiz)
        => new()
        {
            ProtocolOid = thiz.ProtocolOid,
            PublicKeyInfo = thiz.PublicKeyInfo.ToPublicKeyInfo()
        };

    public static ChipAuthenticationPublicKeyInfo ToPublicKeyInfo(this CaSessionUtilities.ChipAuthenticationPublicKeyInfo thiz)
        => new()
        {
            PublicKeyBase64 = Base64.ToBase64String(thiz.PublicKey)
        }
    ;
}