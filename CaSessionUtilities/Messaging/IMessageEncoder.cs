namespace CaSessionUtilities.Messaging;

public interface IMessageEncoder {

    byte[] Encode(MessageContentArgs messageArgs, RdeMessageDecryptionInfo messageCryptoArgs, byte[] secretKey);
}
