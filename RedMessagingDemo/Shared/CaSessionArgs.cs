namespace RedMessagingDemo.Shared;


/// <summary>
/// Arguments required from the MRTD DG14
/// </summary>
public record CaSessionArgs
{
    public string ProtocolOid { get; init; }
    public ChipAuthenticationPublicKeyInfo PublicKeyInfo { get; init; }

    //TODO may also need these as a fallback if the key does not contain all the required info.
    //caSessionArgs.getCaPublicKeyInfo().getKeyId(),
    //caSessionArgs.getCaPublicKeyInfo().getObjectIdentifier(),
}