using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
using Org.BouncyCastle.Utilities.Encoders;
using RdeMessagingDemo.Shared;
using ChipAuthenticationPublicKeyInfo = RdeMessagingDemo.Shared.ChipAuthenticationPublicKeyInfo;

namespace RdeMessagingDemo.Server.Commands;

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