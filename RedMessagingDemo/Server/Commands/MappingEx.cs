using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Shared;
using ChipAuthenticationPublicKeyInfo = RedMessagingDemo.Shared.ChipAuthenticationPublicKeyInfo;

namespace RedMessagingDemo.Server.Commands;

public static class MappingEx
{
    public static ChipAuthenticationProtocolInfo ToCaSessionArgs(this CaSessionArgs thiz)
        => new()
        {
            ProtocolOid = thiz.ProtocolOid,
            PublicKeyInfo = thiz.PublicKeyInfo.ToPublicKeyInfo()
        };

    public static ChipAuthenticationPublicKeyInfo ToPublicKeyInfo(this NL.Rijksoverheid.RDW.RDE.CaSessionUtilities.ChipAuthenticationPublicKeyInfo thiz)
        => new()
        {
            PublicKeyBase64 = Base64.ToBase64String(thiz.PublicKey)
        }
    ;
}