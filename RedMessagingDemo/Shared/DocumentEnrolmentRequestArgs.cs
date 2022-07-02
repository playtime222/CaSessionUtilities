namespace RedMessagingDemo.Shared;

public record DocumentEnrolmentRequestArgs
{
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Security descriptions
    /// </summary>
    public string DataGroup14Base64 { get; set; }
    
    /// <summary>
    /// AKA Ef.Sod
    /// </summary>
    public string DocumentSecurityObjectBase64 { get; set; }

    //Extracted from DG14 for convenience.
    public ChipAuthenticationProtocolInfo ChipAuthenticationProtocolInfo { get; set; }

    //Extracted from Ef.Sod for convenience.
    public DocumentSecurityObjectInfo DocumentSecurityObjectInfo { get; set; }

    //TODO multiple
    public int FileId { get; set; }
    public string FileContentsBase64 { get; set; }
    public int FileReadLength { get; set; }
}