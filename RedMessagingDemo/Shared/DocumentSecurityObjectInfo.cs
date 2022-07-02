namespace RedMessagingDemo.Shared;

public record DocumentSecurityObjectInfo
{
    /// <summary>
    /// TODO Not actually used as currently the demo does not verify DG signatures.
    /// </summary>
    public string VerficationPublicKeyBase64 { get; init; }
}