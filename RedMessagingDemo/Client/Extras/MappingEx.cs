using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Client.Extras
{
    public static class MappingEx
    {
        public static CaSessionUtilities.CaSessionArgs ToCaSessionArgs(this ChipAuthenticationProtocolInfo thiz)
                => new()
                {
                    ProtocolOid = thiz.ProtocolOid,
                    PublicKeyInfo = thiz.PublicKeyInfo.ToPublicKeyInfo()
                };
        
        public static CaSessionUtilities.ChipAuthenticationPublicKeyInfo ToPublicKeyInfo(this ChipAuthenticationPublicKeyInfo thiz)
                => new()
                {
                    PublicKey = Base64.Decode(thiz.PublicKeyBase64)
                }
        ;
    }

}
