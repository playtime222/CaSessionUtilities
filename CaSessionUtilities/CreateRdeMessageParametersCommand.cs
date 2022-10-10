﻿using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Utilities;

namespace CaSessionUtilities;

public class CreateRdeMessageParametersCommand
{
    public RdeMessageParameters Execute(RdeMessageCreateArgs args)
    {
        var caProtocol = EACCAProtocol.doCA(args.CaSessionArgs.PublicKeyInfo.KeyId, args.CaSessionArgs.ProtocolOid, args.CaSessionArgs.PublicKeyInfo.Oid, args.CaSessionArgs.PublicKeyInfo.PublicKey);
        return DebugTest(args, caProtocol);
    }

    public static RdeMessageParameters DebugTest(RdeMessageCreateArgs args, RdeEACCAResult caProtocol)
    {
        var plainCommand = new CommandApdu(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, 0x80 | (args.FileShortId & 0xFF), 0, args.ReadLength);
        var wrappedCommand = new CommandEncoder(caProtocol.Wrapper).Encode(plainCommand).ToArray();
        var plainResponse = Arrays.CopyOf(args.FileContent, args.ReadLength);
        var wrappedResponse = new ResponseEncoder(caProtocol.Wrapper).Write(plainResponse);

        var result = new RdeMessageParameters()
        {
            EphemeralPublicKey = caProtocol.PcdPublicKey,
            WrappedCommand = wrappedCommand,
            WrappedResponse = wrappedResponse,
        };
        return result;
    }
}