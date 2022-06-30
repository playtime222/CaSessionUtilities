namespace CaSessionUtilities;

/// <summary>
/// Should contain RDE 'Certificate'
/// TODO NB read all the DG14 oids before this point
/// </summary>
public record RdeMessageCreateArgs
{
    public CaSessionArgs CaSessionArgs { get; init; }
    public byte FileShortId { get; init; }
    public byte[] FileContent { get; init; }

    /// <summary>
    /// Max is 256 cos max transceive length
    /// </summary>
    public byte ReadLength { get; init; }
}