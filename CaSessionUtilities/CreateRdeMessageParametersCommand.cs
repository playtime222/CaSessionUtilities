using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public class CreateRdeMessageParametersCommand
{
    public RdeMessageParameters Execute(RdeMessageCreateArgs args)
    {
        var caProtocol = EACCAProtocol.doCA(args.CaSessionArgs.PublicKeyInfo.KeyId, args.CaSessionArgs.ProtocolOid, args.CaSessionArgs.PublicKeyInfo.Oid, args.CaSessionArgs.PublicKeyInfo.PublicKey);
        var plainCommand = new CommandAPDU(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, 0x80 | (args.FileShortId & 0xFF), 0, args.ReadLength);
        var wrappedCommand =  new CommandEncoder(caProtocol.Wrapper).Encode(plainCommand).ToArray();
        var plainResponse = Arrays.CopyOf(args.FileContent, args.ReadLength);
        var wrappedResponse = new ResponseEncoder(caProtocol.Wrapper).Write(plainResponse);

        return new() { EphemeralPublicKey = caProtocol.PcdPublicKey, WrappedCommand = wrappedCommand, WrappedResponse = wrappedResponse };
    }
}