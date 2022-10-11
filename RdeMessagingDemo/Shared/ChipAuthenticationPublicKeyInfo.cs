namespace RdeMessagingDemo.Shared;

public class ChipAuthenticationPublicKeyInfo
{
    //[Obsolete("Fallback only?")]
    //public string? Oid { get; init; }

    //EC or ECDH. ASN.1 DER Format
    public string PublicKeyBase64 { get; init; }

    //[Obsolete("Fallback only?")]
    //String? Key OID???
    //public BigInteger? KeyId { get; init; }
}