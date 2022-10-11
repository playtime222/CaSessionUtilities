using NL.Rijksoverheid.RDW.RDE.CaSessionUtilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace RedMessagingDemo.Client.Extras
{
    public static class MappingEx
    {
        public static CaSessionArgs ToCaSessionArgs(this RedMessagingDemo.Shared.ChipAuthenticationProtocolInfo thiz)
                => new()
                {
                    ProtocolOid = thiz.ProtocolOid,
                    PublicKeyInfo = new()
                    {
                        PublicKey = Base64.Decode(thiz.PublicKeyInfo.PublicKeyBase64)
                    }
                };
    }

}
