namespace RedMessagingDemo.Shared;

public record CaSessionArgs
{
    //caSessionArgs.getCaPublicKeyInfo().getKeyId(),
    //caSessionArgs.getCaPublicKeyInfo().getObjectIdentifier(),
    //caSessionArgs.getCaPublicKeyInfo().getSubjectPublicKey());
    //caSessionArgs.getCaInfo().getObjectIdentifier(),
    public string ProtocolOid { get; init; }
    public ChipAuthenticationPublicKeyInfo PublicKeyInfo { get; init; }
}