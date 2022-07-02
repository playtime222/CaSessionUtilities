using Org.BouncyCastle.Utilities.Encoders;

namespace RedMessagingDemo.Client.Extras
{
    public static class MappingEx
    {
        public static CaSessionUtilities.CaSessionArgs ToCaSessionArgs(this RedMessagingDemo.Shared.CaSessionArgs thiz)
                => new()
                {
                    ProtocolOid = thiz.ProtocolOid,
                    PublicKeyInfo = thiz.PublicKeyInfo.ToPublicKeyInfo()
                };
        
        public static CaSessionUtilities.ChipAuthenticationPublicKeyInfo ToPublicKeyInfo(this RedMessagingDemo.Shared.ChipAuthenticationPublicKeyInfo thiz)
                => new()
                {
                    PublicKey = Base64.Decode(thiz.PublicKeyBase64)
                }
        ;
    }

}
