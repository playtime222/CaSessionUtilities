namespace CaSessionUtilities.Messaging;

public interface IMessageEncoder {

    byte[] encode(MessageContentArgs messageArgs, RdeSessionArgs messageCryptoArgs, /*SecretKey*/ byte[] secretKey);
}
