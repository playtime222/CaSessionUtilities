namespace RedMessagingDemo.Shared;

public record DocumentSecurityObjectInfo
{
    //EC or ECDH, ASN.1 DER?
    public string VerficationPublicKeyBase64 { get; init; }
}