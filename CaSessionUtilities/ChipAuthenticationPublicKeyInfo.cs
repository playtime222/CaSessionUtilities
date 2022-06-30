using Org.BouncyCastle.Math;

namespace CaSessionUtilities;

/// <summary>
/// All from DG14. This is the dto.
/// </summary>
public record ChipAuthenticationPublicKeyInfo
{
    [Obsolete("Fallback only?")]
    public string Oid { get; init; }

    //EC or ECDH, ASN.1 DER?
    public byte[] PublicKey { get; init; }


    [Obsolete("Fallback only?")]
    //String? Key OID???
    public BigInteger KeyId { get; init; }
}