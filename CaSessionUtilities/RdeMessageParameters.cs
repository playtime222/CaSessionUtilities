namespace CaSessionUtilities;

public record RdeMessageParameters
{
    public byte[] EphemeralPublicKey { get; init; }
    public byte[] WrappedCommand { get; init; }
    public byte[] WrappedResponse { get; init; }
}