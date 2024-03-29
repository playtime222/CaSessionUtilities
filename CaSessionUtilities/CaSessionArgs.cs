﻿namespace CaSessionUtilities;

/// <summary>
/// From enrolment, specifically DG14
/// </summary>
public record CaSessionArgs
{
    //caSessionArgs.getCaPublicKeyInfo().getKeyId(),
    //caSessionArgs.getCaPublicKeyInfo().getObjectIdentifier(),
    //caSessionArgs.getCaPublicKeyInfo().getSubjectPublicKey());
    //caSessionArgs.getCaInfo().getObjectIdentifier(),
    public string ProtocolOid { get; init; }
    public ChipAuthenticationPublicKeyInfo PublicKeyInfo { get; init; }
}